using UnityEngine;
using UnityEngine.EventSystems;

public class ManipularBicicletaAR : MonoBehaviour
{
    [Header("Sensibilidad de Gestos")]
    public float velocidadRotacion = 0.35f;
    public float velocidadEscala = 0.004f;
    public float velocidadMovimiento = 0.0015f;

    [Header("Límites Proporcionales de Escala (respecto a su tamaño inicial)")]
    public float multiplicadorMinimo = 0.25f;
    public float multiplicadorMaximo = 8.0f;

    private GameObject objetoObjetivo;
    private Vector3 escalaBaseInicial;
    private bool manipulacionActiva = false;

    public void ActivarManipulacion(GameObject objetivo)
    {
        objetoObjetivo = objetivo;
        if (objetoObjetivo != null)
        {
            escalaBaseInicial = objetoObjetivo.transform.localScale;
        }
        manipulacionActiva = true;
    }

    public void DesactivarManipulacion()
    {
        manipulacionActiva = false;
        objetoObjetivo = null;
    }

    public void AplicarTamanoReal(Vector3 escalaReal)
    {
        if (objetoObjetivo == null) return;
        objetoObjetivo.transform.localScale = escalaReal;
    }

    void Update()
    {
        if (!manipulacionActiva || objetoObjetivo == null) return;

        Transform refPadre = objetoObjetivo.transform.parent != null
            ? objetoObjetivo.transform.parent
            : objetoObjetivo.transform;

        // 1 DEDO: Girar libremente en 360° (horizontal y vertical respecto al ImageTarget)
        if (Input.touchCount == 1)
        {
            Touch toque = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(toque.fingerId))
                return;

            if (toque.phase == TouchPhase.Moved)
            {
                float giroHorizontal = -toque.deltaPosition.x * velocidadRotacion;
                float giroVertical = toque.deltaPosition.y * velocidadRotacion;

                objetoObjetivo.transform.Rotate(refPadre.up, giroHorizontal, Space.World);
                objetoObjetivo.transform.Rotate(refPadre.right, giroVertical, Space.World);
            }
        }
        // 2 DEDOS: Pellizcar para escalar proporcionalmente Y arrastrar juntos para mover
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (EventSystem.current != null &&
               (EventSystem.current.IsPointerOverGameObject(t0.fingerId) ||
                EventSystem.current.IsPointerOverGameObject(t1.fingerId)))
                return;

            // A) Escala proporcional suave (funciona igual sin importar si el FBX mide 0.01 o 10)
            Vector2 prevPos0 = t0.position - t0.deltaPosition;
            Vector2 prevPos1 = t1.position - t1.deltaPosition;

            float distPrevia = (prevPos0 - prevPos1).magnitude;
            float distActual = (t0.position - t1.position).magnitude;
            float diferencia = distActual - distPrevia;

            float factorCambio = 1f + (diferencia * velocidadEscala);
            factorCambio = Mathf.Clamp(factorCambio, 0.85f, 1.15f);

            Vector3 candidata = objetoObjetivo.transform.localScale * factorCambio;
            float ratioActual = candidata.x / Mathf.Max(0.00001f, escalaBaseInicial.x);
            float ratioClamped = Mathf.Clamp(ratioActual, multiplicadorMinimo, multiplicadorMaximo);

            objetoObjetivo.transform.localScale = escalaBaseInicial * ratioClamped;

            // B) Mover / Trasladar sobre el plano del ImageTarget (Pan con 2 dedos)
            Vector2 deltaPromedio = (t0.deltaPosition + t1.deltaPosition) * 0.5f;
            Vector3 desplazamiento = new Vector3(deltaPromedio.x * velocidadMovimiento, 0f, deltaPromedio.y * velocidadMovimiento);
            objetoObjetivo.transform.localPosition += desplazamiento;
        }
    }
}