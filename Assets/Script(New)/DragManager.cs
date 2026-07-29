using UnityEngine;

public class DragManager : MonoBehaviour
{
    [SerializeField] private GameObject dragPrefab;

    private GameObject draggedObject;
    private Ingredient draggedIngredient;

    void Update()
    {
        Vector3 mouse =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        // Start dragging
    if (Input.GetMouseButtonDown(0))
    {
        Collider2D hit = Physics2D.OverlapPoint(mouse);

        if (hit != null)
        {
            IngredientPrefab ingredient =
                hit.GetComponent<IngredientPrefab>();

            if (ingredient != null)
            {
                draggedIngredient = ingredient.ingredientAsset;

                draggedObject =
                    Instantiate(dragPrefab, mouse, Quaternion.identity);

                SpriteRenderer dragRenderer =
                    draggedObject.GetComponentInChildren<SpriteRenderer>();

                dragRenderer.color =
                    ingredient.placeholderRenderer.color;
            }
        }
    }

        // Move dragged object
        if (draggedObject != null)
        {
            draggedObject.transform.position = mouse;
        }

        // Release
    if (Input.GetMouseButtonUp(0))
    {
        if (draggedObject != null)
        {
            Collider2D hit = Physics2D.OverlapPoint(mouse);

            if (hit != null)
            {
                DropInterface target =
                    hit.GetComponent<DropInterface>();

                if (target != null)
                {
                    target.ReceiveIngredient(draggedIngredient);
                }
            }

            Destroy(draggedObject);

            draggedObject = null;
            draggedIngredient = null;
        }
    }
    }
}