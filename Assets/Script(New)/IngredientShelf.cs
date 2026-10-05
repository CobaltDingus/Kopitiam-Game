using System.Collections.Generic;
using UnityEngine;

public class IngredientShelf : MonoBehaviour
{
    [SerializeField] List<IngredientObject> shelfIngredients;

    void OnEnable()
    {
        UnlockIngredientDay();
    }

    private void UnlockIngredientDay()
    {
        foreach (IngredientObject ingObj in shelfIngredients)
        {
            if (ingObj == null) continue;

            if (ReworkedSaveManager.instance && ReworkedSaveManager.instance.Day >= ingObj.unlockDay )
            {
                ingObj.gameObject.SetActive(true);
            }
        }
    }
}
