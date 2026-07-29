using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientPrefabNew : 
    MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField] private GameObject dragPrefab;
    public Ingredient ingredientAsset;

    private GameObject draggedObject;
    private Camera cam;
    private SpriteRenderer sourceRenderer;

    private void Awake()
    {
        cam = Camera.main;

        sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnPointerDown(PointerEventData eventData)
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
        Debug.Log("Pointer Down");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject == null)
            return;

        Vector3 worldPos =
            cam.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        draggedObject.transform.position = worldPos;
    }

    public void OnPointerUp(PointerEventData eventData)
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
                dropTarget.ReceiveIngredient(ingredientAsset);
            }
        }

        Destroy(draggedObject);
        draggedObject = null;
    }
}