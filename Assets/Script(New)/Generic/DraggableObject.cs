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
    private Camera cam;
    private SpriteRenderer sourceRenderer;
    
    private void Awake()
    {
        cam = Camera.main;

        sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // public abstract DraggedData GetData();

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canDrag)
        {
            Vector3 worldPos =
                cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            draggedObject = Instantiate(dragPrefab, worldPos, Quaternion.identity);

            // SpriteRenderer sourceRenderer = GetComponent<SpriteRenderer>();
            // SpriteRenderer dragRenderer = draggedObject.GetComponent<SpriteRenderer>();
            SpriteRenderer dragRenderer = draggedObject.GetComponentInChildren<SpriteRenderer>();

            // dragRenderer.sprite = sourceRenderer.sprite;
            dragRenderer.color = sourceRenderer.color;

            DragManager.BeginDrag(dragType);
            Debug.Log("Pointer Down");
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
            if (draggedObject == null)
                return;

            Vector3 worldPos =
                cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            draggedObject.transform.position = worldPos;
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
            if (draggedObject == null)
                return;

            Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null)
            {
                checkInterface(hit);
            }

            Destroy(draggedObject);
            draggedObject = null;
            DragManager.EndDrag(); 
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
