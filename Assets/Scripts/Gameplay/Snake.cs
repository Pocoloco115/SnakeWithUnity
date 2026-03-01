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

    private List<Transform> _bodyParts = new List<Transform>();

    private Vector2Int _direction = Vector2Int.right;
    private Vector2Int _nextDirection = Vector2Int.right;

    private PlayerInput _playerInput;
    private InputAction _moveAction;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];

        InitializeBody();

        StartCoroutine(MoveRoutine());
    }

    void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            return;
        }

        Vector2Int newDir;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            newDir = input.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            newDir = input.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        if (newDir + _direction != Vector2Int.zero)
        {
            _nextDirection = newDir;
        }
    }
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveRate);

            _direction = _nextDirection;

            MoveBody();
            MoveHead();
            RotateHead();
        }
    }
    private void MoveHead()
    {
        transform.position += new Vector3(_direction.x, _direction.y, 0);
    }

    private void MoveBody()
    {
        for (int i = _bodyParts.Count - 1; i > 0; i--)
        {
            _bodyParts[i].position = _bodyParts[i - 1].position;
        }
    }
    private void RotateHead()
    {
        float angle = 0;

        if (_direction == Vector2Int.up)
        {
            angle = 90;
        }
        else if (_direction == Vector2Int.left)
        {
            angle = 180;
        }
        else if (_direction == Vector2Int.down)
        {
            angle = -90;
        }
        else if (_direction == Vector2Int.right)
        {
            angle = 0;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void Grow()
    {
        Transform segment = Instantiate(bodyPrefab).transform;
        segment.position = _bodyParts[_bodyParts.Count - 1].position;
        _bodyParts.Add(segment);
    }

    private void InitializeBody()
    {
        _bodyParts.Clear();
        _bodyParts.Add(this.transform);

        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(bodyPrefab).transform;
            segment.position = transform.position;
            _bodyParts.Add(segment);
        }
    }
}