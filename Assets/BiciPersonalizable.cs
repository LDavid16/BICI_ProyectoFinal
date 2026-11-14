using UnityEngine;

public class BiciPersonalizable : MonoBehaviour
{
    [Header("Ficha Técnica Base")]
    public string nombreModelo = "Bici Montaña Pro";
    public string categoria = "Montaña";
    [TextArea(2, 4)]
    public string descripcion = "Cuadro de aluminio reforzado, doble suspensión y frenos de disco.";
    public float precioBase = 12500f;

    [Header("Escala Real (1:1 en el mundo real)")]
    public Vector3 escalaTamanoReal = new Vector3(1f, 1f, 1f);

    [Header("Piezas Modificables")]
    public Renderer[] renderersCuadro;
    public Transform llantaDelantera;
    public Transform llantaTrasera;

    [Header("Accesorios (Objetos hijos dentro de la bici)")]
    public GameObject objLuces;
    public GameObject objSalpicaderas;
    public GameObject objEstampas;

    [Header("Precios de Extras")]
    public float precioLuces = 450f;
    public float precioSalpicaderas = 380f;
    public float precioEstampas = 250f;
    public float precioLlantaAncha = 600f;

    [Header("Reglas de Compatibilidad del Cuadro")]
    [Tooltip("Ej: 1.15 para Competencia, 1.35 para Tradicional, 1.6 para Montaña")]
    public float anchoMaximoLlanta = 1.4f;
    public bool permiteSalpicaderas = true;

    private Color[] coloresOriginales;
    private Vector3 escalaOriginalDelantera;
    private Vector3 escalaOriginalTrasera;
    private Vector3 posicionOriginalBici;
    private Vector3 escalaOriginalBici;
    private Quaternion rotacionOriginalBici;
    private bool inicializado = false;

    public void GuardarEstadoInicial()
    {
        if (inicializado) return;

        if (renderersCuadro != null && renderersCuadro.Length > 0)
        {
            coloresOriginales = new Color[renderersCuadro.Length];
            for (int i = 0; i < renderersCuadro.Length; i++)
            {
                if (renderersCuadro[i] != null)
                    coloresOriginales[i] = renderersCuadro[i].material.color;
            }
        }

        if (llantaDelantera != null) escalaOriginalDelantera = llantaDelantera.localScale;
        if (llantaTrasera != null) escalaOriginalTrasera = llantaTrasera.localScale;

        posicionOriginalBici = transform.localPosition;
        escalaOriginalBici = transform.localScale;
        rotacionOriginalBici = transform.localRotation;
        inicializado = true;
    }

    public void ReiniciarBici()
    {
        GuardarEstadoInicial();

        if (renderersCuadro != null && coloresOriginales != null)
        {
            for (int i = 0; i < renderersCuadro.Length; i++)
            {
                if (renderersCuadro[i] != null)
                    renderersCuadro[i].material.color = coloresOriginales[i];
            }
        }

        if (llantaDelantera != null) llantaDelantera.localScale = escalaOriginalDelantera;
        if (llantaTrasera != null) llantaTrasera.localScale = escalaOriginalTrasera;

        if (objLuces != null) objLuces.SetActive(false);
        if (objSalpicaderas != null) objSalpicaderas.SetActive(false);
        if (objEstampas != null) objEstampas.SetActive(false);

        transform.localPosition = posicionOriginalBici;
        transform.localScale = escalaOriginalBici;
        transform.localRotation = rotacionOriginalBici;
    }
}