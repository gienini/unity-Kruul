using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieza : MonoBehaviour
{
    [SerializeField] private Sprite spriteColor1 = null;
    [SerializeField] private Sprite spriteColor2 = null;
    [SerializeField] private bool _esColor1;

    public bool EsColor1 { get => _esColor1; set => _esColor1 = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (EsColor1)
        {
            GetComponent<SpriteRenderer>().sprite = spriteColor1;
        }else
        {
            GetComponent<SpriteRenderer>().sprite = spriteColor2;
        }
        
    }
}
