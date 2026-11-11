using System.Collections;
using UnityEngine;

public class ControlCortina : MonoBehaviour
{
    [Header("Configuración Cortina")]
    public Transform rejaMetalica;
    public float distanciaSubida = 15f;
    public float velocidad = 5f;

    [Header("Elementos UI")]
    public GameObject botonAbrir;
    public GameObject panelCategorias;
    public GameObject botonRegresar;
    public GameObject panelFlechas; // Panel que contiene las flechas ◄ y ►

    [Header("Grupos de Bicicletas")]
    public GameObject grupoTradicional;
    public GameObject grupoMontana;
    public GameObject grupoCompetencia;

    private bool subiendo = false;
    private Vector3 posicionObjetivo;

    // Control del carrusel
    private GameObject grupoActivo;
    private int indiceBiciActual = 0;

    void Start()
    {
        if (rejaMetalica != null)
        {
            posicionObjetivo = rejaMetalica.position + Vector3.up * distanciaSubida;
        }

        if (panelCategorias != null) panelCategorias.SetActive(false);
        if (botonRegresar != null) botonRegresar.SetActive(false);
        if (panelFlechas != null) panelFlechas.SetActive(false);

        OcultarTodasLasBicis();
    }

    void Update()
    {
        if (subiendo && rejaMetalica != null)
        {
            rejaMetalica.position = Vector3.MoveTowards(rejaMetalica.position, posicionObjetivo, velocidad * Time.deltaTime);

            if (Vector3.Distance(rejaMetalica.position, posicionObjetivo) < 0.01f)
            {
                subiendo = false;
            }
        }
    }

    public void AbrirTienda()
    {
        subiendo = true;
        if (botonAbrir != null) botonAbrir.SetActive(false);
        if (panelCategorias != null) panelCategorias.SetActive(true);
        if (botonRegresar != null) botonRegresar.SetActive(false);
        if (panelFlechas != null) panelFlechas.SetActive(false);
    }

    // --- SELECCIÓN DE CATEGORÍAS ---
    public void MostrarTradicional()
    {
        ActivarCategoria(grupoTradicional);
    }

    public void MostrarMontana()
    {
        ActivarCategoria(grupoMontana);
    }

    public void MostrarCompetencia()
    {
        ActivarCategoria(grupoCompetencia);
    }

    private void ActivarCategoria(GameObject grupoSeleccionado)
    {
        OcultarTodasLasBicis();
        grupoActivo = grupoSeleccionado;

        if (grupoActivo != null)
        {
            grupoActivo.SetActive(true);
            indiceBiciActual = 0;
            ActualizarVisibilidadBicicletas();
        }

        if (panelCategorias != null) panelCategorias.SetActive(false);
        if (botonRegresar != null) botonRegresar.SetActive(true);
        if (panelFlechas != null) panelFlechas.SetActive(true);
    }

    // --- NAVEGACIÓN CON FLECHAS (CARRUSEL) ---
    public void SiguienteBici()
    {
        if (grupoActivo == null || grupoActivo.transform.childCount == 0) return;

        indiceBiciActual++;
        if (indiceBiciActual >= grupoActivo.transform.childCount)
        {
            indiceBiciActual = 0; // Regresa a la primera bici si llega al final
        }
        ActualizarVisibilidadBicicletas();
    }

    public void AnteriorBici()
    {
        if (grupoActivo == null || grupoActivo.transform.childCount == 0) return;

        indiceBiciActual--;
        if (indiceBiciActual < 0)
        {
            indiceBiciActual = grupoActivo.transform.childCount - 1; // Va a la última bici si retrocede desde la primera
        }
        ActualizarVisibilidadBicicletas();
    }

    private void ActualizarVisibilidadBicicletas()
    {
        if (grupoActivo == null) return;

        // Muestra únicamente la bicicleta del índice actual y oculta las demás
        for (int i = 0; i < grupoActivo.transform.childCount; i++)
        {
            Transform bici = grupoActivo.transform.GetChild(i);
            bici.gameObject.SetActive(i == indiceBiciActual);
        }
    }

    // --- REGRESAR AL MENÚ ---
    public void RegresarAMenu()
    {
        OcultarTodasLasBicis();
        if (panelCategorias != null) panelCategorias.SetActive(true);
        if (botonRegresar != null) botonRegresar.SetActive(false);
        if (panelFlechas != null) panelFlechas.SetActive(false);
    }

    private void OcultarTodasLasBicis()
    {
        if (grupoTradicional != null) grupoTradicional.SetActive(false);
        if (grupoMontana != null) grupoMontana.SetActive(false);
        if (grupoCompetencia != null) grupoCompetencia.SetActive(false);
    }
}