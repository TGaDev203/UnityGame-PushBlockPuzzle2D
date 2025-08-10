using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip onPointSound;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioClip undoButtonSound;

    public void PlayWalkSound() => PlayOnShotSound(walkSound);
    public void PlayOnPointSound() => PlayOnShotSound(onPointSound);
    public void PlayLevelCompleteSound() => PlayOnShotSound(levelCompleteSound);
    public void PlayUndoButtonSound() => PlayOnShotSound(undoButtonSound);

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void PlayOnShotSound(AudioClip clip)
    {
        if (clip == null && soundEffectAudioSource == null) return;

        soundEffectAudioSource.clip = clip;
        soundEffectAudioSource.PlayOneShot(clip);
    }
}