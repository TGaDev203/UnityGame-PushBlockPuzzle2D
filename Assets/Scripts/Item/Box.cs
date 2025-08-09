using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite onPointSprite;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("State")]
    private bool isOnPoint = false;

    [Header("Component")]
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

        StartCoroutine(MoveBoxToCell(targetCell, dir, gridManager));
        SetSpriteBasedOnPoint(allPoints, gridManager, targetCell);

        return true;
    }

    private IEnumerator MoveBoxToCell(Vector3Int targetCell, Vector2Int dir, GridManager gridManager)
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

    public void RefreshSpriteOnLoad(List<Transform> allPoints, GridManager gridManager)
    {
        Vector3Int myCell = gridManager.GetCellInDirection(transform.position, Vector2Int.zero);

        SetSpriteBasedOnPoint(allPoints, gridManager, myCell);
    }

    private void SetSpriteBasedOnPoint(List<Transform> allPoints, GridManager gridManager, Vector3Int cellToCheck)
    {
        isOnPoint = false;

        foreach (var point in allPoints)
        {
            Vector3Int pointCell = gridManager.GetCellInDirection(point.position, Vector2Int.zero);

            if (pointCell == cellToCheck)
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

    public void SetOnPointState(bool onPoint)
    {
        isOnPoint = onPoint;
        spriteRenderer.sprite = isOnPoint ? onPointSprite : normalSprite;
    }
}