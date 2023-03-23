using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeleccionAccion : MonoBehaviour
{
    [SerializeField] private Image flechaP1;
    [SerializeField] private Image flechaP2;
    [SerializeField] private Image flechaDorsos;
    private bool esAnimacionActivada = false;

    private float yMaxima = 240;
    private float yMinima = 200;
    private bool esSubiendo = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        esSubiendo = false;
        flechaP1.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP1.gameObject.GetComponent<RectTransform>().transform.position.x, yMaxima, flechaP1.gameObject.GetComponent<RectTransform>().transform.position.z);
        flechaP2.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP2.gameObject.GetComponent<RectTransform>().transform.position.x, yMaxima, flechaP2.gameObject.GetComponent<RectTransform>().transform.position.z);
        flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.x, yMaxima, flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.z);
        if (PropiedadesCasillasManager.Instance.EsTurnoColor1)
        {
            flechaP2.color = new Color(flechaP2.color.r, flechaP2.color.g, flechaP2.color.b, 0f);
        }else
        {
            flechaP1.color = new Color(flechaP1.color.r, flechaP1.color.g, flechaP1.color.b, 0f);
        }
        esAnimacionActivada = true;
        StartCoroutine(animacionFlechas());
    }
    private void OnDisable()
    {
        esAnimacionActivada = false;
        flechaP2.color = new Color(flechaP2.color.r, flechaP2.color.g, flechaP2.color.b, 1f);
        flechaP1.color = new Color(flechaP2.color.r, flechaP2.color.g, flechaP2.color.b, 1f);
    }

    private IEnumerator animacionFlechas()
    {
        float porcionViaje = (yMinima - yMaxima) / 20;
        while (1 == 1)
        {
            if (esAnimacionActivada)
            {
                if (esSubiendo)
                {
                    flechaP1.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP1.gameObject.GetComponent<RectTransform>().transform.position.x, flechaP1.gameObject.GetComponent<RectTransform>().transform.position.y - (porcionViaje), flechaP1.gameObject.GetComponent<RectTransform>().transform.position.z);
                    flechaP2.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP2.gameObject.GetComponent<RectTransform>().transform.position.x, flechaP2.gameObject.GetComponent<RectTransform>().transform.position.y - (porcionViaje), flechaP2.gameObject.GetComponent<RectTransform>().transform.position.z);
                    flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.x, flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.y - (porcionViaje), flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.z);
                    if (flechaP1.gameObject.GetComponent<RectTransform>().transform.position.y >= yMaxima)
                    {
                        esSubiendo = false;
                    }
                }else
                {
                    flechaP1.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP1.gameObject.GetComponent<RectTransform>().transform.position.x, flechaP1.gameObject.GetComponent<RectTransform>().transform.position.y + (porcionViaje), flechaP1.gameObject.GetComponent<RectTransform>().transform.position.z);
                    flechaP2.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaP2.gameObject.GetComponent<RectTransform>().transform.position.x, flechaP2.gameObject.GetComponent<RectTransform>().transform.position.y + (porcionViaje), flechaP2.gameObject.GetComponent<RectTransform>().transform.position.z);
                    flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position = new Vector3(flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.x, flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.y + (porcionViaje), flechaDorsos.gameObject.GetComponent<RectTransform>().transform.position.z);
                    if (flechaP1.gameObject.GetComponent<RectTransform>().transform.position.y <= yMinima)
                    {
                        esSubiendo = true;
                    }
                }
                yield return new WaitForSeconds(0.05f);

            }
        }
    }
}
