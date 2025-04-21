using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class OverLayManager : DebuggableMonoBehaviour, IPointerClickHandler
{
    public CustomerData customerData;
    private CustomerData Pause_CustomerData_Local;

    // Screens
    public GameObject Game_Screen;
    public GameObject Pause_Screen;
    public GameObject Info_Screen;
    public GameObject PreDay_Screen;

    // UI Toggles (Settings)
    [SerializeField] private ToggleSwitch GameMode_Toggle;
    [SerializeField] private ToggleSwitch DifficultyMode_Toggle;

    // InfoUI elements
    public TextMeshProUGUI Info_DayText;
    public TextMeshProUGUI Info_ScoreText;
    public TextMeshProUGUI Info_TotalServedText;
    public TextMeshProUGUI Info_HappyText;
    public event System.Action OnInfoClosed;

    // PreDayUI elements
    public TextMeshProUGUI PreDay_DayText;
    public TextMeshProUGUI PreDay_ScoreText;
    public event System.Action OnPreDayClosed;
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // Initialize the UI to show the game screen by default
        DefaultView();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // customerData.OnGameModeChanged += HandleModeChanged;
    }

    protected override void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.OverLayManagerLogs;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // customerData.OnGameModeChanged -= HandleModeChanged;
    }

    public void PauseButtonClick()
    {
        CreateLocalCopy();
        RefreshUI();
        Game_Screen.SetActive(false);
        Pause_Screen.SetActive(true);
        Time.timeScale = 0f; // Pause the game

    }
    private void CreateLocalCopy()
    {
        // Destroy any previous local copy
        if (Pause_CustomerData_Local != null)
        {
            Destroy(Pause_CustomerData_Local);
        }
        Pause_CustomerData_Local = Instantiate(customerData);
        Log($"Local copy created: {Pause_CustomerData_Local.gameMode}");

    }
    private void RefreshUI()
    {
        bool isWaves = Pause_CustomerData_Local.gameMode == GameMode.Waves;

        GameMode_Toggle.CurrentValue = isWaves;

        bool isEasy = Pause_CustomerData_Local.difficulty == Difficulty.Easy;
        DifficultyMode_Toggle.CurrentValue = isEasy;
    }

    public void Accept()
    {
        // Copy selected settings (here, just gameMode) from local to global.
        customerData.SetGameMode(Pause_CustomerData_Local.gameMode);
        customerData.SetDifficulty(Pause_CustomerData_Local.difficulty);
        DefaultView();
        // CloseSettings();
        Destroy(Pause_CustomerData_Local);
        Pause_CustomerData_Local = null;
        Time.timeScale = 1f; // Resume the game speed
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
        Destroy(Pause_CustomerData_Local);
        Pause_CustomerData_Local = null;

        Time.timeScale = 1f; // Resume the game speed
    }

    public void ToggleModeButton(int value)
    {
        Pause_CustomerData_Local.SetGameMode((GameMode)value);
        RefreshUI();
    }

    public void ToggleDifficultyButton(int value)
    {
        Log("Tn called with value: " + (Difficulty)value);
        Pause_CustomerData_Local.SetDifficulty((Difficulty)value);
        // RefreshUI();s

    }

    private void DefaultView()
    {
        Game_Screen.SetActive(true);
        Pause_Screen.SetActive(false);
        Info_Screen.SetActive(false);
        PreDay_Screen.SetActive(false);
    }

    public void ShowInfoUI(int day, int score, int totalServed, int happy, int angry)
    {
        Debug.Log($"[Overlay] ShowInfoUI: day={day}, score={score}, served={totalServed}, happy={happy}, angry={angry}");
        Info_DayText.text = $"Day: {day} Completed!";
        Info_ScoreText.text = $"Coins Earned: {score}";
        Info_TotalServedText.text = $"Customers Served: {totalServed}";
        Info_HappyText.text = $"Happy Customers: {happy}";
        Game_Screen.SetActive(false);
        Pause_Screen.SetActive(false);
        PreDay_Screen.SetActive(false);
        Info_Screen.SetActive(true);
    }

    public void CloseInfoUI()
    {

        Debug.Log("[Settings]  CloseInfoUI called");
        DefaultView();
        OnInfoClosed?.Invoke();
    }
    public void ShowPreDayUI(int day)
    {
        Debug.Log($"[Overlay] ShowPreDayUI: starting day {day}");
        PreDay_DayText.text = $"Day {day} Starting";
        PreDay_ScoreText.text = $"Make {customerData.GetRansom(day)} COINS OR ELSE!";
        Game_Screen.SetActive(false);
        Pause_Screen.SetActive(false);
        Info_Screen.SetActive(false);
        PreDay_Screen.SetActive(true);
    }
    public void ClosePreDayUI()
    {
        Debug.Log("[Settings]  ClosePreDayUI called");
        DefaultView();
        OnPreDayClosed?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[Settings]  Settings UI received click");
    }
}