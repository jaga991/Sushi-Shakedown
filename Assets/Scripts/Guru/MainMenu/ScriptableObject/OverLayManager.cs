using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;
using UnityEngine.Playables;

public class OverLayManager : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject PauseUI;

    public CustomerData customerData;
    private CustomerData localCustomerData;
    [SerializeField] private ToggleSwitch gameModeToggle;
    [SerializeField] private ToggleSwitch DifficultyModeToggle;


    private LogSettings logSettings;
    private bool isDebugEnabled = false;

    // Update is called once per frame
    void Update()
    {
    }


    public void Awake()
    {
        if (!logSettings)
        {
            logSettings = Resources.Load<LogSettings>("Guru/DataStore/LogSettings");
        }
        else
        {
            Debug.Log("LogSettings not found. Please assign it in the inspector.");
        }
    }

    void Start()
    {
        // Initialize the UI to show the game screen by default
        DefaultView();
        CreateLocalCopy();
    }

    public void OnEnable()
    {
        logSettings.OnSettingsChanged += UpdateLogStatus;
        // customerData.OnGameModeChanged += HandleModeChanged;
        UpdateLogStatus();
    }

    public void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.OverLayManagerLogs;
    }

    public void OnDisable()
    {
        logSettings.OnSettingsChanged -= UpdateLogStatus;
        // customerData.OnGameModeChanged -= HandleModeChanged;
    }

    public void PauseButtonClick()
    {
        CreateLocalCopy();
        RefreshUI();
        GameUI.SetActive(false);
        PauseUI.SetActive(true);

    }
    private void CreateLocalCopy()
    {
        // Destroy any previous local copy
        if (localCustomerData != null)
        {
            Destroy(localCustomerData);
        }
        localCustomerData = Instantiate(customerData);
        Log($"Local copy created: {localCustomerData.gameMode}");

    }
    private void RefreshUI()
    {
        bool isWaves = localCustomerData.gameMode == GameMode.Waves;

        gameModeToggle.CurrentValue = isWaves;

        bool isEasy = localCustomerData.difficulty == Difficulty.Easy;
        DifficultyModeToggle.CurrentValue = isEasy;
    }

    public void Accept()
    {
        // Copy selected settings (here, just gameMode) from local to global.
        customerData.SetGameMode(localCustomerData.gameMode);
        customerData.SetDifficulty(localCustomerData.difficulty);
        DefaultView();
        // CloseSettings();
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    /// <summary>
    /// Called by the UI "Decline" button.
    /// Discards any changes from the local copy.
    /// </summary>
    public void Decline()
    {
        Log("Settings Declined. Changes discarded.");
        DefaultView();
        // Resume game speed when returning to game
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    public void ToggleModeButton(int value)
    {
        localCustomerData.SetGameMode((GameMode)value);
        RefreshUI();
    }

    public void ToggleDifficultyButton(int value)
    {
        Log("Tn called with value: " + (Difficulty)value);
        localCustomerData.SetDifficulty((Difficulty)value);
        // RefreshUI();s

    }

    void DefaultView()
    {
        GameUI.SetActive(true);
        PauseUI.SetActive(false);
    }


    private void Log(string message, [CallerMemberName] string caller = "")
    {
        if (isDebugEnabled)
            Debug.Log($"[OverlayManager::{caller}] {message}");
    }
}