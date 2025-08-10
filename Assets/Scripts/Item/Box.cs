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

    public bool TryPush(Vector2Int dir, List<Box> allBoxes, List<Transform> allPoints)
    {
        Vector3Int currentCell = GridManager.Instance.GetCellInDirection(transform.position, Vector2Int.zero);
        Vector3Int targetCell = currentCell + new Vector3Int(dir.x, dir.y, 0);

        if (GridManager.Instance.IsBlocked(targetCell)) return false;

        foreach (var box in allBoxes)
        {
            if (box == this) continue;

            Vector3Int boxCell = GridManager.Instance.GetCellInDirection(box.transform.position, Vector2Int.zero);

            if (boxCell == targetCell) return false;
        }

        bool wasOnPoint = isOnPoint;

        StartCoroutine(MoveBoxToCell(targetCell, dir, allPoints, wasOnPoint));

        return true;
    }

    private IEnumerator MoveBoxToCell(Vector3Int targetCell, Vector2Int dir, List<Transform> allPoints, bool wasOnPoint)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = GridManager.Instance.GetWorldCenter(targetCell);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        SetSpriteBasedOnPoint(allPoints, targetCell);

        if (!wasOnPoint && isOnPoint) SoundManager.Instance.PlayOnPointSound();
    }

    public void RefreshSpriteOnLoad(List<Transform> allPoints)
    {
        Vector3Int myCell = GridManager.Instance.GetCellInDirection(transform.position, Vector2Int.zero);

        SetSpriteBasedOnPoint(allPoints, myCell);
    }

    private void SetSpriteBasedOnPoint(List<Transform> allPoints, Vector3Int cellToCheck)
    {
        isOnPoint = false;

        foreach (var point in allPoints)
        {
            Vector3Int pointCell = GridManager.Instance.GetCellInDirection(point.position, Vector2Int.zero);

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