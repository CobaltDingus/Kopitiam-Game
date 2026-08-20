using System;
using UnityEngine;
using UnityEngine.Events;
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
    private Camera cam;
    // private SpriteRenderer sourceRenderer;

    // private object dragData;
    [SerializeField] private bool hasDragIcon;
    private Collider2D objectCollider;
    private Vector3 startPosition;
    
    private void Awake()
    {
        cam = Camera.main;

        // sourceRenderer = GetComponentInChildren<SpriteRenderer>();

        objectCollider = GetComponent<Collider2D>();

        startPosition = transform.position;
    }

    public abstract object GetData();

    public abstract void AfterDropFunctions();

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (hasDragIcon)
            {
                Vector3 worldPos =
                    cam.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;

                draggedObject = Instantiate(dragPrefab, worldPos, Quaternion.identity);

                // SpriteRenderer sourceRenderer = GetComponent<SpriteRenderer>();
                // SpriteRenderer dragRenderer = draggedObject.GetComponent<SpriteRenderer>();
                SpriteRenderer dragRenderer = draggedObject.GetComponentInChildren<SpriteRenderer>();

                // dragRenderer.sprite = sourceRenderer.sprite;
                // dragRenderer.color = sourceRenderer.color;

                dragRenderer.sprite = dragPrefabSprite;
                dragRenderer.sortingOrder = 100;

                DragManager.BeginDrag(dragType);
                Debug.Log("Pointer Down");
            }
            else
            {
                DragManager.BeginDrag(dragType);

                objectCollider.enabled = false;

                Debug.Log("Started dragging");
            }
        }
        else
        {
            return;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (hasDragIcon)
            {
                if (draggedObject == null)
                    return;

                Vector3 worldPos =
                    cam.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;

                draggedObject.transform.position = worldPos;
            }
            else
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;
                transform.position = worldPos;
            }
        }
        else
        {
            return;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (hasDragIcon)
            {
                if (draggedObject == null)
                    return;

                Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;

                Collider2D hit = Physics2D.OverlapPoint(worldPos);

                if (hit != null)
                {
                    DropInterface dropTarget = hit.GetComponent<DropInterface>();

                    if (dropTarget != null)
                    {
                        dropTarget.ReceiveDraggable(GetData());
                        AfterDropFunctions();
                    }      
                }

                Destroy(draggedObject);
                draggedObject = null;
                DragManager.EndDrag(); 
            }
            else
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
                worldPos.z = 0;

                objectCollider.enabled = true;

                // Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

                // foreach (Collider2D hit in hits)
                // {

                // }

                Collider2D hit = Physics2D.OverlapPoint(worldPos);

                if (hit != null)
                {
                    DropInterface dropTarget = hit.GetComponent<DropInterface>();

                    if (dropTarget != null)
                    {
                        dropTarget.ReceiveDraggable(GetData());
                        // transform.position = startPosition;
                        // DragManager.EndDrag(); 
                        AfterDropFunctions();
                    }      
                }

                DragManager.EndDrag(); 
                transform.position = startPosition;
            }
        }
        else
        {
            return;
        }
    }

    public void checkInterface(Collider2D hit)
    {
                // DropInterface dropTarget = hit.GetComponent<DropInterface>();

                // if (dropTarget != null)
                // {
                //     dropTarget.ReceiveDraggable(GetData());
                // }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
