using UnityEngine;
using TMPro;

public class PouringSlot : DraggableObject, DropInterface
{
    // [SerializeField] private TMP_Text slotText;
    [SerializeField] private SpriteRenderer slotSprite;
    private Drink storedDrink = new Drink();
    private bool hasContainer;

    private ContainerType storedContainerType;

    public bool ReceiveDraggable<T>(T draggableObject)
    {
        if (!hasContainer)
        {
            Debug.Log("Container pass");
            if (draggableObject is DrinkContainer drinkContainer)
            {
                hasContainer = true;
                storedContainerType = drinkContainer.containerType;
                slotSprite.sprite = drinkContainer.containerSprite;
                dragType = DragEnum.DrinkContainer;
                canDrag = true;
                return true;
            }
            else
            {
                Debug.Log("Place a cup first!");
                return false;
            }           
        }
        else
        {
            if (draggableObject is Drink drink)
            {
                storedDrink = drink.Clone();
                storedDrink.containerType = storedContainerType;
                storedDrink.isFinished = true;
                slotSprite.sprite = storedDrink.drinkSprite;
                canDrag = true;
                dragType = DragEnum.FinishedDrink;

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override object GetData()
    {
        if (storedDrink.isFinished)
        {
            return storedDrink.Clone();
        }
        else
        {
            return storedContainerType;
        }
    }

    public override void AfterDropFunctions()
    {
        resetContents();
    }
    public void resetContents()
    {
        Drink emptyDrink = new Drink();
        storedDrink = emptyDrink;
        canDrag = false;
        hasContainer = false;
        dragType = DragEnum.None;
        slotSprite.sprite = null;
    }
    // public bool ReceiveDrink(Drink drink)
    // {
    //     Debug.Log("Tray received drink!");
    //     if (drink == null)
    //         return false;

    //     storedDrink = drink;

    //     slotText.text = string.Join(
    //         "\n",
    //         storedDrink.ingredients.ConvertAll(i => i.Name)
    //     );
    //     // square.color = cup.displayColor;
    //     // text.text = cup.displayName;

    //     return true;
    // }
}