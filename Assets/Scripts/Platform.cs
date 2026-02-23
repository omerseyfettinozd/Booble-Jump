using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce = 10f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Karakter platforma sadece yukaridan dusuyorsa carpabilir
        if (collision.relativeVelocity.y <= 0f)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 velocity = rb.linearVelocity;
                    velocity.y = jumpForce;
                    rb.linearVelocity = velocity;
                }
            }
        }
    }

    void Update()
    {
        // Platform, kameranin epey altinda (orn. 7 birim) kaldiysa optimizasyon icin kendini yok etsin
        if (Camera.main != null && Camera.main.transform.position.y - transform.position.y > 7f)
        {
            Destroy(gameObject);
        }
    }
}
