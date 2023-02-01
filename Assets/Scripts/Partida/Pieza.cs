using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieza : MonoBehaviour
{
    [SerializeField] private Sprite spriteColor1 = null;
    [SerializeField] private Sprite spriteColor2 = null;
    [SerializeField] private bool _esColor1;
    private bool _esRefresh = false;

    public bool EsColor1 { get => _esColor1; set => setColor(value); }

    private void setColor(bool esColor1)
    {
        _esColor1 = esColor1;
        _esRefresh = true;

    }

    private void Update()
    {
        if (_esRefresh)
        {
            if (EsColor1)
            {
                GetComponent<SpriteRenderer>().sprite = spriteColor1;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = spriteColor2;
            }
            _esRefresh = false;
        }
    }
}
