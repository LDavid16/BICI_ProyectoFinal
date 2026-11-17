using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

public class GestorPersonalizacionAR : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject panelPersonalizado;
    public GameObject menuBotonesPrincipales;
    public GameObject canvasCompleto;

    [Header("Subpaneles de Opciones")]
    public GameObject subpanelManipulacion;
    public GameObject subpanelColor;
    public GameObject subpanelLlantas;
    public GameObject subpanelAccesorios;

    [Header("Textos de Panel_FichaTecnica")]
    public TextMeshProUGUI txtNombreBici;
    public TextMeshProUGUI txtDescripcionBici;
    public TextMeshProUGUI txtPrecioTotal;

    [Header("Panel de Advertencias / Notificaciones")]
    public GameObject panelAlerta;
    public TextMeshProUGUI txtMensajeAlerta;

    [Header("Script de Manipulación AR")]
    public ManipularBicicletaAR scriptManipulacionAR;

    private BiciPersonalizable biciActual;
    private float factorAnchoActual = 1.0f;
    private float factorRodadaActual = 1.0f;
    private bool subpanelAbierto = false;

    public void IniciarPersonalizacion(GameObject biciObj)
    {
        biciActual = biciObj.GetComponent<BiciPersonalizable>();
        if (biciActual != null)
        {
            biciActual.GuardarEstadoInicial();
        }

        factorAnchoActual = 1.0f;
        factorRodadaActual = 1.0f;
        CerrarTodosLosSubpaneles();

        if (panelPersonalizado != null) panelPersonalizado.SetActive(true);
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(true);
        if (panelAlerta != null) panelAlerta.SetActive(false);
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();

        ActualizarFichaTecnica();
    }

    public void SalirDePersonalizacion()
    {
        CerrarTodosLosSubpaneles();
        if (panelPersonalizado != null) panelPersonalizado.SetActive(false);
        if (panelAlerta != null) panelAlerta.SetActive(false);
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
    }

    // --- 1. BOTÓN MANIPULACIÓN ---
    public void ActivarModoManipulacion()
    {
        CerrarTodosLosSubpaneles();
        subpanelAbierto = true;
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelManipulacion != null) subpanelManipulacion.SetActive(true);

        if (scriptManipulacionAR != null && biciActual != null)
        {
            scriptManipulacionAR.ActivarManipulacion(biciActual.gameObject);
        }
    }

    // --- 2. BOTÓN COLOR (SOLO CUADRO) ---
    public void AbrirSubpanelColor()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        subpanelAbierto = true;
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelColor != null) subpanelColor.SetActive(true);
    }

    public void CambiarColorCuadro(string hexColor)
    {
        if (biciActual == null) return;

        if (ColorUtility.TryParseHtmlString(hexColor, out Color nuevoColor))
        {
            biciActual.AplicarColorCuadro(nuevoColor);
        }
    }

    public void CambiarColorCuadroDirecto(Color nuevoColor)
    {
        if (biciActual == null) return;
        biciActual.AplicarColorCuadro(nuevoColor);
    }

    // --- 3. BOTÓN LLANTAS (4 BOTONES: RODADA +/- Y ANCHO +/-) ---
    public void AbrirSubpanelLlantas()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        subpanelAbierto = true;
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelLlantas != null) subpanelLlantas.SetActive(true);
    }

    public void AumentarRodada()
    {
        if (biciActual == null) return;
        float nuevaRodada = factorRodadaActual + 0.08f;

        if (nuevaRodada > biciActual.rodadaMaximaLlanta)
        {
            MostrarAlerta($"¡Incompatible! Una rodada más grande chocaría con la horquilla del cuadro de {biciActual.categoria}.");
            return;
        }

        factorRodadaActual = nuevaRodada;
        biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        ActualizarFichaTecnica();
    }

    public void DisminuirRodada()
    {
        if (biciActual == null) return;
        float nuevaRodada = factorRodadaActual - 0.08f;

        if (nuevaRodada < 0.76f)
        {
            MostrarAlerta("¡Límite alcanzado! No se puede reducir más la rodada de la bicicleta.");
            return;
        }

        factorRodadaActual = nuevaRodada;
        biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        ActualizarFichaTecnica();
    }

    public void AumentarAnchoLlanta()
    {
        if (biciActual == null) return;
        float nuevoAncho = factorAnchoActual + 0.15f;

        if (nuevoAncho > biciActual.anchoMaximoLlanta)
        {
            MostrarAlerta($"¡Incompatible! Estas llantas son demasiado anchas para el cuadro de {biciActual.categoria}.");
            return;
        }

        factorAnchoActual = nuevoAncho;
        biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        ActualizarFichaTecnica();
    }

    public void DisminuirAnchoLlanta()
    {
        if (biciActual == null) return;
        float nuevoAncho = factorAnchoActual - 0.15f;

        if (nuevoAncho < 0.70f)
        {
            MostrarAlerta("¡Límite alcanzado! No se puede hacer más delgada la llanta.");
            return;
        }

        factorAnchoActual = nuevoAncho;
        biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        ActualizarFichaTecnica();
    }

    // --- 4. BOTÓN ACCESORIOS ---
    public void AbrirSubpanelAccesorios()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        subpanelAbierto = true;
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelAccesorios != null) subpanelAccesorios.SetActive(true);
    }

    public void AlternarLuces()
    {
        if (biciActual == null || biciActual.objLuces == null) return;
        biciActual.objLuces.SetActive(!biciActual.objLuces.activeSelf);
        ActualizarFichaTecnica();
    }

    public void AlternarSalpicaderas()
    {
        if (biciActual == null) return;

        if (!biciActual.permiteSalpicaderas)
        {
            MostrarAlerta($"¡Advertencia! Estas salpicaderas no funcionarán en este cuadro de {biciActual.categoria}.");
            return;
        }

        if (biciActual.objSalpicaderas != null)
        {
            biciActual.objSalpicaderas.SetActive(!biciActual.objSalpicaderas.activeSelf);
            ActualizarFichaTecnica();
        }
    }

    public void AlternarEstampas()
    {
        if (biciActual == null || biciActual.objEstampas == null) return;
        biciActual.objEstampas.SetActive(!biciActual.objEstampas.activeSelf);
        ActualizarFichaTecnica();
    }

    // --- BOTÓN REINICIAR ---
    public void ReiniciarBicicleta()
    {
        if (biciActual == null) return;
        biciActual.ReiniciarBici();
        factorAnchoActual = 1.0f;
        factorRodadaActual = 1.0f;
        ActualizarFichaTecnica();

        if (scriptManipulacionAR != null && subpanelManipulacion != null && subpanelManipulacion.activeSelf)
        {
            scriptManipulacionAR.ActivarManipulacion(biciActual.gameObject);
        }
    }

    // --- 5. BOTÓN GUARDAR PNG ---
    public void GuardarImagenPNG()
    {
        StartCoroutine(CapturarPNG());
    }

    private IEnumerator CapturarPNG()
    {
        if (canvasCompleto != null) canvasCompleto.SetActive(false);
        yield return new WaitForEndOfFrame();

        Texture2D foto = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        foto.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        foto.Apply();

        byte[] bytes = foto.EncodeToPNG();
        Destroy(foto);

        string nombre = $"BiciAR_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string ruta = Path.Combine(Application.persistentDataPath, nombre);
        File.WriteAllBytes(ruta, bytes);

        if (canvasCompleto != null) canvasCompleto.SetActive(true);
        MostrarAlerta($"¡Imagen PNG guardada!\n{nombre}");
    }

    private void ActualizarFichaTecnica()
    {
        if (biciActual == null) return;

        float total = biciActual.precioBase;
        if (biciActual.objLuces != null && biciActual.objLuces.activeSelf) total += biciActual.precioLuces;
        if (biciActual.objSalpicaderas != null && biciActual.objSalpicaderas.activeSelf) total += biciActual.precioSalpicaderas;
        if (biciActual.objEstampas != null && biciActual.objEstampas.activeSelf) total += biciActual.precioEstampas;
        if (factorAnchoActual > 1.05f || factorRodadaActual > 1.04f) total += biciActual.precioLlantaExtra;

        if (txtNombreBici != null) txtNombreBici.text = biciActual.nombreModelo;
        if (txtDescripcionBici != null) txtDescripcionBici.text = biciActual.descripcion;
        if (txtPrecioTotal != null) txtPrecioTotal.text = $"Precio: ${total:N2} MXN";
    }

    public bool IntentarCerrarSubpanel()
    {
        if (subpanelAbierto)
        {
            CerrarTodosLosSubpaneles();
            if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
            if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(true);
            return true;
        }
        return false;
    }

    private void CerrarTodosLosSubpaneles()
    {
        subpanelAbierto = false;
        if (subpanelManipulacion != null) subpanelManipulacion.SetActive(false);
        if (subpanelColor != null) subpanelColor.SetActive(false);
        if (subpanelLlantas != null) subpanelLlantas.SetActive(false);
        if (subpanelAccesorios != null) subpanelAccesorios.SetActive(false);
    }

    public void MostrarAlerta(string msg)
    {
        if (panelAlerta != null && txtMensajeAlerta != null)
        {
            txtMensajeAlerta.text = msg;
            panelAlerta.SetActive(true);
        }
    }

    public void CerrarAlerta()
    {
        if (panelAlerta != null) panelAlerta.SetActive(false);
    }
}