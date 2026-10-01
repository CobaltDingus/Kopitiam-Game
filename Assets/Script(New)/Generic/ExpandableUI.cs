using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExpandableUI : MonoBehaviour
{
    [SerializeField] protected Sprite inactiveSprite;
    [SerializeField] protected Sprite activeSprite;
    [SerializeField] protected GameObject inactiveObject;
    [SerializeField] protected GameObject activeObject;
    [SerializeField] protected Vector3 activeOffset;
    [SerializeField] protected Transform activeTransform;
    [SerializeField] protected Transform inactiveTransform;
    private Vector3 activePositionStored;
    private Vector3 inactivePositionStored;
    [SerializeField] private float inactiveAlpha;
    [SerializeField] private float activeAlpha;
    [SerializeField] protected List<DragEnum> validDragTypes;

    protected SpriteRenderer spriteRenderer;
    protected Vector3 originalPosition;

    private enum ActivateType
    {
        TransformSprite,
        SwapSprite
    }

    [SerializeField] private ActivateType activateType;


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
        activePositionStored = activeTransform.position;
        inactivePositionStored = inactiveTransform.position;
        // spriteRenderer = GetComponent<SpriteRenderer>();
        // originalPosition = transform.localPosition;
        // SetActiveState(false);
    }

    protected virtual void SetActiveState(DragEnum dragType)
    {
        // spriteRenderer.sprite = active ? activeSprite : inactiveSprite;

        // transform.localPosition =
        //     active ? originalPosition + activeOffset : originalPosition;
        
        if (activateType == ActivateType.TransformSprite)
        {
            if (validDragTypes.Contains(dragType))
            {
                
                // activeObject.SetActive(true);
                // inactiveObject.SetActive(false);
                inactiveObject.transform.position = activePositionStored;
                // inactiveObject
            }
            else
            {
                // activeObject.SetActive(false);
                // inactiveObject.SetActive(true);
                // inactiveObject.transform.position += activeOffset;
                inactiveObject.transform.position = inactivePositionStored;
            }
        } else if (activateType == ActivateType.SwapSprite)
        {
            if (validDragTypes.Contains(dragType))
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


}
