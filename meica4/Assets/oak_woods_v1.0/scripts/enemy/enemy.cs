using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 2f;
    public float rangoDeteccion = 5f;
    private Transform objetivo;
    private bool persiguiendo = false;

    [Header("Ataque")]
    public int daño = 10;
    public float tiempoEntreAtaques = 1.5f;
    private float tiempoUltimoAtaque = 0f;

    [Header("Vida")]
    public int vidaMaxima = 50;
    private int vidaActual;

    [Header("Retroceso al recibir daño")]
    public float retrocesoForce = 5f; // 🔹 Fuerza del retroceso
    public float retrocesoDuration = 0.2f; // 🔹 Duración del retroceso

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform jugador;
    private Color originalColor; // 🔹 Guarda el color original del enemigo

    void Start()
    {
        objetivo = puntoA;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vidaActual = vidaMaxima;
        originalColor = spriteRenderer.color; // 🔹 Guardamos el color original

        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
        }
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaJugador < rangoDeteccion)
        {
            persiguiendo = true;
            objetivo = jugador;
        }
        else if (persiguiendo)
        {
            persiguiendo = false;
            objetivo = (Vector3.Distance(transform.position, puntoA.position) < Vector3.Distance(transform.position, puntoB.position)) ? puntoB : puntoA;
        }

        animator.SetBool("Persiguiendo", persiguiendo);
        MoverHaciaObjetivo();
    }

    void MoverHaciaObjetivo()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);

        if (objetivo.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (objetivo.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Vector3.Distance(transform.position, objetivo.position) < 0.2f && !persiguiendo)
        {
            objetivo = (objetivo == puntoA) ? puntoB : puntoA;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && persiguiendo)
        {
            if (Time.time > tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                Movimiento vidaJugador = other.GetComponent<Movimiento>();
                if (vidaJugador != null)
                {
                    Debug.Log("⚔️ Enemigo ha golpeado al jugador por " + daño + " de daño.");
                    vidaJugador.TomarDano(daño);
                    tiempoUltimoAtaque = Time.time;
                }
                else
                {
                    Debug.Log("❌ No se encontró el script Movimiento en el jugador.");
                }
            }
        }
    }

    // ✅ Método para recibir daño con retroceso e iluminación blanca
    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        vidaActual -= damage;
        Debug.Log("💥 Enemigo recibió " + damage + " de daño. Vida restante: " + vidaActual);

        // 🔹 Aplicar retroceso
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Detiene cualquier movimiento actual
            rb.AddForce(attackDirection * retrocesoForce, ForceMode2D.Impulse);
        }

        // 🔹 Parpadeo blanco
        StartCoroutine(FlashWhite());

        if (vidaActual <= 0)
        {
            Die();
        }
    }

    // ✅ Corrige el parpadeo blanco para restaurar el color original
    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white; // 🔹 Se ilumina en blanco
        yield return new WaitForSeconds(0.1f); // 🔹 Tiempo breve
        spriteRenderer.color = originalColor; // 🔹 Se restaura al color original
    }

    // ✅ Método para morir
    private void Die()
    {
        Debug.Log("💀 El enemigo ha muerto.");
        animator.SetTrigger("Muerto");
        Destroy(gameObject, 0.5f);
    }
}
