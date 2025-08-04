using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Tilemap blockedTilemap;
    [SerializeField] private Tilemap walkableTilemap;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2Int direction)
    {
        Vector3Int currentCell = walkableTilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(direction.x, direction.y, 0);

        if (IsWalkable(targetCell))
        {
            Vector3 targetWorldPos = walkableTilemap.GetCellCenterWorld(targetCell);
            playerRb.MovePosition(targetWorldPos);
        }
    }

    private bool IsWalkable(Vector3Int cellPos)
    {
        return walkableTilemap.HasTile(cellPos) && !blockedTilemap.HasTile(cellPos);
    }

    public void MoveUp()
    {
        Move(Vector2Int.up);
    }

    public void MoveDown()
    {
        Move(Vector2Int.down);
    }

    public void MoveLeft()
    {
        Move(Vector2Int.left);
    }

    public void MoveRight()
    {
        Move(Vector2Int.right);
    }
}