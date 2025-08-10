using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject gameObjSpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject undoButton;
    [SerializeField] private GameObject moveButton;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI stepText;
    [SerializeField] private TextMeshProUGUI levelText;
    private GameStateManager gameStateManager;

    [Header("Tilemaps")]
    private Tilemap boxTileMap;
    private Tilemap pointTilemap;
    private Tilemap walkableTilemap;

    [Header("Level State")]
    private List<Transform> allPoints;
    private List<Box> allBoxes;
    private GameObject currentLevel;
    private int currentLevelIndex;
    private bool isLevelCompleted = false;
    private int totalBox = 0;

    private void Awake()
    {
        gameStateManager = GetComponent<GameStateManager>();
    }

    private void Start()
    {
        if (!gameStateManager.HasStarted()) return;

        InitTilemap();

        DebugManager.instance.enableRuntimeUI = false;
        currentLevelIndex = 1;
        StartCoroutine(LoadLevelWithDelay(currentLevelIndex));

        moveButton.gameObject.SetActive(true);
        targetText.gameObject.SetActive(true);
        stepText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
    }

    private void Update()
    {
        CheckLevelComplete();
    }

    private IEnumerator LoadLevelWithDelay(int index)
    {
        player.SetActive(false);
        UnloadCurrentLevel();

        currentLevel = Instantiate(levelPrefabs[index], Vector3.zero, Quaternion.identity, transform);

        yield return null;

        InitTilemap();
        SpawnBoxesAndPointsFromTilemap();

        yield return null;

        SetPlayerToSpawnPoint();

        SetupPlayerAndLevel();
        player.SetActive(true);
    }

    private void UnloadCurrentLevel()
    {
        if (currentLevel == null) return;

        playerMovement.ClearHistoryState();
        playerMovement.stepCounter = 0;
        player.transform.position = Vector3.zero;
        playerMovement.StopAllCoroutines();
        Destroy(currentLevel);
    }

    private void SetPlayerToSpawnPoint()
    {
        Transform spawnPoint = FindSpawnPoint();

        if (spawnPoint == null)
        {
            Debug.Log("SpawnPoint not found in level");
            return;
        }

        player.transform.position = spawnPoint.position;
    }

    private void SetupPlayerAndLevel()
    {
        UpdateBoxesSpritesOnLoad();
        playerMovement.SetBoxesAndPoints(allBoxes, allPoints);
        playerMovement.EnableMovement();
        isLevelCompleted = false;
    }

    private Transform FindSpawnPoint()
    {
        Transform spawnPoint = currentLevel.transform.Find("SpawnPoint");
        if (spawnPoint != null) return spawnPoint;

        foreach (Transform t in currentLevel.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("SpawnPoint")) return t;
        }

        return null;
    }

    public void InitTilemap()
    {
        GameObject level = GameObject.FindGameObjectWithTag("Level");

        if (level != null)
        {
            TilemapMarker[] markers = level.GetComponentsInChildren<TilemapMarker>();

            foreach (var marker in markers)
            {
                switch (marker.type)
                {
                    case TilemapMarker.TilemapType.Walkable:
                        walkableTilemap = marker.GetComponent<Tilemap>();
                        GridManager.Instance.SetWalkableTilemap(walkableTilemap);
                        break;

                    case TilemapMarker.TilemapType.Box:
                        boxTileMap = marker.GetComponent<Tilemap>();
                        break;

                    case TilemapMarker.TilemapType.Point:
                        pointTilemap = marker.GetComponent<Tilemap>();
                        break;
                }
            }
        }
    }

    private void SpawnBoxesAndPointsFromTilemap()
    {
        foreach (Transform child in gameObjSpawner.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn box
        foreach (Vector3Int pos in boxTileMap.cellBounds.allPositionsWithin)
        {
            if (!boxTileMap.HasTile(pos)) continue;
            Vector3 worldPos = boxTileMap.GetCellCenterWorld(pos);
            Instantiate(boxPrefab, worldPos, Quaternion.identity, gameObjSpawner.transform);
            totalBox = CountTiles(boxTileMap);
        }

        // Spawn point
        foreach (Vector3Int pos in pointTilemap.cellBounds.allPositionsWithin)
        {
            if (!pointTilemap.HasTile(pos)) continue;
            Vector3 worldPos = pointTilemap.GetCellCenterWorld(pos);
            GameObject point = Instantiate(pointPrefab, worldPos, Quaternion.identity, gameObjSpawner.transform);
            point.tag = "Point";
        }

        boxTileMap.gameObject.SetActive(false);
        pointTilemap.gameObject.SetActive(false);
    }

    private void UpdateBoxesSpritesOnLoad()
    {
        allPoints = new List<Transform>();
        GameObject[] pointObjs = GameObject.FindGameObjectsWithTag("Point");
        foreach (var p in pointObjs)
        {
            allPoints.Add(p.transform);
        }

        allBoxes = new List<Box>(FindObjectsByType<Box>(FindObjectsSortMode.None));

        foreach (var box in allBoxes)
        {
            box.RefreshSpriteOnLoad(allPoints);
        }
    }

    private void CheckLevelComplete()
    {
        if (isLevelCompleted) return;

        undoButton.gameObject.SetActive(playerMovement.HasMoved());

        targetText.text = $"{GetBoxesOnPointCount()} / {totalBox}";
        levelText.text = $"Level {currentLevelIndex}";
        stepText.text = $"{playerMovement.stepCounter}";

        if (allBoxes == null || allBoxes.Count == 0) return;

        bool allOnPoint = allBoxes.All(box => box.IsOnPoint());
        if (!allOnPoint) return;

        isLevelCompleted = true;
        playerMovement.DisableMovement();
        SoundManager.Instance.PlayLevelCompleteSound();
        StartCoroutine(DelayBeforeLoadNextLevel());
    }

    private int CountTiles(Tilemap tilemap)
    {
        int count = 0;
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos)) count++;
        }
        return count;
    }

    public int GetBoxesOnPointCount()
    {
        int count = 0;

        if (allBoxes == null) return 0;

        foreach (var box in allBoxes)
        {
            if (box.IsOnPoint()) count++;
        }
        return count;
    }

    private IEnumerator DelayBeforeLoadNextLevel()
    {
        yield return new WaitForSeconds(1.5f);

        if (currentLevelIndex >= levelPrefabs.Length)
        {
            Debug.Log("Game Completed!");
            yield break;
        }

        currentLevelIndex += 1;
        StartCoroutine(LoadLevelWithDelay(currentLevelIndex));
    }
}