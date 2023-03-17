using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPartidaUI : MonoBehaviour
{
    [SerializeField] private GameObject GrupoPiezasColor1 = null;
    [SerializeField] private GameObject GrupoPiezasColor2 = null;
    [SerializeField] private GameObject GrupoDorsosUI = null;
    [SerializeField] private GameObject DisplayDorsosUI = null;
    [SerializeField] private GameObject DorsoPrefab = null;
    [SerializeField] private GameObject PiezaPrefabColor1 = null;
    [SerializeField] private GameObject PiezaPrefabColor2 = null;
    [SerializeField] SeleccionAccion grupoSeleccionAccion = null;
    private List<Image> _piezasColor1;
    private List<Image> _piezasColor2;
    private int _numPiezasColor1 = 9;
    private int _numPiezasColor2 = 9;
    private Camera mainCamera;
    private List<GameObject> _listDorsos;
    private float _camaraPosicionZ;
    private bool _esTriggerAnimacionInicial = false;
    private bool esSeleccionAccion = false;

    private void Awake()
    {
    }

    void Start()
    {
    }
    public void iniciaPartida()
    {
        _piezasColor1 = new List<Image>();
        foreach (Image child in GrupoPiezasColor1.GetComponentsInChildren<Image>())
        {
            if (child.gameObject.name.Contains("Pieza"))
            {
                _piezasColor1.Add(child);
            }
        }
        _piezasColor2 = new List<Image>();
        foreach (Image child in GrupoPiezasColor2.GetComponentsInChildren<Image>())
        {
            if (child.gameObject.name.Contains("Pieza"))
            {
                _piezasColor2.Add(child);
            }
        }
        GrupoDorsosUI.GetComponentInChildren<Button>().interactable = false;
        GrupoPiezasColor1.GetComponentInChildren<Button>().interactable = false;
        GrupoPiezasColor2.GetComponentInChildren<Button>().interactable = false;
    }
    void Update()
    {
        displayNombreJugador();
    }
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent += DespuesFadeOutEvent;
        EventHandler.JugadaHechaEvent += JugadaHechaEvent;
        EventHandler.AccionSeleccionadaEvent += AccionSeleccionadaEvent;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
        EventHandler.AccionSeleccionadaEvent -= AccionSeleccionadaEvent;
    }

    private void AccionSeleccionadaEvent(bool esCarta)
    {
        if (esCarta)
        {
            StartCoroutine(GetDorsoSiguiente());
        }
    }
    public void BotonPilaCartas()
    {
        escondeOpcionesAccion();
        EventHandler.CallAccionSeleccionadaEvent(true);
    }
    public void BotonPilaGrupoPiezas1()
    {
        escondeOpcionesAccion();
        EventHandler.CallAccionSeleccionadaEvent(false);
    }
    public void BotonPilaGrupoPiezas2()
    {
        escondeOpcionesAccion();
        EventHandler.CallAccionSeleccionadaEvent(false);
    }
    private void escondeOpcionesAccion()
    {
        if (esSeleccionAccion)
        {
            grupoSeleccionAccion.gameObject.SetActive(false);
            esSeleccionAccion = false;
            //FaderPartidaUI.transform.SetAsLastSibling();
            GrupoPiezasColor2.SetActive(true);
            GrupoPiezasColor1.SetActive(true);
            GrupoPiezasColor1.GetComponentInChildren<Button>().interactable = false;
            GrupoPiezasColor2.GetComponentInChildren<Button>().interactable = false;
            GrupoDorsosUI.GetComponentInChildren<Button>().interactable = false;
            StartCoroutine(SceneControllerManager.Instance.Fade(0f));
        }
    }
    
    private void muestraOpcionesAccion(bool esJugador1)
    {
        grupoSeleccionAccion.gameObject.SetActive(true);
        StartCoroutine(AnimacionFade(1f, esJugador1));
    }
    private void displayNombreJugador()
    {
        if (PropiedadesCasillasManager.Instance.EsTurnoColor1)
        {
            GrupoPiezasColor1.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
            GrupoPiezasColor2.GetComponentInChildren<TextMeshProUGUI>().alpha = 0f;
        } else {
            GrupoPiezasColor1.GetComponentInChildren<TextMeshProUGUI>().alpha = 0f;
            GrupoPiezasColor2.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
        }
    }
    private IEnumerator AnimacionFade(float finalAlpha, bool esJugador1)
    {
        //FaderPartidaUI.transform.SetAsLastSibling();
        if (esJugador1)
        {
            GrupoPiezasColor2.SetActive(false);
        }
        else
        {
            GrupoPiezasColor1.SetActive(false);
        }
        GrupoDorsosUI.transform.SetAsLastSibling();
        GrupoDorsosUI.GetComponentInChildren<Button>().transform.SetAsLastSibling();
        yield return SceneControllerManager.Instance.Fade(finalAlpha);
        if (esJugador1)
        {
            GrupoPiezasColor1.GetComponentInChildren<Button>().interactable = true;
        } else
        {
            GrupoPiezasColor2.GetComponentInChildren<Button>().interactable = true;
        }
        GrupoDorsosUI.GetComponentInChildren<Button>().interactable = true;
        esSeleccionAccion = true;
        
    }
    private void JugadaHechaEvent()
    {
        if (PropiedadesCasillasManager.Instance.EsFase1)
        {
            List<ValorCasilla> casillasConPunto = PropiedadesCasillasManager.Instance.CheckPuntoEnTablero(PropiedadesCasillasManager.Instance.EsTurnoColor1);
            if (casillasConPunto.Count > 0 && ((PropiedadesCasillasManager.Instance.EsTurnoColor1 && _numPiezasColor1 > 0) || (!PropiedadesCasillasManager.Instance.EsTurnoColor1 && _numPiezasColor2 > 0)))
            {
                muestraOpcionesAccion(PropiedadesCasillasManager.Instance.EsTurnoColor1);
            }
            else
            {
                //No hay decision, se juega carta
                EventHandler.CallAccionSeleccionadaEvent(true);
            }
        }
        refreshFichas();
    }

    private void refreshFichas()
    {
        for (int i = 0; i < _piezasColor1.Count; i++)
        {
            _piezasColor1[i].enabled = i >= PropiedadesCasillasManager.Instance.NumFichasPuestasJ1;
        }

        for (int i = 0; i < _piezasColor2.Count; i++)
        {
            _piezasColor2[i].enabled = i >= PropiedadesCasillasManager.Instance.NumFichasPuestasJ2;
        }

    }

    private void DespuesFadeOutEvent()
    {
        if (_esTriggerAnimacionInicial)
        {
            //nos saltamos la animacion de la carta que aparece en el tablero, se hace directamente la del cursor
            //StartCoroutine(instanciaPilaDorsos(Settings.NumCartasTotal));
            StartCoroutine(instanciaPilaDorsos(Settings.NumCartasTotal -1));
            _esTriggerAnimacionInicial = false;
        }
    }

    private void EmpiezaFase1Event()
    {
        iniciaPartida();
        mainCamera = Camera.main;
        _camaraPosicionZ = mainCamera.transform.position.z;
        _esTriggerAnimacionInicial = true;
        refreshFichas();
    }
    private IEnumerator instanciaPilaDorsos(int barajaCount)
    {
        Vector3 posicionDestino = new Vector3(-((barajaCount/2)*Settings.SeparacionSpawnCartasUI), 0, 0);
        _listDorsos = new List<GameObject>();
        for (int i = 0; i < barajaCount; i++)
        {
            yield return new WaitForSeconds(0.05f);
            if (i == (barajaCount - 1))
            {
                //Ultima vuelta, la carta va al medio
                //yield return new WaitForSeconds(0.30f);
                //StartCoroutine(instanciaDorso(new Vector3(0, 0, -_camaraPosicionZ), true));
                StartCoroutine(instanciaDorso(new Vector3(posicionDestino.x, posicionDestino.y, -_camaraPosicionZ), true));
                posicionDestino = new Vector3(posicionDestino.x + Settings.SeparacionSpawnCartasUI, posicionDestino.y, 0);

            }
            else
            {
                //StartCoroutine(instanciaDorso(new Vector3(posicion.x + Random.Range(-0.01f, 0.01f), posicion.y + Random.Range(-0.1f, 0.1f), -_camaraPosicionZ)));
                StartCoroutine(instanciaDorso(new Vector3(posicionDestino.x , posicionDestino.y , -_camaraPosicionZ), false));
                posicionDestino = new Vector3(posicionDestino.x + Settings.SeparacionSpawnCartasUI, posicionDestino.y, 0);
            }

        }
        yield return null;
    }

    private IEnumerator instanciaDorso(Vector3 posicionFinal, bool esUltima)
    {
        //Vector3 posicionInicial = new Vector3(Random.Range(-12f, 21f), 10f, -_camaraPosicionZ);
        Vector3 posicionInicial = new Vector3(0, 0, 0);
        Vector3 porcionViaje = (posicionFinal - posicionInicial) / 100f;
        GameObject dorso = Instantiate(DorsoPrefab, DisplayDorsosUI.transform, false);
        dorso.transform.localScale = new Vector3(3f, 3f, 1f);
        _listDorsos.Add(dorso);
        for (int i = 0; i < 100; i++)//(dorso.transform.position != posicionFinal)
        {
            dorso.transform.position = dorso.transform.position + porcionViaje;
            yield return new WaitForSeconds(0.01f);
        }
        if (esUltima)
        {
            EventHandler.CallDespuesIntroFase1Event();
            StartCoroutine(GetDorsoSiguiente());
        }

        yield return null;
    }
    public IEnumerator GetDorsoSiguiente()
    {
        if (_listDorsos.Count > 0)
        {
            GameObject dorsoSiguiente = _listDorsos[_listDorsos.Count - 1];
            _listDorsos.RemoveAt(_listDorsos.Count - 1);
            for (int i = 0; i < 100; i++)
            {
                dorsoSiguiente.transform.position += Vector3.up;
                yield return new WaitForSeconds(0.01f);
            }
            Destroy(dorsoSiguiente);
        }
        yield return null;
    }

}
