using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    GameObject projectilePrefab; // Prefab de la flecha
    [SerializeField]
    Transform firePoint; // Punto de disparo
    [SerializeField]
    float fireRate; // Tiempo entre disparos
    [SerializeField]
    float shootForce; // Fuerza de disparo

    private float nextFireTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= nextFireTime) // Dispara con "Espacio"
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Controla la cadencia de disparo
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("El prefab de la flecha no est� asignado en el Inspector.");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("El prefab de la flecha no tiene un Rigidbody2D. Revisa su configuraci�n.");
            return;
        }

        // Obtener la direcci�n basada en la orientaci�n del personaje
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 shootDirection = new Vector2(direction, 0);

        // Aplicar velocidad en la direcci�n correcta
        rb.velocity = shootDirection * shootForce;

        // Obtener la escala original de la flecha y ajustarla sin hacerla gigante
        Vector3 originalScale = projectilePrefab.transform.localScale;
        newProjectile.transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * direction, originalScale.y, originalScale.z);

        Destroy(newProjectile, 3f);
    }


}
