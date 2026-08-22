using UnityEngine;
using UnityEngine.EventSystems;

public class TrayCounter : 
    MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public TrayDatabase trayDatabase;

    public GameObject slotOne;
    public GameObject slotTwo;
    public GameObject slotThree;

    private bool dragging;
    private Vector3 startPosition;

    // [SerializeField] private TMP_Text drinkText;

    private Collider2D trayCollider;
    private Camera cam;

    // public string DrinkContents => drinkText.text;

    void Update()
    {
        GameObject[] slots =
        {
            slotOne,
            slotTwo,
            slotThree
        };

        for (int i = 0; i < slots.Length; i++)
        {
            SpriteRenderer spriteRenderer = slots[i].GetComponentInChildren<SpriteRenderer>();

            if (i < trayDatabase.SavedDrinks.Count)
            {
                spriteRenderer.sprite = trayDatabase.SavedDrinks[i].drinkSprite;
            }
            else
            {
                spriteRenderer.sprite = null;
            }
        }
    }

    private void Awake()
    {
        cam = Camera.main;
        trayCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;

        trayCollider.enabled = false;

        Debug.Log("Started dragging tray");
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

        // Re-enable collider
        trayCollider.enabled = true;

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        foreach (Collider2D hit in hits)
        {
            // Searches the hit collider AND its parent objects for CustomerDisplayLink
            CustomerDisplayLink customer = hit.GetComponentInParent<CustomerDisplayLink>();

            if (customer != null)
            {
                if (CustomerManager.Instance.serveStatus)
                {
                    transform.position = startPosition;
                    return;
                }
                CustomerManager.Instance.CustomerServed();
                CustomerManager.Instance.ServeOrder();

                // Reset tray position
                transform.position = startPosition;
                return;
            }
        }

        // Didn't hit a customer, return to original position
        transform.position = startPosition;
    }
}