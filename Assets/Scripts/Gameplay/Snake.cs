using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private float moveRate = 0.2f;
    [SerializeField] private int initialSize = 3;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject curvePrefab;

    private List<Vector3> _positionHistory = new List<Vector3>();
    private List<Transform> _bodyParts = new List<Transform>();
    private Vector2Int _currentDirection = Vector2Int.right;
    private Vector2Int _previousDirection;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private Dictionary<Vector3, GameObject> _turnCurves = new Dictionary<Vector3, GameObject>();

    private float moveTimer;
    private bool isMoving;
    private Vector3[] startPositions;
    private Vector3[] targetPositions;
    private float stepElapsed;

    // Última dirección pendiente
    private Vector2Int _pendingDirection = Vector2Int.zero;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
    }

    void OnEnable() => _moveAction.performed += OnMoveInput;
    void OnDisable() => _moveAction.performed -= OnMoveInput;

    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input == Vector2.zero) return;

        Vector2Int newDir = Mathf.Abs(input.x) > Mathf.Abs(input.y)
            ? (input.x > 0 ? Vector2Int.right : Vector2Int.left)
            : (input.y > 0 ? Vector2Int.up : Vector2Int.down);

        // Guardamos la última dirección válida
        if (newDir + _currentDirection != Vector2Int.zero)
            _pendingDirection = newDir;
    }

    void Start()
    {
        InitializeBody();
        _previousDirection = _currentDirection;
        moveTimer = moveRate;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (!isMoving)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                Move();          // inicia el paso
                stepElapsed = 0f;
                isMoving = true;
                moveTimer = moveRate;
            }
        }
        else
        {
            InterpolateStep();   // suaviza con Lerp
        }
    }

    private void Move()
    {
        _previousDirection = _currentDirection;

        // Aplicamos la última dirección pendiente
        if (_pendingDirection != Vector2Int.zero && _pendingDirection != -_currentDirection)
        {
            SpawnCurve(transform.position, _previousDirection, _pendingDirection);
            _currentDirection = _pendingDirection;
            _pendingDirection = Vector2Int.zero;
        }

        RotateHead();
        Vector3 headTarget = transform.position + new Vector3(_currentDirection.x, _currentDirection.y, 0f);

        if (IsCollision(headTarget))
        {
            GameOver();
            return;
        }

        _positionHistory.Insert(0, headTarget);

        startPositions = new Vector3[_bodyParts.Count];
        targetPositions = new Vector3[_bodyParts.Count];

        for (int i = 0; i < _bodyParts.Count; i++)
            startPositions[i] = _bodyParts[i].position;

        targetPositions[0] = headTarget;
        for (int i = 1; i < _bodyParts.Count; i++)
            targetPositions[i] = _positionHistory[i];

        stepElapsed = 0f;
        isMoving = true;
    }

    private void InterpolateStep()
    {
        stepElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(stepElapsed / moveRate);

        for (int i = 0; i < _bodyParts.Count; i++)
            _bodyParts[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);

        if (t >= 1f)
        {
            isMoving = false;
            for (int i = 0; i < _bodyParts.Count; i++)
                _bodyParts[i].position = targetPositions[i];

            if (_positionHistory.Count > _bodyParts.Count)
                _positionHistory.RemoveAt(_positionHistory.Count - 1);

            HandleCurves();
            CheckFood();
        }
    }

    private void HandleCurves()
    {
        if (_bodyParts.Count > 0)
        {
            Vector3 tailCurrentPos = _bodyParts[^1].position;
            List<Vector3> keysToRemove = new List<Vector3>();
            foreach (var kvp in _turnCurves)
            {
                if (Vector3.Distance(tailCurrentPos, kvp.Key) < 0.05f)
                {
                    if (kvp.Value != null) Destroy(kvp.Value);
                    keysToRemove.Add(kvp.Key);
                }
            }
            foreach (var key in keysToRemove)
                _turnCurves.Remove(key);
        }
    }

    private void SpawnCurve(Vector3 position, Vector2Int fromDir, Vector2Int toDir)
    {
        GameObject curveObj = Instantiate(curvePrefab, position, Quaternion.identity);
        _turnCurves[position] = curveObj;
    }

    private void GameOver()
    {
        foreach (var curve in _turnCurves.Values)
            if (curve != null) Destroy(curve);
        _turnCurves.Clear();
        GameManager.Instance.GameOver();
    }

    private bool IsCollision(Vector3 nextPos)
    {
        if (nextPos.x < -3.5f || nextPos.x > 3.5f ||
            nextPos.y < -3.5f || nextPos.y > 3.5f)
            return true;

        for (int i = 1; i < _bodyParts.Count; i++)
            if (_bodyParts[i].position == nextPos)
                return true;

        return false;
    }

    private void RotateHead()
    {
        float angle = 0;
        if (_currentDirection == Vector2Int.up) angle = 90;
        else if (_currentDirection == Vector2Int.left) angle = 180;
        else if (_currentDirection == Vector2Int.down) angle = -90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Grow()
    {
        Transform segment = Instantiate(bodyPrefab).transform;
        Vector3 lastPos = _positionHistory[^1];
        segment.position = lastPos;
        _bodyParts.Add(segment);
        _positionHistory.Add(lastPos);
    }

    private void InitializeBody()
    {
        _bodyParts.Clear();
        _bodyParts.Add(this.transform);
        Vector3 spawnPos = transform.position;
        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(bodyPrefab).transform;
            spawnPos -= new Vector3(_currentDirection.x, _currentDirection.y, 0);
            segment.position = spawnPos;
            _bodyParts.Add(segment);
        }
        _positionHistory.Clear();
        foreach (var part in _bodyParts)
            _positionHistory.Add(part.position);
        _turnCurves.Clear();
    }

    private void CheckFood()
    {
        GameObject food = GameObject.FindWithTag("Food");
        if (food == null) return;
        if (food.transform.position == transform.position)
        {
            Grow();
            Destroy(food);
            FoodSpawner.Instance.SpawnFood();
        }
    }

    public List<Vector3> GetOccupiedPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform part in _bodyParts)
            positions.Add(part.position);
        return positions;
    }
}