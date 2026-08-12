using UnityEngine;

public class ExpandableUI : MonoBehaviour
{
    [SerializeField] protected Sprite inactiveSprite;
    [SerializeField] protected Sprite activeSprite;
    [SerializeField] protected GameObject inactiveObject;
    [SerializeField] protected GameObject activeObject;
    [SerializeField] protected Vector3 activeOffset;

    protected SpriteRenderer spriteRenderer;
    protected Vector3 originalPosition;


    private void OnEnable()
    {
        DragManager.onDragChange += SetActiveState;
    }

    private void OnDisable()
    {
        DragManager.onDragChange -= SetActiveState;
    }

    protected virtual void Awake()
    {
        // spriteRenderer = GetComponent<SpriteRenderer>();
        // originalPosition = transform.localPosition;
        // SetActiveState(false);
    }

    protected virtual void SetActiveState(DragEnum dragType)
    {
        // spriteRenderer.sprite = active ? activeSprite : inactiveSprite;

        // transform.localPosition =
        //     active ? originalPosition + activeOffset : originalPosition;
        if (dragType == DragEnum.FinishedDrink)
        {
            activeObject.SetActive(true);
            inactiveObject.SetActive(false);
        }
        else
        {
            activeObject.SetActive(false);
            inactiveObject.SetActive(true);
        }
    }


}
