using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public enum GameMode { Normal, Hard }
    public enum GameState { MainMenu, Gameplay, Pause }

    //* -------------------- FIELDS & PROPERTIES --------------------

    [Header("UI Screens")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject hudUI;
    [SerializeField] private GameObject blurPanel;

    [Header("UI - Backgrounds")]
    [SerializeField] private GameObject mainBackground;
    [SerializeField] private GameObject gameplayBackground;

    [Header("UI - Menu Buttons")]
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject resumeGameButton;
    [SerializeField] private GameObject backMainButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private TMP_Dropdown selectModeDropdown;

    [Header("UI - Level Buttons")]
    private const int TOTAL_NORMAL_LEVEL = 20;
    private const int TOTAL_HARD_LEVEL = 10;
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private ScrollRect levelScrollRect;
    [SerializeField] private RectTransform levelContent;
    [SerializeField] private Sprite completedLevelImage;
    private int totalLevels;

    [Header("UI - Audio Buttons")]
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Color unmuteColor = Color.green;
    [SerializeField] private Color muteColor = Color.red;

    [Header("Managers & State")]
    private GameplaySetup gameplaySetup;
    private GameMode currentMode = GameMode.Normal;
    private bool hasStarted = false;

    public GameMode GetCurrentMode() => currentMode;
    public bool HasStarted() => hasStarted;

    //* -------------------- UNITY LIFECYCLE --------------------

    private void Awake()
    {
        gameplaySetup = GetComponent<GameplaySetup>();
        Application.targetFrameRate = 144;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        SoundManager.Instance.PlayMainSound();

        currentMode = (GameMode)SaveManager.Instance.LoadCurrentMode();
        int dropdownIndex = (currentMode == GameMode.Normal) ? 0 : 1;
        selectModeDropdown.value = dropdownIndex;

        ChangeState(GameState.MainMenu);
    }

    //* -------------------- UI STATE MANAGEMENT --------------------

    private void ChangeState(GameState newState)
    {
        menuScreen.SetActive(false);
        hudUI.SetActive(false);
        blurPanel.SetActive(false);

        switch (newState)
        {
            case GameState.MainMenu:
                hasStarted = false;
                menuScreen.SetActive(true);
                startGameButton.SetActive(true);
                resumeGameButton.SetActive(false);
                backMainButton.SetActive(false);
                mainBackground.SetActive(true);
                gameplayBackground.SetActive(false);
                break;

            case GameState.Gameplay:
                hasStarted = true;
                hudUI.SetActive(true);
                pauseButton.SetActive(true);
                gameplayBackground.SetActive(true);
                break;

            case GameState.Pause:
                menuScreen.SetActive(true);
                startGameButton.SetActive(false);
                resumeGameButton.SetActive(true);
                backMainButton.SetActive(true);
                blurPanel.SetActive(true);
                break;
        }

        UpdateBGMButtonColor();
        UpdateSFXButtonColor();
    }

    //* -------------------- BUTTON EVENTS --------------------

    public void OnClickStartGame()
    {
        PlayUISound(true);
        GenerateLevelList();
    }

    public void OnClickLoadLevel(int levelIndex)
    {
        PlayUISound(true);
        SoundManager.Instance.PlayGameplaySound();
        gameplaySetup.InitGame(levelIndex);
        ChangeState(GameState.Gameplay);
        levelScreen.SetActive(false);
    }

    public void OnClickSelectMode(int index)
    {
        SaveManager.Instance.SaveCurrentMode((SaveManager.GameMode)index);
        PlayUISound(true);
        currentMode = (GameMode)index;
    }

    public void OnClickBackMain()
    {
        PlayUISound(false);
        gameplaySetup.UnloadCurrentLevel();
        SoundManager.Instance.PlayMainSound();
        ChangeState(GameState.MainMenu);
    }

    public void OnClickGoBack()
    {
        PlayUISound(false);
        levelScreen.SetActive(false);
    }

    public void OnClickPause()
    {
        PlayUISound(false);
        ChangeState(GameState.Pause);
    }

    public void OnClickResume()
    {
        PlayUISound(true);
        ChangeState(GameState.Gameplay);
    }

    public void OnClickMuteBGM()
    {
        SoundManager.Instance.ToggleBGMMute();
        UpdateBGMButtonColor();
    }

    public void OnClickMuteSFX()
    {
        SoundManager.Instance.ToggleSFXMute();
        UpdateSFXButtonColor();
    }

    public void OnClickQuit()
    {
        PlayUISound(false);
        Application.Quit();
    }

    //* -------------------- HELPERS --------------------

    private void GenerateLevelList()
    {
        levelScreen.SetActive(true);

        totalLevels = (currentMode == GameMode.Normal) ? TOTAL_NORMAL_LEVEL : TOTAL_HARD_LEVEL;

        var mode = GetCurrentMode();
        var completedLevels = SaveManager.Instance.LoadCompletedLevels((SaveManager.GameMode)mode);

        foreach (Transform child in levelContent) Destroy(child.gameObject);

        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject lvlBtn = Instantiate(levelButtonPrefab, levelContent);

            TMP_Text tmpText = lvlBtn.GetComponentInChildren<TMP_Text>();

            if (tmpText != null) tmpText.text = i.ToString();

            int realIndex = (currentMode == GameMode.Hard) ? i + TOTAL_NORMAL_LEVEL : i;

            Button btn = lvlBtn.GetComponent<Button>();
            var image = lvlBtn.GetComponent<Image>();

            bool isUnlocked = (i == 1) || completedLevels.Contains(realIndex - 1);
            bool isCompleted = completedLevels.Contains(realIndex);

            if (isUnlocked)
            {
                btn.interactable = true;
                btn.onClick.AddListener(() => OnClickLoadLevel(realIndex));
                if (image != null && isCompleted) image.sprite = completedLevelImage;
            }
            else
            {
                btn.interactable = false;
                if (image != null) image.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }

        Canvas.ForceUpdateCanvases();
        levelScrollRect.verticalNormalizedPosition = 1f;
    }

    private void PlayUISound(bool isEnter)
    {
        if (isEnter)
            SoundManager.Instance.PlayEnterButtonSound();
        else
            SoundManager.Instance.PlayExitButtonSound();
    }

    private void UpdateBGMButtonColor() => bgmButton.image.color = SoundManager.Instance.IsBGMMuted() ? muteColor : unmuteColor;
    private void UpdateSFXButtonColor() => sfxButton.image.color = SoundManager.Instance.IsSFXMuted() ? muteColor : unmuteColor;
}