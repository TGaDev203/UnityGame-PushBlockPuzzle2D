using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite onPointSprite;
    [SerializeField] private float moveSpeed;
    private SpriteRenderer spriteRenderer;
    private bool isOnPoint = false;

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

        StartCoroutine(MoveToCell(targetCell, dir, gridManager));

        // bool isOnPoint = false;

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

    private IEnumerator MoveToCell(Vector3Int targetCell, Vector2Int dir, GridManager gridManager)
    {
        // isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = gridManager.GetWorldCenter(targetCell);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        // isMoving = false;
    }

    public void UpdateSprite(List<Transform> allPoints, GridManager gridManager)
    {
        Debug.Log("Change");
        Vector3Int myCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);

        foreach (var point in allPoints)
        {
            Vector3Int pointCell = gridManager.GetCellInDirection(point.position, Vector2Int.zero);

            if (pointCell == myCell)
            {
                isOnPoint = true;
                break;
            }
        }

        spriteRenderer.sprite = isOnPoint ? onPointSprite : normalSprite;
    }

    public bool IsOnPoint()
    {
        return isOnPoint;
    }
}