using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlCortina : MonoBehaviour
{
    [Header("Configuración Cortina UI")]
    public RectTransform panelInicioCortina;
    public float velocidadSubida = 2500f;

    [Header("Elementos UI")]
    public GameObject botonAbrir;
    public GameObject visorAR;
    public GameObject panelCategorias;
    public GameObject botonRegresar;
    public GameObject panelFlechas;

    [Header("Grupos de Bicicletas")]
    public GameObject grupoTradicional;
    public GameObject grupoMontana;
    public GameObject grupoCompetencia;

    [Header("Script de Manipulación AR")]
    public ManipularBicicletaAR scriptManipulacionAR;

    [Header("Gestor de Personalización")]
    public GestorPersonalizacionAR gestorPersonalizacion;

    private bool subiendoCortina = false;
    private Vector2 posicionObjetivoUI;

    private GameObject grupoActivo;
    private int indiceBiciActual = 0;
    private bool biciSeleccionada = false;

    void Start()
    {
        if (panelInicioCortina != null)
        {
            panelInicioCortina.gameObject.SetActive(true);
            float alturaPantalla = panelInicioCortina.rect.height > 0 ? panelInicioCortina.rect.height : 2500f;
            posicionObjetivoUI = panelInicioCortina.anchoredPosition + new Vector2(0f, alturaPantalla + 200f);
        }

        if (visorAR != null) visorAR.SetActive(false);
        if (panelCategorias != null) panelCategorias.SetActive(false);
        if (botonRegresar != null) botonRegresar.SetActive(false);
        if (panelFlechas != null) panelFlechas.SetActive(false);

        if (gestorPersonalizacion != null)
        {
            gestorPersonalizacion.SalirDePersonalizacion();
        }

        OcultarTodasLasBicis();
    }

    void Update()
    {
        if (subiendoCortina && panelInicioCortina != null)
        {
            panelInicioCortina.anchoredPosition = Vector2.MoveTowards(
                panelInicioCortina.anchoredPosition,
                posicionObjetivoUI,
                velocidadSubida * Time.deltaTime
            );

            if (Vector2.Distance(panelInicioCortina.anchoredPosition, posicionObjetivoUI) < 1f)
            {
                subiendoCortina = false;
                panelInicioCortina.gameObject.SetActive(false);
            }
        }
    }

    public void AbrirTienda()
    {
        subiendoCortina = true;

        if (botonAbrir != null)
        {
            Button btn = botonAbrir.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }

        ActivarMenuCategorias();
    }

    private void ActivarMenuCategorias()
    {
        if (visorAR != null) visorAR.SetActive(true);
        if (panelCategorias != null)
        {
            panelCategorias.SetActive(true);
            foreach (Button b in panelCategorias.GetComponentsInChildren<Button>(true))
            {
                b.gameObject.SetActive(true);
                b.interactable = true;
            }
        }
        if (botonRegresar != null) botonRegresar.SetActive(false);
        if (panelFlechas != null) panelFlechas.SetActive(false);
    }

    // --- SELECCIÓN DE CATEGORÍAS ---
    public void MostrarTradicional() => ActivarCategoria(grupoTradicional);
    public void MostrarMontana() => ActivarCategoria(grupoMontana);
    public void MostrarCompetencia() => ActivarCategoria(grupoCompetencia);

    private void ActivarCategoria(GameObject grupoSeleccionado)
    {
        OcultarTodasLasBicis();
        grupoActivo = grupoSeleccionado;
        biciSeleccionada = false;

        if (scriptManipulacionAR != null)
        {
            scriptManipulacionAR.DesactivarManipulacion();
        }

        if (gestorPersonalizacion != null)
        {
            gestorPersonalizacion.SalirDePersonalizacion();
        }

        if (grupoActivo != null)
        {
            grupoActivo.SetActive(true);
            indiceBiciActual = 0;
            ActualizarVisibilidadBicicletas();
        }

        if (visorAR != null) visorAR.SetActive(false);
        if (panelCategorias != null) panelCategorias.SetActive(false);
        if (botonRegresar != null) botonRegresar.SetActive(true);
        if (panelFlechas != null) panelFlechas.SetActive(true);
    }

    // --- NAVEGACIÓN CON FLECHAS (CARRUSEL) ---
    public void SiguienteBici()
    {
        if (biciSeleccionada || grupoActivo == null || grupoActivo.transform.childCount == 0) return;

        indiceBiciActual++;
        if (indiceBiciActual >= grupoActivo.transform.childCount) indiceBiciActual = 0;
        ActualizarVisibilidadBicicletas();
    }

    public void AnteriorBici()
    {
        if (biciSeleccionada || grupoActivo == null || grupoActivo.transform.childCount == 0) return;

        indiceBiciActual--;
        if (indiceBiciActual < 0) indiceBiciActual = grupoActivo.transform.childCount - 1;
        ActualizarVisibilidadBicicletas();
    }

    private void ActualizarVisibilidadBicicletas()
    {
        if (grupoActivo == null) return;

        for (int i = 0; i < grupoActivo.transform.childCount; i++)
        {
            Transform bici = grupoActivo.transform.GetChild(i);
            bici.gameObject.SetActive(i == indiceBiciActual);
        }
    }

    // --- FUNCIÓN PARA EL BOTÓN SELECCIONAR ---
    public void SeleccionarBicicleta()
    {
        if (grupoActivo == null || grupoActivo.transform.childCount == 0) return;

        biciSeleccionada = true;

        if (panelFlechas != null) panelFlechas.SetActive(false);
        if (botonRegresar != null) botonRegresar.SetActive(true);

        Transform biciActiva = grupoActivo.transform.GetChild(indiceBiciActual);
        if (gestorPersonalizacion != null && biciActiva != null)
        {
            gestorPersonalizacion.IniciarPersonalizacion(biciActiva.gameObject);
        }
    }

    // --- REGRESAR ---
    public void RegresarAMenu()
    {
        // Nivel 1: Si había un subpanel abierto (Color, Llantas o Accesorios), primero cierra ese subpanel
        if (biciSeleccionada && gestorPersonalizacion != null && gestorPersonalizacion.IntentarCerrarSubpanel())
        {
            return;
        }

        // Nivel 2: Si estaba en el menú de los 6 botones, oculta personalización y regresa al carrusel
        if (biciSeleccionada)
        {
            biciSeleccionada = false;

            if (gestorPersonalizacion != null)
            {
                gestorPersonalizacion.SalirDePersonalizacion();
            }

            if (scriptManipulacionAR != null)
            {
                scriptManipulacionAR.DesactivarManipulacion();
            }

            if (panelFlechas != null) panelFlechas.SetActive(true);
            return;
        }

        // Nivel 3: Si estaba en el carrusel, regresa al menú de categorías
        OcultarTodasLasBicis();
        ActivarMenuCategorias();
    }

    private void OcultarTodasLasBicis()
    {
        if (grupoTradicional != null) grupoTradicional.SetActive(false);
        if (grupoMontana != null) grupoMontana.SetActive(false);
        if (grupoCompetencia != null) grupoCompetencia.SetActive(false);
    }
}