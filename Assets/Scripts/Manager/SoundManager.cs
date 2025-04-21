using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    [SerializeField] private AudioMixer audioMixer;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (EventManager.Instance != null)
        {

        }
    }


    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        if (audioClipArray.Length > 0)
        {
            AudioClip clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
            PlaySound(clip, position, volume);
        }
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        if (audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, position, volume);
        }
    }
}

