using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public enum GameMode { Normal, Hard }

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject resumeGameButton;
    [SerializeField] private GameObject selectModeButton;
    [SerializeField] private GameObject backMainButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject mainImage;
    [SerializeField] private GameObject gameplayImage;
    [SerializeField] private GameObject blurPanel;
    [SerializeField] private GameObject hudUI;
    private LevelManager levelManager;
    private bool hasStarted;
    private GameMode currentMode = GameMode.Normal;

    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Color unmuteColor = Color.green;
    [SerializeField] private Color muteColor = Color.red;
    private bool isThemeMuted = false;
    private bool isSFXMuted = false;
    private float lastThemeVolume = 1f;
    private float lastSFXVolume = 1f;

    public bool HasStarted() => hasStarted;
    public GameMode GetCurrentMode() => currentMode;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    private void Start()
    {
        SoundManager.Instance.PlayMainSound();
        menuScreen.SetActive(true);
        mainImage.SetActive(true);
        hasStarted = false;
    }

    public void OnClickStartGame()
    {
        hasStarted = true;
        SoundManager.Instance.PlayGameplaySound();
        SoundManager.Instance.PlayEnterButtonSound();
        gameplayImage.SetActive(true);
        levelManager.InitGame();
        menuScreen.SetActive(false);
        pauseButton.SetActive(true);
        hudUI.SetActive(true);
    }

    public void OnClickSelectMode(int index)
    {
        SoundManager.Instance.PlayEnterButtonSound();
        currentMode = (GameMode)index;

        // switch (currentMode)
        // {
        //     case GameMode.Normal:
        //         break;

        //     case GameMode.Hard:
        //         break;
        // }
    }

    public void OnClickBackMain()
    {
        levelManager.UnloadCurrentLevel();
        SoundManager.Instance.PlayMainSound();
        SoundManager.Instance.PlayExitButtonSound();
        hasStarted = false;
        startGameButton.SetActive(true);
        resumeGameButton.SetActive(false);
        selectModeButton.SetActive(true);
        backMainButton.SetActive(false);
        blurPanel.SetActive(false);
        hudUI.SetActive(false);
        gameplayImage.SetActive(false);
        hasStarted = false;
    }

    public void OnClickPause()
    {
        SoundManager.Instance.PlayExitButtonSound();
        menuScreen.SetActive(true);
        startGameButton.SetActive(false);
        resumeGameButton.SetActive(true);
        selectModeButton.SetActive(false);
        backMainButton.SetActive(true);
        pauseButton.SetActive(false);
        blurPanel.SetActive(true);
    }

    public void OnClickResume()
    {
        SoundManager.Instance.PlayEnterButtonSound();
        menuScreen.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void OnClickMuteBGM()
    {
        if (!isThemeMuted)
        {
            lastThemeVolume = SoundManager.Instance.GetBGMVolume();
            SoundManager.Instance.SetBGMVolume(0f);
            isThemeMuted = true;
            bgmButton.image.color = muteColor;
        }
        else
        {
            SoundManager.Instance.SetBGMVolume(lastThemeVolume);
            isThemeMuted = false;
            bgmButton.image.color = unmuteColor;
        }
    }

    public void OnClickMuteSFX()
    {
        if (!isSFXMuted)
        {
            lastSFXVolume = SoundManager.Instance.GetSFXVolume();
            SoundManager.Instance.SetSFXVolume(0f);
            isSFXMuted = true;
            sfxButton.image.color = muteColor;
        }
        else
        {
            SoundManager.Instance.SetSFXVolume(lastSFXVolume);
            isSFXMuted = false;
            sfxButton.image.color = unmuteColor;
        }
    }

    public void OnClickQuit()
    {
        SoundManager.Instance.PlayExitButtonSound();
        Application.Quit();
    }
}