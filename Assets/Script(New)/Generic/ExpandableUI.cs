using System.Collections.Generic;
using UnityEngine;

public class ExpandableUI : MonoBehaviour
{
    [SerializeField] protected GameObject inactiveObject;
    [SerializeField] protected GameObject activeObject;
    [SerializeField] protected Vector3 activeOffset;
    [SerializeField] protected Transform activeTransform;
    [SerializeField] protected Transform inactiveTransform;
    private Vector3 activePositionStored;
    private Vector3 inactivePositionStored;
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
    }

    protected virtual void SetActiveState(DragEnum dragType)
    {
        if (activateType == ActivateType.TransformSprite)
        {
            if (validDragTypes.Contains(dragType))
            {
                inactiveObject.transform.position = activePositionStored;
            }
            else
            {
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
