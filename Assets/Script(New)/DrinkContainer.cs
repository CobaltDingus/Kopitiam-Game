using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public enum ContainerType
{
    None,
    Hot,
    Cold,
    Takeaway
}
public class DrinkContainer: DraggableObject
{
    public ContainerType containerType;
    public Sprite containerSprite;
    // private Drink containerData;
    public override object GetData()
    {
        return this;
    }
    public override void AfterDropFunctions()
    {
        return;
    }
    // public Ingredient cupType;
    //OLD CODE
    //public List<Ingredient> ingredients = new();

    // public Color displayColor;
    // public string displayName;
}