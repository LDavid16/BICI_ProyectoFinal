using UnityEngine;

public class BiciPersonalizable : MonoBehaviour
{
    public enum TipoCuadro { Tradicional, Competencia, Montana }

    [Header("Información General")]
    public string nombreModelo = "Bicicleta";
    public string categoria = "Tradicional";
    public TipoCuadro tipoCuadro = TipoCuadro.Tradicional;
    [TextArea(2, 4)]
    public string descripcion = "Bicicleta personalizable";
    public float precioBase = 3500f;

    [Header("Partes para Color del Cuadro")]
    public Renderer[] renderersCuadro;

    [Header("Llantas y Límites")]
    public Transform ruedaDelantera;
    public Transform ruedaTrasera;
    public float rodadaMaximaLlanta = 1.16f;
    public float anchoMaximoLlanta = 1.30f;

    [Header("Accesorios 3D (Hijos de esta bici)")]
    public GameObject objLuz1;          // Accesorio1 (Permitido en todas)
    public GameObject objLuz2;          // Accesorio2 (Permitido en todas)
    public GameObject objSalpicadera1;  // Accesorio3 (Solo Tradicional)
    public GameObject objSalpicadera2;  // Accesorio4 (Solo Competencia y Montaña)

    [Header("Precios Extra por Accesorio")]
    public float precioLuz1 = 250f;
    public float precioLuz2 = 300f;
    public float precioSalpicadera1 = 200f;
    public float precioSalpicadera2 = 350f;

    private Color[] coloresIniciales;
    private Vector3 escalaInicialDelantera;
    private Vector3 escalaInicialTrasera;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    private Vector3 escalaInicialBici;
    private bool estadoGuardado = false;

    private void Awake()
    {
        if (objLuz1 != null) objLuz1.SetActive(false);
        if (objLuz2 != null) objLuz2.SetActive(false);
        if (objSalpicadera1 != null) objSalpicadera1.SetActive(false);
        if (objSalpicadera2 != null) objSalpicadera2.SetActive(false);
    }

    public void GuardarEstadoInicial()
    {
        if (estadoGuardado) return;

        posicionInicial = transform.localPosition;
        rotacionInicial = transform.localRotation;
        escalaInicialBici = transform.localScale;

        if (renderersCuadro != null && renderersCuadro.Length > 0)
        {
            coloresIniciales = new Color[renderersCuadro.Length];
            for (int i = 0; i < renderersCuadro.Length; i++)
            {
                if (renderersCuadro[i] != null)
                    coloresIniciales[i] = renderersCuadro[i].material.color;
            }
        }

        if (ruedaDelantera != null) escalaInicialDelantera = ruedaDelantera.localScale;
        if (ruedaTrasera != null) escalaInicialTrasera = ruedaTrasera.localScale;

        estadoGuardado = true;
    }

    public void AplicarColorCuadro(Color nuevoColor)
    {
        if (renderersCuadro == null) return;
        foreach (Renderer rend in renderersCuadro)
        {
            if (rend != null)
                rend.material.color = nuevoColor;
        }
    }

    public void AplicarDimensionesLlantas(float factorAncho, float factorRodada)
    {
        if (ruedaDelantera != null)
        {
            ruedaDelantera.localScale = new Vector3(
                escalaInicialDelantera.x * factorAncho,
                escalaInicialDelantera.y * factorRodada,
                escalaInicialDelantera.z * factorRodada
            );
        }

        if (ruedaTrasera != null)
        {
            ruedaTrasera.localScale = new Vector3(
                escalaInicialTrasera.x * factorAncho,
                escalaInicialTrasera.y * factorRodada,
                escalaInicialTrasera.z * factorRodada
            );
        }
    }

    public void ReiniciarBici()
    {
        if (estadoGuardado)
        {
            transform.localPosition = posicionInicial;
            transform.localRotation = rotacionInicial;
            transform.localScale = escalaInicialBici;

            if (renderersCuadro != null && coloresIniciales != null)
            {
                for (int i = 0; i < renderersCuadro.Length; i++)
                {
                    if (renderersCuadro[i] != null && i < coloresIniciales.Length)
                        renderersCuadro[i].material.color = coloresIniciales[i];
                }
            }

            if (ruedaDelantera != null) ruedaDelantera.localScale = escalaInicialDelantera;
            if (ruedaTrasera != null) ruedaTrasera.localScale = escalaInicialTrasera;
        }

        if (objLuz1 != null) objLuz1.SetActive(false);
        if (objLuz2 != null) objLuz2.SetActive(false);
        if (objSalpicadera1 != null) objSalpicadera1.SetActive(false);
        if (objSalpicadera2 != null) objSalpicadera2.SetActive(false);
    }
}