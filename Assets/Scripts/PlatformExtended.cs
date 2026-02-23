using UnityEngine;

// Platform tiplerini belirten sinif / Enum defining platform types
public enum PlatformType
{
    Normal,
    Moving,
    Fragile, // Kirilan / Fragile
    HighJump // 10 kat ziplatan guclendirici / 10x jump power-up
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
    [Tooltip("Kameranin ne kadar asagisinda kalirsa bu platform yok edilsin? / How far below the camera should this platform be destroyed?")]
    public float destroyDistance = 7f;

    void Start()
    {
        startPosition = transform.position;

        // Karakterin cok hizli yukari firladiginda tavanlara (diger platformlara) alttan carpip 
        // fizik motorunu bug'a sokmasini (pir pir) engellemek icin Tek Yonlu (One Way) sistemini otomatik kurar:
        // Automatically sets up the One Way system to prevent the character from hitting ceilings from below and breaking the physics engine:
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.usedByEffector = true; // Effector'u kullanmasina izin ver / Allow it to use Effector
        }

        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        if (effector == null)
        {
            // Eger prefabda yoksa otomatik ekler / Automatically add if not in prefab
            effector = gameObject.AddComponent<PlatformEffector2D>();
        }
        effector.useOneWay = true; // Sadece yukaridan asagi dusenleri tutar, alttan gidenleri icinden gecirir / Catch objects falling from above, let objects from below pass through
    }

    void Update()
    {
        // 1- Eger bu hareketli (Moving) bir platformsa saga sola gitsin
        // 1- If this is a moving platform, move left and right
        if (type == PlatformType.Moving)
        {
            float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        // 2- Ekranda cok asagida kaldiysa yok et (Optimizasyon)
        // 2- Destroy if it falls too far below the screen (Optimization)
        if (Camera.main != null && Camera.main.transform.position.y - transform.position.y > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Karakter platforma sadece yukaridan dusuyorsa carpabilir
        // Player can only collide if falling from above
        if (collision.relativeVelocity.y <= 0f)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Ziplama gucunu uygula / Apply jump force
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 velocity = rb.linearVelocity;
                    velocity.y = jumpForce;
                    rb.linearVelocity = velocity;
                }

                // 3- Eger platform kirilan tipteyse (Fragile), ziplandiginda kendini yok et
                // 3- If platform is fragile, destroy itself upon jumping
                if (type == PlatformType.Fragile)
                {
                    // Isterseniz buraya bir kirilma efekti, parcacik (particle system) veya ses eklenebilir
                    // A breaking effect, particle system or sound can be added here if desired
                    Destroy(gameObject);
                }
            }
        }
    }
}
