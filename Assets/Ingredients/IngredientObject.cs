using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientObject : 
    MonoBehaviour,
    // DraggableObject,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    // [SerializeField] private GameObject dragPrefab;
    public Ingredient ingredientAsset;

    public DragEnum dragType;

    public bool canDrag;
    
    private GameObject draggedObject;

    [SerializeField] private GameObject dragPrefab;
    [SerializeField] private Sprite dragPrefabSprite;
    private Camera cam;
    private SpriteRenderer sourceRenderer;

    // Progression stage stuff
    public int unlockDay = 0;
    
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

            dragRenderer.sprite = dragPrefabSprite;
            dragRenderer.sortingOrder = 100;
            // dragRenderer.color = sourceRenderer.color;

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
                DropIngredientInterface dropTarget = hit.GetComponent<DropIngredientInterface>();

                if (dropTarget != null)
                {
                    dropTarget.ReceiveIngredient(ingredientAsset);
                }      
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
}