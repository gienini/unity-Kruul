using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_DecoracionesCartas", menuName = "Scriptable Objects/Decoraciones Baraja")]
public class SO_DecoracionesCartas : ScriptableObject
{
    [SerializeField] public bool esColor1;
    [SerializeField] private List<ElementoDecoracionCarta> cartas;
}
