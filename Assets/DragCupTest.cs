using UnityEngine;
using TMPro;

public class DragCupTest : MonoBehaviour
{
    private bool dragging;
    private Vector3 startPosition;
    [SerializeField] private TMP_Text drinkText;
    private Collider2D cupCollider;

    public string DrinkContents => drinkText.text;
    void Start()
    {
        startPosition = transform.position;
        cupCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;

        if (dragging)
        {
            transform.position = mouse;
        }

        if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;

            // Enable collider again after checking
            cupCollider.enabled = true;

            // Collider2D hit = Physics2D.OverlapPoint(mouse);

            // if (hit != null)
            // {
            //     Debug.Log(hit.gameObject.name);
            //     Customer customer = hit.GetComponent<Customer>();

            //     if (customer != null)
            //     {
            //         customer.ReceiveCup(this);
            //         return;
            //     }
            // }
            // else
            // {
            //     Debug.Log("Nothing detected");
            // }

            Collider2D[] hits = Physics2D.OverlapPointAll(mouse);

            foreach (Collider2D hit in hits)
            {
                Customer customer = hit.GetComponent<Customer>();

                if (customer != null)
                {
                    customer.ReceiveCup(this);
                    Destroy(gameObject);
                    return;
                }
            }

            transform.position = startPosition;
        }
    }

    void OnMouseDown()
    {
        dragging = true;

        cupCollider.enabled = false;

        Debug.Log("Cup collider enabled: " + cupCollider.enabled);
    }
}