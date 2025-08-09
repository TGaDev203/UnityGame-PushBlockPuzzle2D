using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public Vector3 playerPosition;
    public Dictionary<Box, Vector3> boxPositions;
    public Dictionary<Box, bool> boxOnpointStates;
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float moveSpeed = 5f;
    private Stack<GameState> history = new Stack<GameState>();
    private bool isMoving = false;
    private List<Box> allBoxes;
    private List<Transform> allPoints;
    private Vector2Int lastDirection;
    private PlayerAnimation playerAnim;
    private bool canMove = false;

    public void MoveUp() => Move(Vector2Int.up);
    public void MoveDown() => Move(Vector2Int.down);
    public void MoveLeft() => Move(Vector2Int.left);
    public void MoveRight() => Move(Vector2Int.right);
    public void EnableMovement() => canMove = true;
    public void DisableMovement() => canMove = false;

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
        if (isMoving || !canMove) return;

        Vector3Int currentCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (gridManager.IsBlocked(targetCell)) return;

        SaveState();

        if (BoxAtCell(targetCell))
        {
            foreach (var box in allBoxes)
            {
                Vector3Int boxCell = gridManager.GetCellInDirection(box.transform.position, Vector2Int.zero);
                if (boxCell != targetCell) continue;

                Vector3Int afterBoxCell = targetCell + new Vector3Int(dir.x, dir.y, 0);

                if (gridManager.IsBlocked(afterBoxCell) || BoxAtCell(afterBoxCell)) return;

                if (box.TryPush(dir, gridManager, allBoxes, allPoints))
                {
                    StartCoroutine(MoveToCell(targetCell, dir));
                    return;
                }
            }
        }
        else StartCoroutine(MoveToCell(targetCell, dir));
    }

    private IEnumerator MoveToCell(Vector3Int targetCell, Vector2Int dir)
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
        playerAnim.SetIsWalking(false);
        isMoving = false;
    }

    private void SaveState()
    {
        GameState state = new GameState();
        state.playerPosition = transform.position;
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
        if (history.Count == 0)
        {
            Debug.Log("No moves to undo");
            return;
        }

        GameState prevState = history.Pop();
        transform.position = prevState.playerPosition;

        foreach (var box in allBoxes)
        {
            if (prevState.boxPositions.ContainsKey(box))
            {
                box.transform.position = prevState.boxPositions[box];
                if (prevState.boxOnpointStates.TryGetValue(box, out bool wasOnPoint)) box.SetOnPointState(wasOnPoint);
            }
        }
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
}