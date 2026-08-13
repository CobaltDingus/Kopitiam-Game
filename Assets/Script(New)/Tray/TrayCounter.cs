using UnityEngine;

public class TrayCounter : MonoBehaviour
{
    public TrayDatabase trayDatabase;

    public GameObject slotOne;
    public GameObject slotTwo;
    public GameObject slotThree;

    void Update()
    {
        GameObject[] slots =
        {
            slotOne,
            slotTwo,
            slotThree
        };

        for (int i = 0; i < slots.Length; i++)
        {
            SpriteRenderer spriteRenderer = slots[i].GetComponentInChildren<SpriteRenderer>();

            if (i < trayDatabase.SavedDrinks.Count)
            {
                spriteRenderer.sprite = trayDatabase.SavedDrinks[i].drinkSprite;
            }
            else
            {
                spriteRenderer.sprite = null;
            }
        }
    }
}