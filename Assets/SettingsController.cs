using UnityEngine;
using UnityEngine.UI;         // For Button, Toggle, etc.

public class SettingsController : MonoBehaviour
{
    [Header("Screen Panels")]
    [Tooltip("Settings Panel GameObject")]
    public GameObject settingsPanel;
    [Tooltip("Main Menu Panel GameObject")]
    public GameObject mainMenuPanel;
    public MainMenuManager mainMenuManager;

    [Header("Game Mode Data")]
    public CustomerData customerData;  // Global data
    [SerializeField] private ToggleSwitch gameModeToggle;

    // Local copy of the CustomerData that is used to modify settings locally.
    private CustomerData localCustomerData;

    private void Start()
    {
        // For initial setup, create the local copy and refresh the UI.
        CreateLocalCopy();
        RefreshUI();
    }

    private void OnEnable()
    {
        mainMenuManager.OnSettingsOpened += ShowSettings;
    }

    private void OnDisable()
    {
        mainMenuManager.OnSettingsOpened -= ShowSettings;
    }

    /// <summary>
    /// Creates a fresh local copy of the global CustomerData.
    /// </summary>
    private void CreateLocalCopy()
    {
        // Destroy any previous local copy
        if (localCustomerData != null)
        {
            Destroy(localCustomerData);
        }
        localCustomerData = Instantiate(customerData);
    }

    /// <summary>
    /// Refreshes all UI elements based on the local copy.
    /// </summary>
    private void RefreshUI()
    {
        bool isWaves = localCustomerData.gameMode == GameMode.Waves;
        // Update the toggle state using a helper method from your ToggleSwitch
        gameModeToggle.SetStateSilently(isWaves);
        // If you have more UI elements to refresh, do it here.
    }

    /// <summary>
    /// Called to show the settings panel.
    /// Creates a fresh local copy and refreshes the UI.
    /// </summary>
    public void ShowSettings()
    {
        CreateLocalCopy();
        RefreshUI();
        settingsPanel?.SetActive(true);
        mainMenuPanel?.SetActive(false);
    }

    public void HideMainMenu()
    {
        mainMenuPanel?.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
    }

    /// <summary>
    /// Called by the UI "Apply" button.
    /// Commits all changes from the local copy back to the global CustomerData.
    /// </summary>
    public void Accept()
    {
        // Copy selected settings (here, just gameMode) from local to global.
        customerData.SetGameMode(localCustomerData.gameMode);
        Debug.Log("Settings Accepted. Global settings updated.");

        CloseSettings();
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    /// <summary>
    /// Called by the UI "Decline" button.
    /// Discards any changes from the local copy.
    /// </summary>
    public void Decline()
    {
        Debug.Log("Settings Declined. Changes discarded.");
        CloseSettings();
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    public void CloseSettings()
    {
        settingsPanel?.SetActive(false);
        mainMenuPanel?.SetActive(true);
    }

    /// <summary>
    /// UI callback for toggling game mode.
    /// Updates the local copy instead of the global object.
    /// </summary>
    public void ToggleModeButton(int value)
    {
        localCustomerData.SetGameMode((GameMode)value);
        Debug.Log($"Local game mode changed to: {localCustomerData.gameMode}");
        RefreshUI();
    }

    /// <summary>
    /// Refreshes the UI. This can be called externally as needed.
    /// </summary>
    public void Refresh()
    {
        RefreshUI();
    }
}
