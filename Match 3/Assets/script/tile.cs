using UnityEngine;

public class tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool isSelected;
    public bool isEmpty
    {
        get
        {
            return spriteRenderer.sprite == null ? true : false;
        }
    }
}
