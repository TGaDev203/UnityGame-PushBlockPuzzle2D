using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private GridManager gridManager;
    private GameObject currentLevel;
    private List<Transform> allPoints;
    private List<Box> allBoxes;

    private void Start()
    {
        DebugManager.instance.enableRuntimeUI = false;
        StartCoroutine(LoadLevelWithDelay(1));
    }

    private void Update()
    {
        CheckLevelComplete();
    }

    private IEnumerator LoadLevelWithDelay(int index)
    {
        LoadLevel(index);
        yield return null;

        InitPointsAndBoxes();
    }

    private void LoadLevel(int index)
    {
        if (currentLevel != null) Destroy(currentLevel);

        currentLevel = Instantiate(levelPrefabs[index], Vector3.zero, Quaternion.identity, transform);

        foreach (Transform t in currentLevel.GetComponentInChildren<Transform>())
        {
            if (t.CompareTag("SpawnPoint"))
            {
                player.transform.position = t.position;
                break;
            }
        }
    }

    private void InitPointsAndBoxes()
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
            box.UpdateSprite(allPoints, gridManager);
        }
    }

    private void CheckLevelComplete()
    {
        
    }
}
