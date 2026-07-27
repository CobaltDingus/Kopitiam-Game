using UnityEngine;

public class DraggedIngredient : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void Setup(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
