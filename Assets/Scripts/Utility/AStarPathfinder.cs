using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] GridManager gridManager;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    //* -------------------- NODE CLASS --------------------

    private class Node
    {
        public Vector3Int position;
        public Node parent;
        public float gCost; // Cost from start to this node
        public float hCost; // Estimated cost from this to end node
        public float fCost => gCost + hCost; // Total cost

        public Node(Vector3Int pos) => position = pos;
    }

    //* -------------------- PATHFINDING HELPERS --------------------

    // Caculate Manhattan distance between 2 cells
    private float GetDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Get neighbor movable cells (up, down, left, right)
    private List<Vector3Int> GetNeighbors(Vector3Int current)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = current + dir;
            if (!gridManager.IsBlocked(neighborPos)) neighbors.Add(neighborPos);
        }

        return neighbors;
    }

    //* -------------------- PATHFINDING ALGORITHM --------------------

    // Path Finder
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        List<Node> openList = new List<Node>();

        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Node startNode = new Node(start);
        startNode.gCost = 0;
        startNode.hCost = GetDistance(start, goal);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Get the node with smallest fCost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            if (currentNode.position == goal)
            {
                // Path found, trace back and return path
                return RetracePath(currentNode);
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.position);

            foreach (var neighborPos in GetNeighbors(currentNode.position))
            {
                if (closedSet.Contains(neighborPos)) continue;

                float tentativeGCost = currentNode.gCost + 1; // Assume cost of each step is  1

                Node neighborNode = openList.Find(n => n.position == neighborPos);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborPos);
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = GetDistance(neighborPos, goal);
                    neighborNode.parent = currentNode;
                    openList.Add(neighborNode);
                }

                else if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.parent = currentNode;
                }
            }
        }

        // Path not found
        return null;
    }

    //* -------------------- PATH RECONSTRUCTION --------------------

    // Trace back the path from goal to start
    private List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
}
