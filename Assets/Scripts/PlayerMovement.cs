using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Tilemap blockedTilemap;
    [SerializeField] private Tilemap boxTilemap;
    private Rigidbody2D playerRb;
    private WalkableTileChecker tileChecker;
    private List<GameObject> allBoxes;

    public void MoveUp() => Move(Vector2Int.up);
    public void MoveDown() => Move(Vector2Int.down);
    public void MoveLeft() => Move(Vector2Int.left);
    public void MoveRight() => Move(Vector2Int.right);

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        tileChecker = GetComponent<WalkableTileChecker>();
    }

    private void Start()
    {
        allBoxes = new List<GameObject>();
        foreach (var box in FindObjectsByType<PushableBox>(FindObjectsSortMode.None))
        {
            allBoxes.Add(box.gameObject);
        }
    }


    private void Move(Vector2Int dir)
    {
        Vector3Int targetCell = tileChecker.GetCellInDirection(transform.position, dir);

        if (tileChecker.IsWalkable(targetCell))
        {
            playerRb.MovePosition(tileChecker.GetWorldPosition(targetCell));
        }

        else
        {
            foreach (var box in FindObjectsByType<PushableBox>(FindObjectsSortMode.None))
            {
                Vector3Int boxCell = boxTilemap.WorldToCell(box.transform.position);
                if (boxCell == targetCell)
                {
                    bool pushed = box.TryPush(dir, blockedTilemap, allBoxes);

                    if (pushed)
                    {
                        playerRb.MovePosition(tileChecker.GetWorldPosition(targetCell));
                    }

                    break;
                }
            }
        }
    }
}