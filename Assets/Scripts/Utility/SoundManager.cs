using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip ss;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
