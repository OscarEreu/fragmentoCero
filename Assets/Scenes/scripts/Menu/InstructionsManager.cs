using UnityEngine;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    public GameObject panelInstrucciones;

    public GameObject[] instrucciones; // Las 4 imágenes de instrucciones
    private int index = 0;

    public Button btnAtras;
    public Button btnAdelante;
    public Button btnCerrar;

    void Start()
    {
        panelInstrucciones.SetActive(false);
        MostrarInstruccion();

        btnAtras.onClick.AddListener(Anterior);
        btnAdelante.onClick.AddListener(Siguiente);
        btnCerrar.onClick.AddListener(Cerrar);
    }

    public void Abrir()
    {
        index = 0;
        panelInstrucciones.SetActive(true);
        MostrarInstruccion();
    }

    void MostrarInstruccion()
    {
        // Mostrar solo la instrucción actual
        for (int i = 0; i < instrucciones.Length; i++)
            instrucciones[i].SetActive(i == index);

        // Controlar flechas
        btnAtras.gameObject.SetActive(index > 0);
        btnAdelante.gameObject.SetActive(index < instrucciones.Length - 1);
    }

    void Siguiente()
    {
        if (index < instrucciones.Length - 1)
        {
            index++;
            MostrarInstruccion();
        }
    }

    void Anterior()
    {
        if (index > 0)
        {
            index--;
            MostrarInstruccion();
        }
    }

    void Cerrar()
    {
        panelInstrucciones.SetActive(false);
    }
}