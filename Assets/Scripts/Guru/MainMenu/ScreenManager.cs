using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Header("Screen Panels")]
    [Tooltip("Main Menu Panel GameObject")]
    public GameObject settingsPanel;

    public MainMenuManager mainMenuManager;
    public void ShowSettings()
    {
        Debug.Log("Settings Panel Opened");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OnEnable()
    {
        mainMenuManager.OnSettingsOpened += ShowSettings;
    }

    public void OnDisable()
    {
        mainMenuManager.OnSettingsOpened -= ShowSettings;
    }

    public void Accept()
    {
        // Logic to accept the settings
        Debug.Log("Settings Accepted");
    }
    public void Decline()
    {
        // Logic to decline the settings
        Debug.Log("Settings Declined");
    }

}
