using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip bgmAudioClip;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayBGMAudio(bgmAudioClip);
    }

    public void PlayBGMAudio(AudioClip bgmAudioClip)
    {
        audioSource.clip = bgmAudioClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
