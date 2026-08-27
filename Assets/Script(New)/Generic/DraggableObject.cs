using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class DraggableObject : 
    MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    // [SerializeField] private object draggableObject;
    public DragEnum dragType;

    public bool canDrag;
    
    private GameObject draggedObject;

    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Sprite dragPrefabSprite;
    public Camera cam;
    [SerializeField] private bool hasDragIcon;
    private Collider2D objectCollider;
    private Vector3 startPosition;
    private Vector3 dragOffset;

    [SerializeField] private UIPanel panel;
    // [SerializeField] private UIPanel overlay;

    [SerializeField] private float dragHoldTime = 0.2f;

    private float holdTimer;
    private bool isHolding;
    private bool isDragging;

    private Vector2 pointerDownPosition;

    // Just for objects that drag themselves not icons
    [SerializeField] private string normalSortingLayer = "CounterObjects";
    [SerializeField] private string dragSortingLayer = "DragObjects";

    [SerializeField] private List<SpriteRenderer> objectSprites;

    
    private void Awake()
    {
        cam = Camera.main;

        objectCollider = GetComponent<Collider2D>();

        startPosition = transform.position;
    }

    public abstract object GetData();

    public abstract void AfterDropFunctions();

    public void OnPointerDown(PointerEventData eventData)
    {
        
        holdTimer = 0f;
        isHolding = true;
        isDragging = false;

        pointerDownPosition = eventData.position;
    }

    void Update()
    {
        if (!isHolding || isDragging)
            return;

        if (canDrag)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= dragHoldTime)
            {
                StartDragging();
            }
        }
    }

    void StartDragging()
    {
        isDragging = true;

        if (objectSprites.Count > 0)
        {
            foreach (SpriteRenderer spriteRenderer in objectSprites)
            {
                spriteRenderer.sortingLayerName = dragSortingLayer;
            }
        }

        Vector3 worldPos = cam.ScreenToWorldPoint(pointerDownPosition);
        worldPos.z = 0;

        if (hasDragIcon)
        {
            draggedObject = Instantiate(dragPrefab, worldPos, Quaternion.identity);

            SpriteRenderer dragRenderer = draggedObject.GetComponentInChildren<SpriteRenderer>();

            dragRenderer.sprite = dragPrefabSprite;
            // dragRenderer.sortingOrder = 100;

            Debug.Log("Dragging icon");
        }
        else
        {
            dragOffset = transform.position - worldPos;      

            objectCollider.enabled = false;

            Debug.Log("Dragging object");
        }
        DragManager.BeginDrag(dragType);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        if (hasDragIcon)
        {
            if (draggedObject == null)
                return;

            draggedObject.transform.position = worldPos;
        }
        else
        {
            transform.position = worldPos + dragOffset;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        if (!isDragging)
        {
            if (panel != null)
            {
                panel.OpenPanel();
                // overlay.SetActive(!overlay.activeSelf);
            }

            return;
        }

        Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null)
        {
            DropInterface dropTarget =
                hit.GetComponent<DropInterface>();

            if (dropTarget != null && hit.gameObject != gameObject)
            {
                if (dropTarget.ReceiveDraggable(GetData()))
                {
                    AfterDropFunctions();
                }
            }
        }

        if (hasDragIcon)
        {
            if (draggedObject == null)
                return;

            Destroy(draggedObject);
            draggedObject = null;
        }
        else
        {
            objectCollider.enabled = true;
            transform.position = startPosition;

            if (objectSprites.Count > 0)
            {
                foreach (SpriteRenderer spriteRenderer in objectSprites)
                {
                    spriteRenderer.sortingLayerName = normalSortingLayer;
                }
            }
        }
        isDragging = false;
        DragManager.EndDrag(); 

    }


}
