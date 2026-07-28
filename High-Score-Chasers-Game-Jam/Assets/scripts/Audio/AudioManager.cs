using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip[] bgmPlayListClips;
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
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        StartCoroutine(PlayBGMPlayList());
    }

    public IEnumerator PlayBGMPlayList()
    {
        int playListLength = bgmPlayListClips.Length;
        for (int i = 0; ; i = (i + 1) % playListLength)
        {
            audioSource.clip = bgmPlayListClips[i];
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }



    public void PlayAudio(AudioSource audioSource, AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);    
    }

}
