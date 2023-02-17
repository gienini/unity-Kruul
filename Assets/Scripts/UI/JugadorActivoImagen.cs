using UnityEngine;
using UnityEngine.UI;

public class JugadorActivoImagen : MonoBehaviour
{
    [SerializeField] private Sprite imagenJugador1 = null;
    [SerializeField] private Sprite imagenJugador2 = null;
    private Image _imageComponent;
    private void OnEnable()
    {
        EventHandler.EmpiezaFase1Event += EmpiezaFase1Event;
        EventHandler.JugadaHechaEvent += JugadaHechaEvent;
    }
    private void OnDisable()
    {
        EventHandler.EmpiezaFase1Event -= EmpiezaFase1Event;
        EventHandler.JugadaHechaEvent -= JugadaHechaEvent;
    }

    private void JugadaHechaEvent()
    {
        RefrescaTextos();
    }

    private void EmpiezaFase1Event()
    {
        RefrescaTextos();
    }

    private void RefrescaTextos()
    {
        if (_imageComponent == null)
        {
            _imageComponent = GetComponent<Image>();
        }
        if (PropiedadesCasillasManager.Instance.EsTurnoColor1)
        {
            _imageComponent.sprite = imagenJugador1;
        }else
        {
            _imageComponent.sprite = imagenJugador2;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _imageComponent = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
