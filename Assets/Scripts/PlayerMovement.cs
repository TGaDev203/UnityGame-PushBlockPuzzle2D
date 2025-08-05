using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Tilemap desPointTilemap;
    private List<Box> allBoxes;
    private List<Transform> allPoints;

    public void MoveUp() => Move(Vector2Int.up);
    public void MoveDown() => Move(Vector2Int.down);
    public void MoveLeft() => Move(Vector2Int.left);
    public void MoveRight() => Move(Vector2Int.right);

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
        Vector3Int currentCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (gridManager.IsBlocked(targetCell)) return;

        if (BoxAtCell(targetCell))
        {
            foreach (var box in allBoxes)
            {
                Vector3Int boxCell = gridManager.GetCellInDirection(box.transform.position, Vector2Int.zero);

                if (boxCell != targetCell) continue;

                Vector3Int afterBoxCell = targetCell + new Vector3Int(dir.x, dir.y, 0);

                if (gridManager.IsBlocked(afterBoxCell) || BoxAtCell(afterBoxCell)) return;

                if (box.TryPush(dir, gridManager, allBoxes, allPoints)) transform.position = gridManager.GetWorldCenter(targetCell);

                return;
            }
        }

        else transform.position = gridManager.GetWorldCenter(targetCell);
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
}