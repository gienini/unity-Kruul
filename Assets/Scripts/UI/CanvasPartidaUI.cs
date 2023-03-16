using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPartidaUI : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject GrupoPiezasColor1 = null;
    [SerializeField] private GameObject GrupoPiezasColor2 = null;
    [SerializeField] private GameObject GrupoDorsosUI = null;
    [SerializeField] private GameObject DisplayDorsosUI = null;
    [SerializeField] private GameObject DorsoPrefab = null;
    [SerializeField] private GameObject PiezaPrefabColor1 = null;
    [SerializeField] private GameObject PiezaPrefabColor2 = null;
    private List<Image> _piezasColor1;
    private List<Image> _piezasColor2;
    private int _numPiezasColor1 = 9;
    private int _numPiezasColor2 = 9;
    private Camera mainCamera;
    private List<GameObject> _listDorsos;
    private float _camaraPosicionZ;
    private bool _esTriggerAnimacionInicial = false;
    private bool isFading;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }
    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    private void Awake()
    {
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
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
        ISaveableRegister();
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.DespuesFadeOutEvent -= DespuesFadeOutEvent;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
        EventHandler.AccionSeleccionadaEvent -= AccionSeleccionadaEvent;
        ISaveableDeregister();
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
        //FaderPartidaUI.transform.SetAsLastSibling();
        GrupoPiezasColor2.SetActive(true);
        GrupoPiezasColor1.SetActive(true);
        GrupoPiezasColor1.GetComponentInChildren<Button>().interactable = false;
        GrupoPiezasColor2.GetComponentInChildren<Button>().interactable = false;
        StartCoroutine(SceneControllerManager.Instance.Fade(0f));
    }
    private void muestraOpcionesAccion(bool esJugador1)
    {
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

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave IsaveableSave()
    {
        SceneSave sceneSave = new SceneSave();
        GameObjectSave.sceneData.Remove(NombresEscena.Escena_PartidaNormal.ToString());
        sceneSave.dictVector3 = new Dictionary<string, List<Vector3Serializable>>();
        if (_piezasColor1 != null && _piezasColor1.Count > 0)
        {
            List<Vector3Serializable> images = new List<Vector3Serializable>();
            foreach(Image pieza in _piezasColor1)
            {
                images.Add(new Vector3Serializable(pieza.gameObject.transform.position.x, pieza.transform.position.y, pieza.transform.position.z));
            }
            sceneSave.dictVector3.Add("_piezasColor1", images);
        }
        if (_piezasColor2 != null && _piezasColor2.Count > 0)
        {
            List<Vector3Serializable> images = new List<Vector3Serializable>();
            foreach (Image pieza in _piezasColor2)
            {
                images.Add(new Vector3Serializable(pieza.gameObject.transform.position.x, pieza.transform.position.y, pieza.transform.position.z));
            }
            sceneSave.dictVector3.Add("_piezasColor2", images);
        }
        if (_listDorsos != null && _listDorsos.Count > 0)
        {
            List<Vector3Serializable> images = new List<Vector3Serializable>();
            foreach (GameObject pieza in _listDorsos)
            {
                images.Add(new Vector3Serializable(pieza.transform.position.x, pieza.transform.position.y, pieza.transform.position.z));
            }
            sceneSave.dictVector3.Add("_listDorsos", images);
        }
        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.intDictionary.Add("_numPiezasColor1", _numPiezasColor1);
        sceneSave.intDictionary.Add("_numPiezasColor2", _numPiezasColor2);
        sceneSave.boolDictionary = new Dictionary<string, bool>();
        GameObjectSave.sceneData.Add(NombresEscena.Escena_PartidaNormal.ToString(), sceneSave);
        return GameObjectSave;
    }
    public void IsaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;
            if (gameObjectSave.sceneData.TryGetValue(NombresEscena.Escena_PartidaNormal.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.dictVector3 != null && sceneSave.dictVector3.TryGetValue("_piezasColor1", out List<Vector3Serializable> piezasColor1))
                {
                    foreach (Image pieza in _piezasColor1)
                    {
                        Destroy(pieza);
                    }
                    _piezasColor1 = new List<Image>();
                    foreach (Vector3Serializable v3s in piezasColor1)
                    {
                        GameObject pieza = Instantiate(PiezaPrefabColor1, GrupoPiezasColor1.transform, false);
                        pieza.transform.position = new Vector3(v3s.x, v3s.y, v3s.z);
                        _piezasColor1.Add(pieza.GetComponent<Image>());
                    }
                }
                if (sceneSave.dictVector3 != null && sceneSave.dictVector3.TryGetValue("_piezasColor2", out List<Vector3Serializable> piezasColor2))
                {
                    foreach (Image pieza in _piezasColor2)
                    {
                        Destroy(pieza);
                    }
                    _piezasColor2 = new List<Image>();
                    foreach (Vector3Serializable v3s in piezasColor2)
                    {
                        GameObject pieza = Instantiate(PiezaPrefabColor2, GrupoPiezasColor2.transform, false);
                        pieza.transform.position = new Vector3(v3s.x, v3s.y, v3s.z);
                        _piezasColor2.Add(pieza.GetComponent<Image>());
                    }
                }
                if (sceneSave.dictVector3 != null && sceneSave.dictVector3.TryGetValue("_listDorsos", out List<Vector3Serializable> listDorsos))
                {
                    foreach (GameObject dorso in _listDorsos)
                    {
                        Destroy(dorso);
                    }
                    _listDorsos = new List<GameObject>();
                    foreach (Vector3Serializable v3s in listDorsos)
                    {
                        GameObject dorso = Instantiate(DorsoPrefab, DisplayDorsosUI.transform, false);
                        dorso.transform.position = new Vector3(v3s.x, v3s.y, v3s.z);
                        _listDorsos.Add(dorso);
                    }
                }
                if (sceneSave.intDictionary != null && sceneSave.intDictionary.TryGetValue("_numPiezasColor1", out int numPiezasColor1))
                {
                    _numPiezasColor1 = numPiezasColor1;
                }
                if (sceneSave.intDictionary != null && sceneSave.intDictionary.TryGetValue("_numPiezasColor2", out int numPiezasColor2))
                {
                    _numPiezasColor2 = numPiezasColor2;
                }
            }
        }
    }

    public void IsaveableStoreScene(string sceneName)
    {
        //
    }

    public void IsaveableRestoreScene(string sceneName)
    {
        //
    }
}
