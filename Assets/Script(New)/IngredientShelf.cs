using System.Collections.Generic;
using UnityEngine;

public class IngredientShelf : MonoBehaviour
{
    [SerializeField] List<IngredientObject> shelfIngredients;

    void OnEnable()
    {
        // ReworkedSaveManager.OnDayStart += UnlockIngredientDay;
        UnlockIngredientDay();
    }

    void OnDisable()
    {
        // ReworkedSaveManager.OnDayStart -= UnlockIngredientDay;
    }

    void Start()
    {
        
    }

    void Update()
    {

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
