using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Kept for Text component on textBubble

public class CustomerController : DebuggableMonoBehaviour
{
    // === Movement ===
    public float speed = 3f;
    public Vector2 targetPosition; // Will be assigned from OrderArea
    private bool hasArrived = false;
    private bool isWalkingOffScreen = false;
    private Vector2 offScreenTarget;

    // === UI Elements ===
    public GameObject textBubble;
    public PatienceBar patienceBar;
    public GameObject OrderBubble;
    public OrderBubble orderBubble;
    private Text bubbleText;

    // === Customer Data ===
    public CustomerData customerData;
    public CustomerData CustomerData;

    // === Audio ===
    public AudioClip orderCompletedSound;
    public AudioClip orderFailedSound;
    public AudioSource audioSource;

    // === Sprite / Visuals ===
    private SpriteRenderer spriteRenderer;
    public Sprite happySprite;
    public Sprite frustratedSprite;
    public Sprite angrySprite;

    // === Patience System ===
    public int maxPatience = 100;
    public int patienceBoostOnCorrect = 10;
    private int currentPatience;
    private Coroutine progressRoutine;

    // === Order Assignment ===
    public OrderArea assignedOrderArea; // Exposed if you still want to check later

    // === Difficulty ===
    private readonly int[] DifficultyMultiplierArr = { 1, 2 };
    private int difficultyMultiplier = 1; // Default to Easy
    protected override void Awake()
    {
        base.Awake(); // Call the base class method to set up logging
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Log("NPCController: No SpriteRenderer found!");
        }
        if (textBubble != null)
        {
            bubbleText = textBubble.GetComponentInChildren<Text>();
            if (bubbleText != null)
            {
                bubbleText.text = "..."; // Display initial three dots.
            }
        }

        if (OrderBubble != null)
        {
            OrderBubble.SetActive(false); // Hide the order bubble initially
        }
        else
        {
            Log("OrderBubble is not active.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable(); // Call the base class method to set up logging
        customerData.OnDifficultyChanged += OnDifficultyChanged;
    }
    protected override void OnDisable()
    {
        base.OnDisable(); // Call the base class method to clean up logging
        customerData.OnDifficultyChanged -= OnDifficultyChanged;
    }
    protected override void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.CustomerControllerLogs;
    }
    private void OnDifficultyChanged(Difficulty difficulty)
    {
        // Update the speed based on the difficulty level
        Log($"CustomerController: Difficulty changed to {difficulty}");
        Log("New Difficulaty multiplier is " + DifficultyMultiplierArr[(int)difficulty]);
        difficultyMultiplier = DifficultyMultiplierArr[(int)difficulty];

    }


    public void SetOrderArea(OrderArea area)
    {
        assignedOrderArea = area;
        targetPosition = area.GetCoordinates();
    }


