using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    //* -------------------- FIELDS & PROPERTIES --------------------

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip mainSound;
    [SerializeField] private AudioClip gameplaySound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip onPointSound;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioClip undoButtonSound;
    [SerializeField] private AudioClip enterButtonSound;
    [SerializeField] private AudioClip exitButtonSound;

    [Header("Keys")]
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string BGM_MUTED_KEY = "BGMMuted";
    private const string SFX_MUTED_KEY = "SFXMuted";

    [Header("Flags")]
    private bool isBGMMuted = false;
    private bool isSFXMuted = false;

    //* -------------------- PLAY METHODS --------------------

    public void PlayMainSound() => PlayBGMSound(mainSound);
    public void PlayGameplaySound() => PlayBGMSound(gameplaySound);
    public void PlayWalkSound() => PlaySFXSound(walkSound);
    public void PlayOnPointSound() => PlaySFXSound(onPointSound);
    public void PlayLevelCompleteSound() => PlaySFXSound(levelCompleteSound);
    public void PlayUndoButtonSound() => PlaySFXSound(undoButtonSound);
    public void PlayEnterButtonSound() => PlaySFXSound(enterButtonSound);
    public void PlayExitButtonSound() => PlaySFXSound(exitButtonSound);

    //* -------------------- UNITY LIFECYCLE --------------------

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            bool bgmMuted = PlayerPrefs.GetInt(BGM_MUTED_KEY, 0) == 1;
            bool sfxMuted = PlayerPrefs.GetInt(SFX_MUTED_KEY, 0) == 1;

            isBGMMuted = bgmMuted;
            isSFXMuted = sfxMuted;

            float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
            float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

            backgroundAudioSource.volume = bgmMuted ? 0f : bgmVol;
            soundEffectAudioSource.volume = sfxMuted ? 0f : sfxVol;
        }

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    //* -------------------- PLAY HELPERS --------------------

    private void PlaySFXSound(AudioClip clip)
    {
        if (clip == null || soundEffectAudioSource == null) return;

        soundEffectAudioSource.clip = clip;
        soundEffectAudioSource.PlayOneShot(clip);
    }

    private void PlayBGMSound(AudioClip clip)
    {
        if (backgroundAudioSource == null) return;

        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = clip;
        backgroundAudioSource.Play();
    }

    //* -------------------- MUTE TOGGLES --------------------

    public void ToggleBGMMute()
    {
        bool currentlyMuted = backgroundAudioSource.volume <= 0f;

        if (currentlyMuted)
        {
            backgroundAudioSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
            PlayerPrefs.SetInt(BGM_MUTED_KEY, 0);
            isBGMMuted = false;
        }

        else
        {
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, backgroundAudioSource.volume);
            backgroundAudioSource.volume = 0f;
            PlayerPrefs.SetInt(BGM_MUTED_KEY, 1);
            isBGMMuted = true;
        }

        PlayerPrefs.Save();
    }

    public void ToggleSFXMute()
    {
        bool currentlyMuted = soundEffectAudioSource.volume <= 0f;

        if (currentlyMuted)
        {
            soundEffectAudioSource.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            PlayerPrefs.SetInt(SFX_MUTED_KEY, 0);
            isSFXMuted = false;
        }

        else
        {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, soundEffectAudioSource.volume);
            soundEffectAudioSource.volume = 0f;
            PlayerPrefs.SetInt(SFX_MUTED_KEY, 1);
            isSFXMuted = true;
        }

        PlayerPrefs.Save();
    }

    //* -------------------- VOLUME GETTERS/SETTERS --------------------

    public void SetBGMVolume(float value)
    {
        backgroundAudioSource.volume = value;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.SetInt(BGM_MUTED_KEY, value <= 0f ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        soundEffectAudioSource.volume = value;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.SetInt(SFX_MUTED_KEY, value <= 0 ? 1 : 0);
        PlayerPrefs.Save();
    }

    public float GetBGMVolume() => backgroundAudioSource.volume;
    public float GetSFXVolume() => soundEffectAudioSource.volume;
    public bool IsBGMMuted() => isBGMMuted;
    public bool IsSFXMuted() => isSFXMuted;
}