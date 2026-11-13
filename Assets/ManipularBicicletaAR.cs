using UnityEngine;

public class ManipularBicicletaAR : MonoBehaviour
{
    [Header("Ajustes de Manipulación AR")]
    public float velocidadRotacion = 150f;
    public float velocidadEscala = 0.5f;
    public float escalaMinima = 0.2f;
    public float escalaMaxima = 3.0f;

    private GameObject biciActual;
    private bool estaActivo = false;

    private Vector3 escalaInicial;
    private Quaternion rotacionInicial;
    private Vector3 ultimaPosicionMouse;

    void Update()
    {
        if (!estaActivo || biciActual == null) return;

        // ==========================================
        // 1. PRUEBAS EN PC / EDITOR (MOUSE Y RUEDA)
        // ==========================================
        if (Input.GetMouseButtonDown(0))
        {
            ultimaPosicionMouse = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - ultimaPosicionMouse;
            float rotacionY = delta.x * velocidadRotacion * 0.02f;
            biciActual.transform.Rotate(Vector3.up, -rotacionY, Space.World);
            ultimaPosicionMouse = Input.mousePosition;
        }

        // Zoom en PC con la rueda del mouse (Scroll Wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            AplicarEscala(scroll * velocidadEscala);
        }

        // ==========================================
        // 2. DISPOSITIVO MÓVIL (TOQUES TÁCTILES AR)
        // ==========================================
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float rotacionY = touch.deltaPosition.x * (velocidadRotacion * 0.005f);
                biciActual.transform.Rotate(Vector3.up, -rotacionY, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
            AplicarEscala(deltaMagnitudeDiff * 0.002f);
        }
    }

    private void AplicarEscala(float incremento)
    {
        Vector3 nuevaEscala = biciActual.transform.localScale + Vector3.one * incremento;
        nuevaEscala.x = Mathf.Clamp(nuevaEscala.x, escalaMinima, escalaMaxima);
        nuevaEscala.y = Mathf.Clamp(nuevaEscala.y, escalaMinima, escalaMaxima);
        nuevaEscala.z = Mathf.Clamp(nuevaEscala.z, escalaMinima, escalaMaxima);

        biciActual.transform.localScale = nuevaEscala;
    }

    public void ActivarManipulacion(GameObject bici)
    {
        biciActual = bici;
        if (biciActual != null)
        {
            escalaInicial = biciActual.transform.localScale;
            rotacionInicial = biciActual.transform.localRotation;
        }
        estaActivo = true;
    }

    public void DesactivarManipulacion()
    {
        if (biciActual != null)
        {
            biciActual.transform.localScale = escalaInicial;
            biciActual.transform.localRotation = rotacionInicial;
        }
        biciActual = null;
        estaActivo = false;
    }
}