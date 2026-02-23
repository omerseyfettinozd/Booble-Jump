using UnityEngine;

// Platform tiplerini belirten sinif
public enum PlatformType
{
    Normal,
    Moving,
    Fragile, // Kirilan
    HighJump // 10 kat zıplatan güçlendirici
}

public class PlatformExtended : MonoBehaviour
{
    [Header("Settings")]
    public float jumpForce = 10f;
    public PlatformType type = PlatformType.Normal;

    [Header("Moving Platform Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    private Vector3 startPosition;

    [Header("Optimization Settings")]
    [Tooltip("Kameranin ne kadar asagisinda kalirsa bu platform yok edilsin?")]
    public float destroyDistance = 7f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // 1- Eger bu hareketli (Moving) bir platformsa saga sola gitsin
        if (type == PlatformType.Moving)
        {
            float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        // 2- Ekranda cok asagida kaldiysa yok et (Optimizasyon)
        if (Camera.main != null && Camera.main.transform.position.y - transform.position.y > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Karakter platforma sadece yukaridan dusuyorsa carpabilir
        if (collision.relativeVelocity.y <= 0f)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Ziplama gucunu uygula
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 velocity = rb.linearVelocity;
                    velocity.y = jumpForce;
                    rb.linearVelocity = velocity;
                }

                // 3- Eger platform kirilan tipteyse (Fragile), ziplandiginda kendini yok et
                if (type == PlatformType.Fragile)
                {
                    // Isterseniz buraya bir kirilma efekti, parcacik (particle system) veya ses eklenebilir
                    Destroy(gameObject);
                }
            }
        }
    }
}
