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
        if (Instance == null) Instance = this;

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

    //* -------------------- VOLUME CONTROL --------------------

    public float GetBGMVolume()
    {
        return backgroundAudioSource.volume;
    }

    public void SetBGMVolume(float value)
    {
        backgroundAudioSource.volume = value;
        PlayerPrefs.SetFloat("ThemeSongVolume", value);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return soundEffectAudioSource.volume;
    }

    public void SetSFXVolume(float value)
    {
        soundEffectAudioSource.volume = value;
        PlayerPrefs.SetFloat("SoundEffectVolume", value);
        PlayerPrefs.Save();
    }
}