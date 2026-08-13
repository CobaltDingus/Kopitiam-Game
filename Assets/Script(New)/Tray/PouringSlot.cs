using UnityEngine;
using TMPro;

public class PouringSlot : MonoBehaviour, DropDrinkInterface
{
    [SerializeField] private TMP_Text slotText;
    [SerializeField] private SpriteRenderer slotSprite;
    private Drink storedDrink;
    public bool ReceiveDrink(Drink drink)
    {
        Debug.Log("Tray received drink!");
        if (drink == null)
            return false;

        storedDrink = drink;

        slotText.text = string.Join(
            "\n",
            storedDrink.ingredients.ConvertAll(i => i.Name)
        );
        // square.color = cup.displayColor;
        // text.text = cup.displayName;

        return true;
    }
}