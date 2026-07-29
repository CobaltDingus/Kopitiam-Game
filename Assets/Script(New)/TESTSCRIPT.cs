using UnityEngine;
using UnityEngine.EventSystems;

public class TESTSCRIPT : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
    }
}