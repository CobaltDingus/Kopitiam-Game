using UnityEngine;

public class IngredientShelf : MonoBehaviour
{
    [SerializeField] IngredientObject teaObject;
    [SerializeField] IngredientObject uiloObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveManager.saveManager != null && SaveManager.saveManager.DayCount >= 2)
        {
            teaObject.gameObject.SetActive(true);
            uiloObject.gameObject.SetActive(true);       
            
        }
    }
}
