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
