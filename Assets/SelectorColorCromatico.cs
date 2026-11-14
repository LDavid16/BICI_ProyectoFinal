using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class SelectorColorCromatico : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Referencia al Gestor")]
    public GestorPersonalizacionAR gestorPersonalizacion;

    [Header("Indicador Visual (Opcional)")]
    public Image muestraColorActual;

    private Image imagenPaleta;
    private RectTransform rectTransform;
    private Texture2D texturaPaleta;

    void Awake()
    {
        imagenPaleta = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        if (imagenPaleta.sprite != null)
        {
            texturaPaleta = imagenPaleta.sprite.texture;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SeleccionarColorEnPunto(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SeleccionarColorEnPunto(eventData);
    }

    private void SeleccionarColorEnPunto(PointerEventData eventData)
    {
        if (texturaPaleta == null || gestorPersonalizacion == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 puntoLocal))
        {
            Rect rect = rectTransform.rect;
            float normX = Mathf.InverseLerp(rect.xMin, rect.xMax, puntoLocal.x);
            float normY = Mathf.InverseLerp(rect.yMin, rect.yMax, puntoLocal.y);

            Sprite sprite = imagenPaleta.sprite;
            int texX = Mathf.RoundToInt(sprite.textureRect.x + normX * sprite.textureRect.width);
            int texY = Mathf.RoundToInt(sprite.textureRect.y + normY * sprite.textureRect.height);

            texX = Mathf.Clamp(texX, 0, texturaPaleta.width - 1);
            texY = Mathf.Clamp(texY, 0, texturaPaleta.height - 1);

            Color colorTocado = texturaPaleta.GetPixel(texX, texY);

            // Ignorar zonas transparentes fuera del círculo
            if (colorTocado.a < 0.1f) return;

            if (muestraColorActual != null)
            {
                muestraColorActual.color = colorTocado;
            }

            gestorPersonalizacion.CambiarColorCuadroDirecto(colorTocado);
        }
    }
}