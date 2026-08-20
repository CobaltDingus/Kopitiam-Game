using UnityEngine;

public class RubbishBin : 
ExpandableUI, 
// DropDrinkInterface
DropInterface
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool ReceiveDraggable<T>(T dragData) 
    {
        Debug.Log("Item was thrown away");
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
