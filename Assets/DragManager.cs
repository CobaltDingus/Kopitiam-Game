using UnityEngine;

public class DragManager : MonoBehaviour
{
    [SerializeField] private GameObject dragPrefab;

    private GameObject draggedObject;
    private string draggedIngredient;

    void Update()
    {
        Vector3 mouse =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouse);

            if (hit != null)
            {
                IngredientSource source =
                    hit.GetComponent<IngredientSource>();

                if (source != null)
                {
                    draggedIngredient = source.ingredientName;

                    draggedObject = Instantiate(dragPrefab, mouse, Quaternion.identity);

                    SpriteRenderer sourceRenderer = source.GetComponent<SpriteRenderer>();
                    SpriteRenderer dragRenderer = draggedObject.GetComponent<SpriteRenderer>();

                    dragRenderer.color = sourceRenderer.color;
                }
            }
        }

        if (draggedObject != null)
        {
            draggedObject.transform.position = mouse;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (draggedObject != null)
            {
                Collider2D hit = Physics2D.OverlapPoint(mouse);

                if (hit != null)
                {
                    Cup cup = hit.GetComponent<Cup>();

                    if (cup != null)
                    {
                        cup.AddIngredient(draggedIngredient);
                    }
                }

                Destroy(draggedObject);
                draggedObject = null;
            }
        }
    }
}