using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public Image mainImage; // La imagen grande
    public Sprite[] illustrations; // Las ilustraciones
    private int currentIndex = 0;

    void Start()
    {
        // Asegurarnos de que la primera imagen se muestre siempre al abrir el menú
        if (illustrations.Length > 0 && mainImage != null)
        {
            mainImage.sprite = illustrations[0];
        }
    }

    public void ChangeImage(int index)
    {
        if (index >= 0 && index < illustrations.Length)
        {
            currentIndex = index;
            mainImage.sprite = illustrations[currentIndex];
        }
    }

    public void NextImage()
    {
        ChangeImage((currentIndex + 1) % illustrations.Length);
    }

    public void PreviousImage()
    {
        ChangeImage((currentIndex - 1 + illustrations.Length) % illustrations.Length);
    }
}
