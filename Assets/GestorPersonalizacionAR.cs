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
    private float factorGrosorActual = 1.0f;
    private bool subpanelAbierto = false;

    public void IniciarPersonalizacion(GameObject biciObj)
    {
        biciActual = biciObj.GetComponent<BiciPersonalizable>();
        if (biciActual != null)
        {
            biciActual.GuardarEstadoInicial();
        }

        factorGrosorActual = 1.0f;
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
        if (biciActual == null || biciActual.renderersCuadro == null) return;

        if (ColorUtility.TryParseHtmlString(hexColor, out Color nuevoColor))
        {
            foreach (Renderer r in biciActual.renderersCuadro)
            {
                if (r != null) r.material.color = nuevoColor;
            }
        }
    }

    public void CambiarColorCuadroDirecto(Color nuevoColor)
    {
        if (biciActual == null || biciActual.renderersCuadro == null) return;

        foreach (Renderer r in biciActual.renderersCuadro)
        {
            if (r != null) r.material.color = nuevoColor;
        }
    }

    // --- 3. BOTÓN LLANTAS ---
    public void AbrirSubpanelLlantas()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        subpanelAbierto = true;
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelLlantas != null) subpanelLlantas.SetActive(true);
    }

    public void CambiarGrosorLlantas(float nuevoFactor)
    {
        if (biciActual == null) return;

        if (nuevoFactor > biciActual.anchoMaximoLlanta)
        {
            MostrarAlerta($"¡Incompatible! Estas llantas ({nuevoFactor:F1}x) son demasiado anchas para el cuadro de {biciActual.categoria}.");
            return;
        }

        factorGrosorActual = nuevoFactor;

        if (biciActual.llantaDelantera != null)
            biciActual.llantaDelantera.localScale = new Vector3(factorGrosorActual, 1f + (factorGrosorActual - 1f) * 0.25f, 1f + (factorGrosorActual - 1f) * 0.25f);

        if (biciActual.llantaTrasera != null)
            biciActual.llantaTrasera.localScale = new Vector3(factorGrosorActual, 1f + (factorGrosorActual - 1f) * 0.25f, 1f + (factorGrosorActual - 1f) * 0.25f);

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

    // --- BOTÓN REINICIAR (DENTRO DE MANIPULACIÓN) ---
    public void ReiniciarBicicleta()
    {
        if (biciActual == null) return;
        biciActual.ReiniciarBici();
        factorGrosorActual = 1.0f;
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
        if (factorGrosorActual > 1.05f) total += biciActual.precioLlantaAncha;

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