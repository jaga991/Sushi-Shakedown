using UnityEngine;
using UnityEngine.UI;             // for Button
using UnityEngine.SceneManagement; // for loading scenes
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("Reference to an AudioSource in the scene")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip buttonClickClip;
    // we'll load these at runtime instead of via Inspector
    private AudioClip[] musicClips;

    public GuruAudioManager gm;


    public event System.Action OnSettingsOpened;

    void Awake()
    {
        // Load all clips in that Resources folder
        musicClips = Resources.LoadAll<AudioClip>(
            "Audio/Guru/MainMenu/BackgroundTracks"
        );

        if (musicClips == null || musicClips.Length == 0)
            Debug.LogWarning("MainMenuManager: No background tracks found in Resources!");
    }

    void Start()
    {
        gm = GameObject.Find("GuruAudioManager").GetComponent<GuruAudioManager>();

        Debug.Log("Starting Main Menu…");
        PlayRandomMusic();
    }

    private void PlayRandomMusic()
    {
        if (musicSource == null || musicClips == null || musicClips.Length == 0)
            return;

        // pick one clip at random
        int idx = Random.Range(0, musicClips.Length);
        AudioClip chosen = musicClips[idx];

        musicSource.clip = chosen;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log($"Now playing: {chosen.name}");

        // unload all other clips to free memory
        for (int i = 0; i < musicClips.Length; i++)
        {
            if (i == idx) continue;
            Resources.UnloadAsset(musicClips[i]);
        }

        // keep only the chosen clip in the array
        musicClips = new AudioClip[] { chosen };
    }

    public void OnPlayButtonClicked()
    {

        gm.PlayButtonClickSound();
        Debug.Log("Play button clicked! Loading Game scene...");
        EventSystem.current.SetSelectedGameObject(null);
        GameSceneManager.instance.StartGame();

    }

    public void OnSettingsButtonClicked()
    {
        gm.PlayButtonClickSound();
        Debug.Log("Settings button clicked! Opening Settings...");
        EventSystem.current.SetSelectedGameObject(null);
        OnSettingsOpened?.Invoke();
    }

    public void OnExitButtonClicked()
    {
        gm.PlayButtonClickSound();
        Debug.Log("Exit button clicked! Quitting...");
        EventSystem.current.SetSelectedGameObject(null);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
