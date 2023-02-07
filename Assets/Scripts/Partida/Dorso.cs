using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dorso : MonoBehaviour
{
    [SerializeField] public ParticleSystem ParticulasVoltear = null;
    [SerializeField] public ParticleSystem ParticulasEsperar = null;
    [SerializeField] public ParticleSystem ParticulasEmpezar = null;
    [SerializeField] public List<GameObject> PartesDorso = null;
    [SerializeField] public GameObject Carta = null;
    private void Start()
    {
        foreach (GameObject go in PartesDorso)
        {
            go.SetActive(true);
        }
        Carta.SetActive(false);
        ToggleParticulasEmpezar();
    }
    public void ToggleDorsoCarta()
    {
        foreach (GameObject go in PartesDorso)
        {
            go.SetActive(!go.activeSelf);
        }
        Carta.SetActive(!Carta.activeSelf);
    }
    public void ToggleParticulasEsperar()
    {
        ParticulasEsperar.gameObject.SetActive(ParticulasEsperar.gameObject.activeSelf);
    }

    public void ToggleParticulasEmpezar()
    {
        ParticulasEmpezar.gameObject.SetActive(ParticulasEmpezar.gameObject.activeSelf);
    }

    public void ToggleParticulasVoltear()
    {
        ParticulasVoltear.gameObject.SetActive(ParticulasVoltear.gameObject.activeSelf);
    }
}
