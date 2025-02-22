using UnityEngine;

public class DoubleJumpItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que el jugador tenga el tag "Player"
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.EnableDoubleJump(); // Activa el doble salto
                Destroy(gameObject); // Destruye el �tem despu�s de recogerlo
            }
        }
    }
}
