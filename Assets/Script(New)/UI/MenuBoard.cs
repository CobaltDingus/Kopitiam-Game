using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBoard : 
MonoBehaviour,
IPointerDownHandler
{
    [SerializeField] private UIPanel panel;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private RecipeBook recipeBook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        menuText.text = "MENU\n\n" + string.Join(
            "\n",
            recipeBook.allRecipes.ConvertAll(i => "- " + i.drinkName)
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!panel.gameObject.activeSelf)
        {
            panel.OpenPanel();
        }
    }
}
