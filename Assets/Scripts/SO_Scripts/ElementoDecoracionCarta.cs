using UnityEngine;

[System.Serializable]
public class ElementoDecoracionCarta
{
    private Transform transform;
    public SpriteRenderer spriteRenderer;

    public Transform Transform { get => transform; set => transform = value; }

    public ElementoDecoracionCarta()
    {

    }
}
