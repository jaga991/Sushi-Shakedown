// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.ResourceLocations;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using System.Collections.Generic;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// public class MainMenuManager : MonoBehaviour
// {
//     [Header("Music")]
//     [Tooltip("Reference to an AudioSource in the scene")]


//     // we’ll load these at runtime instead of via Inspector
//     private AudioClip[] musicClips;

//     [Header("UI Buttons")]
//     public Button playButton;
//     public Button settingsButton;
//     public Button exitButton;

//     public AudioSource musicSource;
//     private IList<IResourceLocation> _locations;

//     void Awake()
//     {
//         // Get the list of addresses (tiny metadata, not the clips themselves)
//         Addressables.LoadResourceLocationsAsync(
//             "Background",
//             Addressables.MergeMode.Union
//         ).Completed += OnLocationsLoaded;
//     }

//     private void OnLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> h)
//     {
//         if (h.Status == AsyncOperationStatus.Succeeded)
//             _locations = h.Result;
//         else
//             Debug.LogError("Failed to load music locations");
//     }

//     void Start()
//     {
//         PlayRandomMusic();
//     }
//     private void PlayRandomMusic()
//     {
//         Debug.Log("Loading random music...");
//         if (_locations == null || _locations.Count == 0 || musicSource == null)
//             return;
//         Debug.Log("Loading random music...__2");
//         // Pick one location at random
//         var loc = _locations[Random.Range(0, _locations.Count)];
//         // Asynchronously load that single clip
//         Addressables.LoadAssetAsync<AudioClip>(loc).Completed += handle =>
//         {
//             if (handle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 musicSource.clip = handle.Result;
//                 musicSource.loop = true;
//                 musicSource.Play();
//                 Debug.Log($"Now playing: {handle.Result.name}");
//             }
//             else
//             {
//                 Debug.LogError($"Failed to load clip at {loc.PrimaryKey}");
//             }
//         };
//     }


//     public void OnPlayButtonClicked()
//     {
//         Debug.Log("Play button clicked! Loading Game scene...");
//         EventSystem.current.SetSelectedGameObject(null);
//         // SceneManager.LoadScene("GameScene");
//     }

//     public void OnSettingsButtonClicked()
//     {
//         Debug.Log("Settings button clicked! Opening Settings...");
//         EventSystem.current.SetSelectedGameObject(null);
//         // settingsPanel.SetActive(true);
//     }

//     public void OnExitButtonClicked()
//     {
//         Debug.Log("Exit button clicked! Quitting...");
//         EventSystem.current.SetSelectedGameObject(null);

// #if UNITY_EDITOR
//         EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }
// }

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

    // we'll load these at runtime instead of via Inspector
    private AudioClip[] musicClips;


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
        Debug.Log("Play button clicked! Loading Game scene...");
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("Customers");
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked! Opening Settings...");
        EventSystem.current.SetSelectedGameObject(null);
        // settingsPanel.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked! Quitting...");
        EventSystem.current.SetSelectedGameObject(null);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
