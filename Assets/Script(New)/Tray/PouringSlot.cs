using UnityEngine;

public class PouringSlot : DraggableObject, DropInterface
{
    [SerializeField] private SpriteRenderer slotSprite;
    [SerializeField] private SpriteRenderer liquidSprite;
    [SerializeField] private Sprite outlineSprite;

    public void SetCamera(Camera newCamera)
    {
        cam = newCamera;
    }

    private void OnEnable()
    {
        DrinkManager.Instance.OnPouringSlotChanged += RefreshVisuals;
    }

    private void OnDisable()
    {
        DrinkManager.Instance.OnPouringSlotChanged -= RefreshVisuals;
    }

    private void Start()
    {
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        var state = DrinkManager.Instance.PouringSlot;

        if (!state.hasContainer)
        {
            slotSprite.sprite = outlineSprite;
            liquidSprite.enabled = false;
            liquidSprite.color = Color.white;
            canDrag = false;
            dragType = DragEnum.None;
            return;
        }

        slotSprite.sprite = state.containerSprite;
        canDrag = true;

        if (state.storedDrink.isFinished)
        {
            liquidSprite.enabled = true;
            liquidSprite.color = GlobalUtilities.HexToColor(state.storedDrink.colorHex);
            dragType = DragEnum.FinishedDrink;
        }
        else
        {
            liquidSprite.enabled = false;
            dragType = DragEnum.DrinkContainer;
        }
    }

    public bool ReceiveDraggable<T>(T draggableObject)
    {
        var state = DrinkManager.Instance.PouringSlot;

        if (!state.hasContainer)
        {
            if (draggableObject is DrinkContainer drinkContainer)
            {
                AudioManager.instance?.PlaySFX(SFXType.CupPlacing);
                return DrinkManager.Instance.TrySetContainer(
                    drinkContainer.containerType, drinkContainer.containerSprite);
            }

            Debug.Log("Place a cup first!");
            return false;
        }
        else
        {
            if (draggableObject is Drink drink && drink.isStirred)
            {
                return DrinkManager.Instance.TryPourDrink(drink);
            }

            return false;
        }
    }

    public override object GetData()
    {
        return DrinkManager.Instance.TakePouringSlotData();
    }

    public override void AfterDropFunctions()
    {
        DrinkManager.Instance.ClearPouringSlot();
    }
}