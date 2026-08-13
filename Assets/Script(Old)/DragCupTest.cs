using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCupTest : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    private bool dragging;
    private Vector3 startPosition;

    [SerializeField] private TMP_Text drinkText;

    private Collider2D cupCollider;
    private Camera cam;

    public string DrinkContents => drinkText.text;

    private void Awake()
    {
        cam = Camera.main;
        cupCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;

        // Disable so we can detect objects underneath while dragging
        cupCollider.enabled = false;

        Debug.Log("Started dragging cup");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragging)
            return;

        dragging = false;

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        // Re-enable collider
        cupCollider.enabled = true;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        foreach (Collider2D hit in hits)
        {
            Customer customer = hit.GetComponent<Customer>();

            if (customer != null)
            {
                // customer.ReceiveCup(this);
                Destroy(gameObject);
                return;
            }
        }

        // Didn't hit a customer, return to where we started
        transform.position = startPosition;
    }
}