using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    //* -------------------- UNITY LIFECYCLE --------------------

    private const string KEY_CURRENT_MODE = "CurrentMode";
    private const string KEY_NORMAL_COMPLETE = "NORMAL_LevelsCompleted";
    private const string KEY_HARD_COMPLETE = "Hard_LevelsCompleted";

    public enum GameMode
    {

    }
}