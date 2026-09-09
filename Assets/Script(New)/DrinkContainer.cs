using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]

public enum ContainerType
{
    None,
    Hot,
    Cold,
    Takeaway
}
public class DrinkContainer: 
MonoBehaviour,
IPointerDownHandler
// DraggableObject
{
    public ContainerType containerType;
    public Sprite containerSprite;
    [SerializeField] PouringSlot pouringSlot;

    // public object GetData()
    // {
    //     return this;
    // }
    // public void AfterDropFunctions()
    // {
    //     return;
    // }
    public void OnPointerDown(PointerEventData eventData) 
    {
        pouringSlot.ReceiveDraggable(this);
    }
}