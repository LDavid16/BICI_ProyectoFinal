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
    [Tooltip("Deja en 0 si la pieza solo tiene 1 material. Si tiene varios (ej. Bike), pon el índice del cuadro.")]
    public int indiceMaterialCuadro = 0;
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
    public float precioLlantaExtra = 600f;

    [Header("Reglas de Compatibilidad del Cuadro")]
    [Tooltip("Ej: 1.15 para Competencia, 1.35 para Tradicional, 1.6 para Montaña")]
    public float anchoMaximoLlanta = 1.4f;
    [Tooltip("Ej: 1.10 para Tradicional/Competencia, 1.25 para Montaña")]
    public float rodadaMaximaLlanta = 1.15f;
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
                {
                    Material[] mats = renderersCuadro[i].materials;
                    int idx = Mathf.Clamp(indiceMaterialCuadro, 0, mats.Length - 1);
                    coloresOriginales[i] = mats[idx].color;
                }
            }
        }

        if (llantaDelantera != null) escalaOriginalDelantera = llantaDelantera.localScale;
        if (llantaTrasera != null) escalaOriginalTrasera = llantaTrasera.localScale;

        posicionOriginalBici = transform.localPosition;
        escalaOriginalBici = transform.localScale;
        rotacionOriginalBici = transform.localRotation;
        inicializado = true;
    }

    public void AplicarColorCuadro(Color nuevoColor)
    {
        GuardarEstadoInicial();
        if (renderersCuadro == null) return;

        for (int i = 0; i < renderersCuadro.Length; i++)
        {
            if (renderersCuadro[i] != null)
            {
                Material[] mats = renderersCuadro[i].materials;
                int idx = Mathf.Clamp(indiceMaterialCuadro, 0, mats.Length - 1);
                mats[idx].color = nuevoColor;
            }
        }
    }

    public void AplicarDimensionesLlantas(float factorAncho, float factorRodada)
    {
        GuardarEstadoInicial();

        if (llantaDelantera != null)
        {
            llantaDelantera.localScale = new Vector3(
                escalaOriginalDelantera.x * factorAncho,
                escalaOriginalDelantera.y * factorRodada,
                escalaOriginalDelantera.z * factorRodada
            );
        }

        if (llantaTrasera != null)
        {
            llantaTrasera.localScale = new Vector3(
                escalaOriginalTrasera.x * factorAncho,
                escalaOriginalTrasera.y * factorRodada,
                escalaOriginalTrasera.z * factorRodada
            );
        }
    }

    public void ReiniciarBici()
    {
        GuardarEstadoInicial();

        if (renderersCuadro != null && coloresOriginales != null)
        {
            for (int i = 0; i < renderersCuadro.Length; i++)
            {
                if (renderersCuadro[i] != null && i < coloresOriginales.Length)
                {
                    Material[] mats = renderersCuadro[i].materials;
                    int idx = Mathf.Clamp(indiceMaterialCuadro, 0, mats.Length - 1);
                    mats[idx].color = coloresOriginales[i];
                }
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