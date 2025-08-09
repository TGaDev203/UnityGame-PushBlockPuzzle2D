using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class GameState
{
    public Vector3 playerPosition;
    public Vector2Int playerDirection;
    public int stepsRecorded;
    public Dictionary<Box, Vector3> boxPositions;
    public Dictionary<Box, bool> boxOnpointStates;
}

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    private PlayerAnimation playerAnim;

    [Header("Movement State")]
    [SerializeField] private float moveSpeed;
    private bool canAcceptInput = false;
    private bool isMoving = false;
    private bool hasMoved = false;
    private Vector2Int lastDirection;

    [Header("Game Objects")]
    private List<Box> allBoxes;
    private List<Transform> allPoints;

    [Header("Undo History")]
    private Stack<GameState> history = new Stack<GameState>();

    [Header("Step Counter")]
    public int stepCounter = 0;

    public void MoveUp() => Move(Vector2Int.up);
    public void MoveDown() => Move(Vector2Int.down);
    public void MoveLeft() => Move(Vector2Int.left);
    public void MoveRight() => Move(Vector2Int.right);
    public void EnableMovement() => canAcceptInput = true;
    public void DisableMovement() => canAcceptInput = false;
    private bool CanMoveToCell(Vector3Int cell) => !gridManager.IsBlocked(cell);

    private void Awake()
    {
        playerAnim = GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        allBoxes = new List<Box>(FindObjectsByType<Box>(FindObjectsSortMode.None));

        allPoints = new List<Transform>();
        GameObject[] pointObjs = GameObject.FindGameObjectsWithTag("Point");

        foreach (var p in pointObjs)
        {
            allPoints.Add(p.transform);
        }
    }

    private void Move(Vector2Int dir)
    {
        if (isMoving || !canAcceptInput) return;

        Vector3Int currentCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (!CanMoveToCell(targetCell)) return;
        SaveState();

        if (TryPushBoxAtCell(targetCell, dir)) StartCoroutine(MovePlayerToCell(targetCell, dir));

        else if (!BoxAtCell(targetCell)) StartCoroutine(MovePlayerToCell(targetCell, dir));
    }

    private IEnumerator MovePlayerToCell(Vector3Int targetCell, Vector2Int dir)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = gridManager.GetWorldCenter(targetCell);

        playerAnim.SetIsWalking(true);
        lastDirection = dir;
        playerAnim.SetDirection(lastDirection);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        stepCounter++;
        playerAnim.SetIsWalking(false);
        isMoving = false;
    }

    private bool TryPushBoxAtCell(Vector3Int cell, Vector2Int dir)
    {
        Box boxToPush = GetBoxAtCell(cell);
        if (boxToPush == null) return false;

        Vector3Int afterBoxCell = cell + new Vector3Int(dir.x, dir.y, 0);

        if (gridManager.IsBlocked(afterBoxCell) || BoxAtCell(afterBoxCell)) return false;

        return boxToPush.TryPush(dir, gridManager, allBoxes, allPoints);
    }

    private Box GetBoxAtCell(Vector3Int cell)
    {
        foreach (var box in allBoxes)
        {
            Vector3Int boxCell = gridManager.GetCellInDirection(box.transform.position, Vector2Int.zero);

            if (boxCell == cell) return box;
        }
        return null;
    }

    private void SaveState()
    {
        GameState state = new GameState();
        state.playerPosition = transform.position;
        state.playerDirection = lastDirection;
        state.stepsRecorded = stepCounter;
        state.boxPositions = new Dictionary<Box, Vector3>();
        state.boxOnpointStates = new Dictionary<Box, bool>();

        foreach (var box in allBoxes)
        {
            state.boxPositions[box] = box.transform.position;
            state.boxOnpointStates[box] = box.IsOnPoint();
        }

        history.Push(state);
    }

    public void UndoMove()
    {
        if (history.Count == 0) return;

        GameState prevState = history.Pop();
        transform.position = prevState.playerPosition;
        lastDirection = prevState.playerDirection;
        stepCounter = prevState.stepsRecorded;

        playerAnim.SetDirection(lastDirection);

        foreach (var box in allBoxes)
        {
            if (prevState.boxPositions.TryGetValue(box, out Vector3 pos))
            {
                box.transform.position = pos;

                if (prevState.boxOnpointStates.TryGetValue(box, out bool wasOnPoint)) box.SetOnPointState(wasOnPoint);
            }
        }

        if (history.Count == 0) playerAnim.SetDirection(Vector2Int.down);
    }

    public void ClearHistoryState()
    {
        history.Clear();
        playerAnim.SetDirection(Vector2Int.down);
    }

    private bool BoxAtCell(Vector3Int cell)
    {
        foreach (var box in allBoxes)
        {
            Vector3Int boxCell = gridManager.GetCellInDirection(box.transform.position, Vector2Int.zero);

            if (boxCell == cell) return true;
        }
        return false;
    }

    public void SetBoxesAndPoints(List<Box> boxes, List<Transform> points)
    {
        allBoxes = boxes;
        allPoints = points;
    }

    public bool HasMoved()
    {
        hasMoved = history.Count > 0;
        return hasMoved;
    }
}