using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private float moveRate = 0.2f;
    [SerializeField] private int initialSize = 3;
    [SerializeField] private GameObject bodyPrefab;

    private List<Vector3> _positionHistory = new List<Vector3>();
    private List<Transform> _bodyParts = new List<Transform>();
    private Queue<Vector2Int> _directionQueue = new Queue<Vector2Int>();
    private Vector2Int _currentDirection = Vector2Int.right;
    private PlayerInput _playerInput;
    private InputAction _moveAction;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
    }

    void OnEnable()
    {
        _moveAction.performed += OnMoveInput;
    }

    void OnDisable()
    {
        _moveAction.performed -= OnMoveInput;
    }

    private void OnMoveInput(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        if (input == Vector2.zero) return;

        Vector2Int newDir;
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            newDir = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            newDir = input.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        if (newDir + _currentDirection != Vector2Int.zero &&
            (_directionQueue.Count == 0 || _directionQueue.Peek() != newDir))
        {
            _directionQueue.Enqueue(newDir);
        }
    }

    void Start()
    {
        InitializeBody();
        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }

            if (_directionQueue.Count > 0)
            {
                Vector2Int next = _directionQueue.Dequeue();
                if (next + _currentDirection != Vector2Int.zero)
                {
                    _currentDirection = next;
                }
            }

            RotateHead();

            Vector3 headStart = transform.position;
            Vector3 headTarget = headStart + new Vector3(_currentDirection.x, _currentDirection.y, 0);

            if (IsCollision(headTarget))
            {
                GameOver();
                yield break;
            }

            _positionHistory.Insert(0, headTarget);

            Vector3[] startPositions = new Vector3[_bodyParts.Count];
            Vector3[] targetPositions = new Vector3[_bodyParts.Count];

            for (int i = 0; i < _bodyParts.Count; i++)
            {
                startPositions[i] = _bodyParts[i].position;
            }

            targetPositions[0] = headTarget;
            for (int i = 1; i < _bodyParts.Count; i++)
            {
                targetPositions[i] = _positionHistory[i];
            }

            float elapsed = 0f;
            while (elapsed < moveRate)
            {
                float t = elapsed / moveRate;
                for (int i = 0; i < _bodyParts.Count; i++)
                {
                    _bodyParts[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < _bodyParts.Count; i++)
            {
                _bodyParts[i].position = targetPositions[i];
            }

            if (_positionHistory.Count > _bodyParts.Count)
            {
                _positionHistory.RemoveAt(_positionHistory.Count - 1);
            }

            CheckFood();

            yield return null; 
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();
        GameManager.Instance.GameOver();
    }

    private bool IsCollision(Vector3 nextPos)
    {
        if (nextPos.x < -3.5f || nextPos.x > 3.5f ||
            nextPos.y < -3.5f || nextPos.y > 3.5f)
        {
            return true;
        }

        for (int i = 1; i < _bodyParts.Count; i++)
        {
            if (_bodyParts[i].position == nextPos)
            {
                return true;
            }
        }

        return false;
    }

    private void RotateHead()
    {
        float angle = 0;
        if (_currentDirection == Vector2Int.up)
        {
            angle = 90;
        }
        else if (_currentDirection == Vector2Int.left)
        {
            angle = 180;
        }
        else if (_currentDirection == Vector2Int.down)
        {
            angle = -90;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Grow()
    {
        Transform segment = Instantiate(bodyPrefab).transform;
        Vector3 lastPos = _positionHistory[_positionHistory.Count - 1];
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
        for (int i = 0; i < _bodyParts.Count; i++)
        {
            _positionHistory.Add(_bodyParts[i].position);
        }
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
        {
            positions.Add(part.position);
        }
        return positions;
    }
}