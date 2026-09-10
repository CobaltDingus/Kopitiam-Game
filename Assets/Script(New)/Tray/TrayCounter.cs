using System.Collections.Generic;
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
    private Vector3 dragOffset;

    private Collider2D trayCollider;
    private Camera cam;
    [SerializeField] private Sprite[] containerSprites;
    [SerializeField] private SpriteRenderer[] cupRenderers;
    [SerializeField] private SpriteRenderer[] drinkRenderers;
    [SerializeField] private SpriteRenderer traySprite;

    [SerializeField] private string normalSortingLayer = "CounterObjects";
    [SerializeField] private string dragSortingLayer = "DragObjects";

    [SerializeField] private List<SpriteRenderer> objectSprites;

    void Update()
    {
        for (int i = 0; i < cupRenderers.Length; i++)
        {
            if (i < trayDatabase.SavedDrinks.Count)
            {
                if (trayDatabase.SavedDrinks[i].containerType == ContainerType.Hot)
                {
                    cupRenderers[i].sprite = containerSprites[0];
                }

                drinkRenderers[i].color = HexToColor(trayDatabase.SavedDrinks[i].colorHex);
                drinkRenderers[i].enabled = true;
            }
            else
            {
                cupRenderers[i].sprite = null;
                drinkRenderers[i].color = Color.clear;
                drinkRenderers[i].enabled = false;
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
        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        dragging = true;

        trayCollider.enabled = false;

        dragOffset = transform.position - worldPos;

        if (objectSprites.Count > 0)
        {
            foreach (SpriteRenderer spriteRenderer in objectSprites)
            {
                spriteRenderer.sortingLayerName = dragSortingLayer;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging)
            return;

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        transform.position = worldPos + dragOffset;
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
            ReworkedCustomerDisplayLink customer = hit.GetComponentInParent<ReworkedCustomerDisplayLink>();

            if (customer != null)
            {
                // ONLY allow serving when explicitly in ServingOrder state
                if (ReworkedCustomerManager.instance.CurrentCounterState != ReworkedCustomerManager.CounterState.ServingOrder)
                {
                    transform.position = startPosition;
                    if (objectSprites.Count > 0)
                    {
                        foreach (SpriteRenderer spriteRenderer in objectSprites)
                        {
                            spriteRenderer.sortingLayerName = normalSortingLayer;
                        }
                    }
                    return;
                }

                // Process the order (this handles evaluation and showing the Next button)
                ReworkedCustomerManager.instance.ProcessOrder();

                // Reset tray position
                transform.position = startPosition;
                if (objectSprites.Count > 0)
                {
                    foreach (SpriteRenderer spriteRenderer in objectSprites)
                    {
                        spriteRenderer.sortingLayerName = normalSortingLayer;
                    }
                }
                return;
            }
        }

        // Didn't hit a customer, return to original position
        transform.position = startPosition;

        if (objectSprites.Count > 0)
        {
            foreach (SpriteRenderer spriteRenderer in objectSprites)
            {
                spriteRenderer.sortingLayerName = normalSortingLayer;
            }
        }
    }

    public Color HexToColor(string hexCode)
    {
        if (ColorUtility.TryParseHtmlString(hexCode, out Color newColor))
        {
            return newColor;
        }
        else
        {
            Debug.LogWarning("Invalid Hexadecimal string provided!");
            return Color.clear;
        }
    }
}