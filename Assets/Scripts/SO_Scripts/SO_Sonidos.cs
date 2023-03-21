using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_PropiedadesCasilla", menuName = "Scriptable Objects/Sonidos")]

public class SO_Sonidos : ScriptableObject
{
    [SerializeField]
    public List<SoundItem> soundDetails;
}
