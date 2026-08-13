using UnityEngine;

public class RubbishBin : 
ExpandableUI, 
DropDrinkInterface
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool ReceiveDrink(Drink drink) 
    {
        Debug.Log("Drink was thrown way");
        return true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
