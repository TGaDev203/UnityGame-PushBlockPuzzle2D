using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string KEY_CURRENT_MODE = "CurrentMode";
    private const string KEY_NORMAL_COMPLETED = "NORMAL_LevelsCompleted";
    private const string KEY_HARD_COMPLETED = "Hard_LevelsCompleted";

    public enum GameMode
    {
        Normal = 0,
        Hard = 1
    }

    //* -------------------- UNITY LIFECYCLE --------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //* -------------------- SAVE & LOAD METHODS --------------------

    public void SaveCurrentMode(GameMode mode)
    {
        PlayerPrefs.SetInt(KEY_CURRENT_MODE, (int)mode);
        PlayerPrefs.Save();
    }

    public GameMode LoadCurrentMode()
    {
        int modeInt = PlayerPrefs.GetInt(KEY_CURRENT_MODE, 0);
        return (GameMode)modeInt;
    }

    public void SaveLevelCompleted(GameMode mode, int levelIndex)
    {
        string key = (mode == GameMode.Normal) ? KEY_NORMAL_COMPLETED : KEY_HARD_COMPLETED;

        string completedLevels = PlayerPrefs.GetString(key, "");
        HashSet<int> completedSet = new HashSet<int>();

        if (!string.IsNullOrEmpty(completedLevels))
        {
            foreach (var lvlStr in completedLevels.Split(','))
            {
                if (int.TryParse(lvlStr, out int lvl)) completedSet.Add(lvl);
            }
        }

        if (!completedSet.Contains(levelIndex))
        {
            completedSet.Add(levelIndex);
            string saveString = string.Join(",", completedSet.OrderBy(x => x));
            PlayerPrefs.SetString(key, saveString);
            PlayerPrefs.Save();
        }
    }

    public HashSet<int> LoadCompletedLevels(GameMode mode)
    {
        string key = (mode == GameMode.Normal) ? KEY_NORMAL_COMPLETED : KEY_HARD_COMPLETED;

        string completedLevels = PlayerPrefs.GetString(key, "");
        HashSet<int> completedSet = new HashSet<int>();

        if (!string.IsNullOrEmpty(completedLevels))
        {
            foreach (var lvlStr in completedLevels.Split(','))
            {
                if (int.TryParse(lvlStr, out int lvl)) completedSet.Add(lvl);
            }
        }

        return completedSet;
    }
}