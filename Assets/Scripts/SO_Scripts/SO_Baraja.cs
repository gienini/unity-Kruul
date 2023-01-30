using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Baraja", menuName = "Scriptable Objects/Baraja")]
public class SO_Baraja : ScriptableObject
{
    [SerializeField] public List<string> cartas;
}
