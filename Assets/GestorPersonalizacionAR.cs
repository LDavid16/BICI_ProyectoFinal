using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorPersonalizacionAR : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject panelPersonalizado;
    public GameObject menuBotonesPrincipales;

    [Header("Subpaneles de Edición")]
    public GameObject subpanelManipulacion;
    public GameObject subpanelColor;
    public GameObject subpanelLlantas;
    public GameObject subpanelAccesorios;

    [Header("Script de Manipulación Táctil AR")]
    public ManipularBicicletaAR scriptManipulacionAR;

    [Header("Panel de Alerta (Incompatibilidad)")]
    public GameObject panelAlerta;
    public TextMeshProUGUI txtMensajeAlerta;
    public float duracionAlerta = 3.0f;

    [Header("Ficha Técnica y Precio en Pantalla")]
    public TextMeshProUGUI txtNombreBici; // Usado para mostrar toda la ficha en bloque limpio

    [Header("Selector de Color (Rueda Cromática)")]
    public Image imagenRuedaColor;

    private BiciPersonalizable biciActual;
    private float factorRodadaActual = 1.0f;
    private float factorAnchoActual = 1.0f;
    private float velocidadManipulacion = 0.05f;
    private Coroutine corrutinaAlerta;

    private void Start()
    {
        if (panelPersonalizado != null) panelPersonalizado.SetActive(false);
        if (panelAlerta != null) panelAlerta.SetActive(false);
        CerrarTodosLosSubpaneles();
    }

    // --- CONEXIÓN CON CONTROLCORTINA.CS ---
    public void IniciarPersonalizacion(GameObject biciSeleccionada)
    {
        if (biciSeleccionada == null) return;

        biciActual = biciSeleccionada.GetComponent<BiciPersonalizable>();

        if (biciActual != null)
        {
            biciActual.GuardarEstadoInicial();
            if (biciActual.objLuz1 != null) biciActual.objLuz1.SetActive(false);
            if (biciActual.objLuz2 != null) biciActual.objLuz2.SetActive(false);
            if (biciActual.objSalpicadera1 != null) biciActual.objSalpicadera1.SetActive(false);
            if (biciActual.objSalpicadera2 != null) biciActual.objSalpicadera2.SetActive(false);
        }

        factorRodadaActual = 1.0f;
        factorAnchoActual = 1.0f;

        if (panelPersonalizado != null) panelPersonalizado.SetActive(true);
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(true);
        if (panelAlerta != null) panelAlerta.SetActive(false);

        CerrarTodosLosSubpaneles();
        ActualizarFichaYPrecio();
    }

    public void SalirDePersonalizacion()
    {
        if (biciActual != null)
        {
            biciActual.ReiniciarBici();
            biciActual = null;
        }

        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        if (panelPersonalizado != null) panelPersonalizado.SetActive(false);
        if (panelAlerta != null) panelAlerta.SetActive(false);
        CerrarTodosLosSubpaneles();
    }

    public bool IntentarCerrarSubpanel()
    {
        bool habiaModoEdicionActivo =
            (subpanelManipulacion != null && subpanelManipulacion.activeSelf) ||
            (subpanelColor != null && subpanelColor.activeSelf) ||
            (subpanelLlantas != null && subpanelLlantas.activeSelf) ||
            (subpanelAccesorios != null && subpanelAccesorios.activeSelf) ||
            (menuBotonesPrincipales != null && !menuBotonesPrincipales.activeSelf);

        if (habiaModoEdicionActivo)
        {
            CerrarTodosLosSubpaneles();
            if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
            if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(true);
            return true;
        }
        return false;
    }

    // --- APERTURA DE SUBPANELES (OCULTA LOS BOTONES PRINCIPALES) ---
    public void AbrirSubpanelManipulacion()
    {
        CerrarTodosLosSubpaneles();
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelManipulacion != null) subpanelManipulacion.SetActive(true);
        if (scriptManipulacionAR != null && biciActual != null)
            scriptManipulacionAR.ActivarManipulacion(biciActual.gameObject);
    }

    public void AbrirSubpanelColor()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelColor != null) subpanelColor.SetActive(true);
    }

    public void AbrirSubpanelLlantas()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelLlantas != null) subpanelLlantas.SetActive(true);
    }

    public void AbrirSubpanelAccesorios()
    {
        CerrarTodosLosSubpaneles();
        if (scriptManipulacionAR != null) scriptManipulacionAR.DesactivarManipulacion();
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        if (subpanelAccesorios != null) subpanelAccesorios.SetActive(true);
    }

    private void CerrarTodosLosSubpaneles()
    {
        if (subpanelManipulacion != null) subpanelManipulacion.SetActive(false);
        if (subpanelColor != null) subpanelColor.SetActive(false);
        if (subpanelLlantas != null) subpanelLlantas.SetActive(false);
        if (subpanelAccesorios != null) subpanelAccesorios.SetActive(false);
    }

    // --- 1. COLOR DEL CUADRO ---
    public void SeleccionarColorDesdeUI()
    {
        if (biciActual == null || imagenRuedaColor == null) return;

        RectTransform rectTransform = imagenRuedaColor.rectTransform;
        Vector2 puntoLocal;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out puntoLocal))
        {
            Rect rect = rectTransform.rect;
            float normX = Mathf.Clamp01((puntoLocal.x - rect.x) / rect.width);
            float normY = Mathf.Clamp01((puntoLocal.y - rect.y) / rect.height);

            Texture2D tex = imagenRuedaColor.sprite.texture;
            if (tex != null && tex.isReadable)
            {
                Color colorElegido = tex.GetPixelBilinear(normX, normY);
                if (colorElegido.a > 0.1f)
                {
                    biciActual.AplicarColorCuadro(colorElegido);
                }
            }
        }
    }

    public void AplicarColorPredefinido(Color color)
    {
        if (biciActual != null) biciActual.AplicarColorCuadro(color);
    }

    public void CambiarColorCuadroDirecto(Color color)
    {
        if (biciActual != null) biciActual.AplicarColorCuadro(color);
    }

    // --- 2. LLANTAS (RODADA Y ANCHO) ---
    public void AumentarRodada()
    {
        if (biciActual == null) return;
        if (factorRodadaActual + 0.04f <= biciActual.rodadaMaximaLlanta)
        {
            factorRodadaActual += 0.04f;
            biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        }
        else
        {
            MostrarAlerta("Has alcanzado la rodada máxima permitida para este cuadro.");
        }
    }

    public void DisminuirRodada()
    {
        if (biciActual == null) return;
        if (factorRodadaActual - 0.04f >= 0.80f)
        {
            factorRodadaActual -= 0.04f;
            biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        }
    }

    public void AumentarAnchoLlanta()
    {
        if (biciActual == null) return;
        if (factorAnchoActual + 0.05f <= biciActual.anchoMaximoLlanta)
        {
            factorAnchoActual += 0.05f;
            biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        }
        else
        {
            MostrarAlerta("El ancho de llanta excede el espacio de la horquilla.");
        }
    }

    public void DisminuirAnchoLlanta()
    {
        if (biciActual == null) return;
        if (factorAnchoActual - 0.05f >= 0.75f)
        {
            factorAnchoActual -= 0.05f;
            biciActual.AplicarDimensionesLlantas(factorAnchoActual, factorRodadaActual);
        }
    }

    // --- 3. ACCESORIOS CON VALIDACIÓN DE CUADRO ---
    public void AlternarLuz1()
    {
        if (biciActual == null || biciActual.objLuz1 == null) return;
        biciActual.objLuz1.SetActive(!biciActual.objLuz1.activeSelf);
        ActualizarFichaYPrecio();
    }

    public void AlternarLuz2()
    {
        if (biciActual == null || biciActual.objLuz2 == null) return;
        biciActual.objLuz2.SetActive(!biciActual.objLuz2.activeSelf);
        ActualizarFichaYPrecio();
    }

    public void AlternarSalpicadera1()
    {
        if (biciActual == null || biciActual.objSalpicadera1 == null) return;

        if (biciActual.tipoCuadro != BiciPersonalizable.TipoCuadro.Tradicional)
        {
            MostrarAlerta("Por el tipo de cuadro no se recomienda agregar estas salpicaderas.");
            return;
        }

        biciActual.objSalpicadera1.SetActive(!biciActual.objSalpicadera1.activeSelf);
        ActualizarFichaYPrecio();
    }

    public void AlternarSalpicadera2()
    {
        if (biciActual == null || biciActual.objSalpicadera2 == null) return;

        if (biciActual.tipoCuadro == BiciPersonalizable.TipoCuadro.Tradicional)
        {
            MostrarAlerta("Por el tipo de cuadro no se recomienda agregar estas salpicaderas.");
            return;
        }

        biciActual.objSalpicadera2.SetActive(!biciActual.objSalpicadera2.activeSelf);
        ActualizarFichaYPrecio();
    }

    // --- 4. BOTÓN GUARDAR PNG Y ENVIAR A TIENDA ---
    public void GuardarImagenPNG()
    {
        StartCoroutine(CapturarYCompartirPNG());
    }

    private IEnumerator CapturarYCompartirPNG()
    {
        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(false);
        CerrarTodosLosSubpaneles();
        if (panelAlerta != null) panelAlerta.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D foto = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        foto.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        foto.Apply();

        byte[] bytes = foto.EncodeToPNG();
        Destroy(foto);

        string nombreArchivo = $"Cotizacion_BiciAR_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string rutaArchivo = Path.Combine(Application.temporaryCachePath, nombreArchivo);
        File.WriteAllBytes(rutaArchivo, bytes);

        if (menuBotonesPrincipales != null) menuBotonesPrincipales.SetActive(true);

        string resumenPedido = ConstruirResumenPedido();

        new NativeShare()
            .AddFile(rutaArchivo)
            .SetSubject("Cotización de Bicicleta Personalizada AR")
            .SetText(resumenPedido)
            .Share();
    }

    private string ConstruirResumenPedido()
    {
        if (biciActual == null)
            return "Hola, te envío el diseño de mi bicicleta personalizada desde BiciAR.";

        float total = biciActual.precioBase;
        string listaAccesorios = "";

        if (biciActual.objLuz1 != null && biciActual.objLuz1.activeSelf)
        {
            total += biciActual.precioLuz1;
            listaAccesorios += $"\n• Luz Frontal LED (+${biciActual.precioLuz1:N0} MXN)";
        }
        if (biciActual.objLuz2 != null && biciActual.objLuz2.activeSelf)
        {
            total += biciActual.precioLuz2;
            listaAccesorios += $"\n• Luz Trasera / Doble (+${biciActual.precioLuz2:N0} MXN)";
        }
        if (biciActual.objSalpicadera1 != null && biciActual.objSalpicadera1.activeSelf)
        {
            total += biciActual.precioSalpicadera1;
            listaAccesorios += $"\n• Salpicadera Clásica (+${biciActual.precioSalpicadera1:N0} MXN)";
        }
        if (biciActual.objSalpicadera2 != null && biciActual.objSalpicadera2.activeSelf)
        {
            total += biciActual.precioSalpicadera2;
            listaAccesorios += $"\n• Salpicadera Deportiva/MTB (+${biciActual.precioSalpicadera2:N0} MXN)";
        }

        if (string.IsNullOrEmpty(listaAccesorios))
            listaAccesorios = "\n• Sin accesorios extra";

        return $"¡Hola! Me gustaría cotizar y pedir esta bicicleta personalizada en AR:\n\n" +
               $"Modelo: {biciActual.nombreModelo} ({biciActual.categoria})\n" +
               $"Rodada: x{factorRodadaActual:F2} | Ancho llanta: x{factorAnchoActual:F2}\n" +
               $"Accesorios incluidos:{listaAccesorios}\n\n" +
               $"Total estimado: ${total:N2} MXN";
    }

    // --- 5. REINICIAR Y ACTUALIZACIÓN DE PRECIOS ---
    public void ReiniciarPersonalizacion()
    {
        if (biciActual != null)
        {
            biciActual.ReiniciarBici();
        }

        factorRodadaActual = 1.0f;
        factorAnchoActual = 1.0f;
        CerrarTodosLosSubpaneles();
        if (panelAlerta != null) panelAlerta.SetActive(false);
        ActualizarFichaYPrecio();
    }

    private void ActualizarFichaYPrecio()
    {
        if (biciActual == null) return;

        float total = biciActual.precioBase;
        if (biciActual.objLuz1 != null && biciActual.objLuz1.activeSelf) total += biciActual.precioLuz1;
        if (biciActual.objLuz2 != null && biciActual.objLuz2.activeSelf) total += biciActual.precioLuz2;
        if (biciActual.objSalpicadera1 != null && biciActual.objSalpicadera1.activeSelf) total += biciActual.precioSalpicadera1;
        if (biciActual.objSalpicadera2 != null && biciActual.objSalpicadera2.activeSelf) total += biciActual.precioSalpicadera2;

        // Ficha técnica unificada en un solo bloque con saltos de línea ordenados
        if (txtNombreBici != null)
        {
            txtNombreBici.text = $"<b>{biciActual.nombreModelo} ({biciActual.categoria})</b>\n" +
                                 $"{biciActual.descripcion}\n" +
                                 $"<b>Total: ${total:N2} MXN</b>";
        }
    }

    private void MostrarAlerta(string mensaje)
    {
        if (panelAlerta == null || txtMensajeAlerta == null) return;

        txtMensajeAlerta.text = mensaje;
        panelAlerta.SetActive(true);

        if (corrutinaAlerta != null) StopCoroutine(corrutinaAlerta);
        corrutinaAlerta = StartCoroutine(OcultarAlertaDespuesDeTiempo());
    }

    private IEnumerator OcultarAlertaDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionAlerta);
        if (panelAlerta != null) panelAlerta.SetActive(false);
    }
}