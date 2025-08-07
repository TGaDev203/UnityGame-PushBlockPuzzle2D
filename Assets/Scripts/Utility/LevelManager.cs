using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] levelPrefabs;
    private GameObject currentLevel;

    private void Start()
    {
        DebugManager.instance.enableRuntimeUI = false;
        LoadLevel(1);
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
}
