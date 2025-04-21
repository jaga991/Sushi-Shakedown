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
    private CustomerData localCustomerData;
    public GameObject GameUI;
    public GameObject PauseUI;
    [SerializeField] private ToggleSwitch gameModeToggle;
    [SerializeField] private ToggleSwitch DifficultyModeToggle;

    public GameObject InfoUI;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI totalServedText;
    public TextMeshProUGUI happyText;

    public event System.Action OnInfoClosed;


    public GameObject PreDayUI;

    public TextMeshProUGUI preDayText;
    public TextMeshProUGUI preDayScoreText;

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
        GameUI.SetActive(false);
        PauseUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game

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
        Destroy(localCustomerData);
        localCustomerData = null;

        Time.timeScale = 1f; // Resume the game speed
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
        InfoUI.SetActive(false);
        PreDayUI.SetActive(false);
    }

    public void ShowInfoUI(int day, int score, int totalServed, int happy, int angry)
    {
        // Pause the game
        Debug.Log($"[Settings]  ShowInfoUI called with day: {day}, score: {score}, totalServed: {totalServed}, happy: {happy}, angry: {angry}");
        dayText.text = $"Day: {day} Completed !";
        scoreText.text = $"Coins Earned {score}";
        totalServedText.text = $"Customers Served: {totalServed}";
        happyText.text = $"Happy Customers: {happy}";
        GameUI.SetActive(false);
        PauseUI.SetActive(false);
        PreDayUI.SetActive(false);
        InfoUI.SetActive(true);
    }

    public void CloseInfoUI()
    {

        Debug.Log("[Settings]  CloseInfoUI called");
        DefaultView();
        OnInfoClosed?.Invoke();
    }
    public void ShowPreDayUI(int Day)
    {
        Debug.Log("[Settings]  ShowPreDayUI called");
        preDayText.text = $"Day {Day} Starting";
        preDayScoreText.text = $"Make  {customerData.GetRansom(Day)} COINS OR ELSE !";
        GameUI.SetActive(false);
        PauseUI.SetActive(false);
        InfoUI.SetActive(false);
        PreDayUI.SetActive(true);
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