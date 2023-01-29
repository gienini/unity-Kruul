using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_PropiedadesCasilla", menuName = "Scriptable Objects/PropiedadesCasilla")]
public class SO_PropiedadesCasilla : ScriptableObject 
{
    public int gridAncho;
    public int gridAlto;
    public int originX;
    public int originY;
     
    [SerializeField]
    public List<PropiedadCasilla> propiedadCasillaList;
}
