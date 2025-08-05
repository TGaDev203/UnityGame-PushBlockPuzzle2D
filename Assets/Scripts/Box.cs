using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite onPointSprite;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool TryPush(Vector2Int dir, GridManager gridManager, List<Box> allBoxes, List<Transform> allPoints)
    {
        Vector3Int currentCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (gridManager.IsBlocked(targetCell)) return false;

        foreach (var box in allBoxes)
        {
            if (box == this) continue;

            Vector3Int boxCell = gridManager.GetCellInDirection(box.transform.position, Vector2Int.zero);

            if (boxCell == targetCell) return false;
        }

        transform.position = gridManager.GetWorldCenter(targetCell);

        bool isOnPoint = false;

        foreach (var point in allPoints)
        {
            Vector3Int pointCell = gridManager.GetCellInDirection(point.position, Vector2Int.zero);

            if (pointCell == targetCell)
            {
                isOnPoint = true;
            }

        }
        spriteRenderer.sprite = isOnPoint ? onPointSprite : normalSprite;

        return true;
    }
}