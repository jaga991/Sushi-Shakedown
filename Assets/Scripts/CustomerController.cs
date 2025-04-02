using UnityEngine;
using UnityEngine.UI; // For Text component

public class CustomerController : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 targetPosition; // Will be assigned from OrderArea
    public GameObject textBubble;
    public GameObject OrderBubble;

    private Text bubbleText;

    public OrderArea assignedOrderArea; // Exposed if you still want to check later

    public void SetOrderArea(OrderArea area)
    {
        assignedOrderArea = area;
        targetPosition = area.GetCoordinates();
    }

    void Awake()
    {
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
            Debug.Log("OrderBubble is not active.");
        }
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            ArrivedAtCounter();
        }
    }

    void ArrivedAtCounter()
    {
        // Log arrival, optional bubble update
        if (assignedOrderArea != null)
        {
            Debug.Log($"Customer reached assigned OrderArea: {assignedOrderArea.gameObject.name}");
        }

        if (bubbleText != null)
        {
            bubbleText.text = "Order";
        }
        OrderBubble.SetActive(true); // Show the order bubble
        textBubble.SetActive(false); // Hide the text bubble
        enabled = false;
    }
}
