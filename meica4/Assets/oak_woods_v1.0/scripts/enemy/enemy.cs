using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    Transform pointA;
    [SerializeField]
    Transform pointB;
    [SerializeField]
    float velocity;
    [SerializeField]
    float detectionRange;
    private Transform target;
    private bool isPursuing = false;

    [Header("Ataque")]
    [SerializeField]
    int damage;
    [SerializeField]
    float timeBetweenAttacks;
    private float lastAttackTime;

    [Header("Vida")]
    [SerializeField]
    int maxLife;
    private int currentLife;

    [Header("Retroceso al recibir daño")]
    [SerializeField]
    float backwardForce; // 🔹 Fuerza del retroceso
    [SerializeField]
    float backwardDuration; // 🔹 Duración del retroceso

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;
    private Color originalColor; // 🔹 Guarda el color original del enemigo

    void Start()
    {
        target = pointA;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentLife = maxLife;
        originalColor = spriteRenderer.color; // 🔹 Guardamos el color original

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);

        if (playerDistance < detectionRange)
        {
            isPursuing = true;
            target = player;
        }
        else if (isPursuing)
        {
            isPursuing = false;
            target = (Vector3.Distance(transform.position, pointA.position) < Vector3.Distance(transform.position, pointB.position)) ? pointB : pointA;
        }

        animator.SetBool("Persiguiendo", isPursuing);
        MoverHaciaObjetivo();
    }

    void MoverHaciaObjetivo()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, velocity * Time.deltaTime);

        if (target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (target.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.2f && !isPursuing)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPursuing)
        {
            if (Time.time > lastAttackTime + timeBetweenAttacks)
            {
                Movimiento playerLife = other.GetComponent<Movimiento>();
                if (playerLife != null)
                {
                    Debug.Log("⚔️ Enemigo ha golpeado al jugador por " + damage + " de daño.");
                    playerLife.TakeDamage(damage);
                    lastAttackTime = Time.time;
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
        currentLife -= damage;
        Debug.Log("💥 Enemigo recibió " + damage + " de daño. Vida restante: " + currentLife);

        // 🔹 Aplicar retroceso
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Detiene cualquier movimiento actual
            rb.AddForce(attackDirection * backwardForce, ForceMode2D.Impulse);
        }

        // 🔹 Parpadeo blanco
        StartCoroutine(FlashWhite());

        if (currentLife <= 0)
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