    void Update()
    {
        // Move towards the assigned order area if not arrived.
        if (!hasArrived && !isWalkingOffScreen)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                ArrivedAtCounter();
                hasArrived = true;
            }
        }
        else if (isWalkingOffScreen)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (offScreenTarget - currentPosition).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            if (Vector2.Distance(currentPosition, offScreenTarget) < 0.1f)
            {
                Log("Customer has walked off screen.");
                isWalkingOffScreen = false;
                Destroy(gameObject);
            }
        }
    }


    private IEnumerator PatienceCountdown()
    {
        float waitPerPoint = 0.1f / difficultyMultiplier;
        // Log($"CustomerController: Patience countdown started. Wait time per point: {waitPerPoint} seconds.");

        while (currentPatience > 0)
        {
            // update the UI
            patienceBar.SetHealth(currentPatience);

            // wait scaled by difficulty
            yield return new WaitForSeconds(waitPerPoint);

            // then lose one point
            currentPatience--;
        }

        // out of patience!
        OrderFailed(1);
    }


    void SetOffScreenTarget()
    {
        Camera cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        Vector3 targetViewportPos;

        // If the customer is in the left half, target a point off-screen to the left.
        if (viewportPos.x < 0.5f)
        {
            targetViewportPos = new Vector3(-0.1f, viewportPos.y, viewportPos.z);
        }
        // Otherwise, target a point off-screen to the right.
        else
        {
            targetViewportPos = new Vector3(1.1f, viewportPos.y, viewportPos.z);
        }

        // Convert the target viewport position back to world space.
        Vector3 worldTarget = cam.ViewportToWorldPoint(targetViewportPos);
        // Preserve current Z position.
        worldTarget.z = transform.position.z;
        offScreenTarget = worldTarget;
    }

    void ArrivedAtCounter()
    {
        textBubble.SetActive(false);
        OrderBubble.SetActive(true);

        // orderBubble.StartOrder(Random.Range(1, 4));
        orderBubble.StartOrder(1);
        currentPatience = maxPatience;

        // Start the fake progress count (0 to 100) over 10 seconds.\
        if (progressRoutine != null) StopCoroutine(progressRoutine);
        progressRoutine = StartCoroutine(PatienceCountdown());
    }

    public void OnCorrectDelivery()
    {
        Log("CustomerController: Correct delivery! , boosted patience.");

        currentPatience = Mathf.Min(maxPatience, currentPatience + patienceBoostOnCorrect);

        patienceBar.SetHealth(currentPatience);
    }

    public void OnWrongDelivery(string foodName)
    {
        Log($"CustomerController: Wrong delivery of {foodName}!");
        OrderFailed(2);
    }

    /// <summary>
    /// Called by OrderBubble when the player delivers the wrong item.
    /// Shows feedback, deducts a bit of patience, then reverts.
    /// </summary>

    public void OnAllOrdersFulfilled()
    {
        // 1) stop patience timer
        if (progressRoutine != null)
            StopCoroutine(progressRoutine);

        OrderCompleted();
    }
    void OrderFailed(int reason)
    {
        if (reason == 1)
        {
            // customer ran out of patience 
            CustomerData.DeductScore(1);
            Log("Times up! Order failed.");
        }
        else if (reason == 2)
        {
            CustomerData.DeductScore(5);
            CustomerData.DeductScore(5);
            Log("Customer Received Wrong Order !!");
            // on wrong delivery 
        }
        spriteRenderer.sprite = angrySprite;
        PlayOrderFailed();

        OrderBubble.SetActive(false);
        SetOffScreenTarget();
        isWalkingOffScreen = true;
        assignedOrderArea.UpdateState(false);
    }

    // Updated method to set the off-screen target based on viewport bounds.
    // debugging thing , remove this 

    void OrderCompleted()
    {
        // Determine customer mood based on patience level
        int patiencePercent = patienceBar.GetHealth();

        spriteRenderer.sprite = patiencePercent switch
        {
            > 75 => happySprite,
            < 35 => frustratedSprite,
            _ => spriteRenderer.sprite
        };

        PlayOrderSuccess(patiencePercent);

        CustomerData.Increment();

        Log("Order Completed!");
        // Calculate score based on patience percentage (1-10)
        int score = Mathf.Clamp(1 + Mathf.FloorToInt(patiencePercent * 9f / 100f), 1, 10);

        CustomerData.AddScore(score);
        // Log("Added Score is " + score);

        CoinHandler.Instance.SpawnCoins(score, transform.position);


        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
            progressRoutine = null;
        }

        OrderBubble.SetActive(false);

        SetOffScreenTarget();

        isWalkingOffScreen = true;



        if (assignedOrderArea != null)
        {
            assignedOrderArea.UpdateState(false);
        }
    }

    // helper functions
    public void PlayOrderSuccess(int patiencePercent)
    {
        if (orderCompletedSound != null)
        {
            audioSource.PlayOneShot(orderCompletedSound);
        }
    }

    public void PlayOrderFailed()
    {
        if (orderFailedSound != null)
        {
            audioSource.PlayOneShot(orderFailedSound);
        }
    }



    // debugging thing , remove this
    void OnMouseDown()
    {
        if (hasArrived)
        {
            OrderCompleted();
        }
    }

}
