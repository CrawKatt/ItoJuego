using UnityEngine;

public class DoubleJumpItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            Movimiento player = other.GetComponent<Movimiento>();
            if (player != null)
            {
                player.EnableDoubleJump(); // Activa el doble salto
                Destroy(gameObject); // Destruye el ítem después de recogerlo
            }
        }
    }
}
