using UnityEngine;

public class TilemapMarker : MonoBehaviour
{
    public enum TilemapType { Box, Point, Wall, Walkable }

    public TilemapType type;
}