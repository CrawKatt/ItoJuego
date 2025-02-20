using UnityEngine;
using UnityEngine.UI;

public class DoubleJumpUI : MonoBehaviour
{
    public static DoubleJumpUI instance; // Para acceder fácilmente desde el personaje

    private void Awake()
    {
        instance = this;  // Configurar singleton
        gameObject.SetActive(false);  // Ocultar icono al inicio
    }

    public void ShowIcon()
    {
        gameObject.SetActive(true); // Mostrar icono
    }
}
