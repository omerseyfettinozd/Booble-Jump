using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    
    [Header("Screen Wrap Settings")]
    [Tooltip("Ekranin disina ciktiginda diger taraftan belirmesini saglar / Wrap around screen when going out of bounds")]
    public bool enableScreenWrap = true;
    [Tooltip("Diger tarafa gecerkenki tolerans (Gorsel hatalari engeller) / Tolerance when wrapping (prevents visual glitches)")]
    public float wrapOffset = 0.2f;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private float screenWidthInUnits;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Cok hizli ziplamalar (Yay firlatmasi vb.) sirasinda kameranin 120 FPS
        // fizik motorunun 50 FPS calismasindan dogan ekran titremesi (pir pir) 
        // hatasini (Jitter) onleyen pürüzsüzlestirici kod:
        // Smoothing code to prevent screen jitter (pir pir) caused by camera at 120 FPS and physics at 50 FPS during fast jumps:
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // Calculate screen width in world units for screen wrapping
        // Ekran genisligini dunya birimine gore hesapla (Ekran kaymasi icin)
        float height = Camera.main.orthographicSize * 2;
        screenWidthInUnits = height * Camera.main.aspect;

        // Jiroskop (Ivmeolcer) Cihazini Yeni Sistem Yuzunden Manuel Acma
        // Manually enable Accelerometer device due to New Input System
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("Accelerometer aktif edildi! / Accelerometer enabled!");
        }
    }

    void Update()
    {
        moveInput = 0f;

        // 1. Yeni sistem PC/Klavye Controller
        // 1. New System PC/Keyboard Controller
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }

        // 2. Yeni system Dokunmatik Ekran / Fare Kontrolleri (İPTAL EDİLDİ - YERİNE JİROSKOP GELDİ)
        // 2. New System Touchscreen / Mouse Controls (CANCELLED - GYROSCOPE ADDED INSTEAD)
        /*
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            if (touchPos.x < Screen.width / 2f) moveInput = -1f;
            else if (touchPos.x > Screen.width / 2f) moveInput = 1f;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            if (mousePos.x < Screen.width / 2f) moveInput = -1f;
            else if (mousePos.x > Screen.width / 2f) moveInput = 1f;
        }
        */

        // 3. JİROSKOP (Accelerometer) Kontrolleri (Android/iOS İvme Sensörü)
        // 3. GYROSCOPE (Accelerometer) Controls (Android/iOS Fallback)
        float tiltAmount = 0f;
        
        // A) Eger YENİ Input System devrede ise oradan oku:
        // A) If NEW Input System is active, read from there:
        if (Accelerometer.current != null)
        {
            tiltAmount = Accelerometer.current.acceleration.ReadValue().x;
        }
        // B) Eger ESKİ Input devrede ise oradan oku (Güvenlik/Fallback):
        // B) If OLD Input is active, read from there (Safety/Fallback):
        else 
        {
            tiltAmount = Input.acceleration.x;
        }

        // Surtunme veya ufak el titremelerine karsi "Deadzone" eklenebilir ama Doodle'da genelde 
        // hassas hareket istenir. Sadece x eksenini carpalim:
        // Deadzone can be added for friction or slight hand tremors, but Doodle usually requires precise movement. Let's just multiply the x axis:
        
        // Eger Klaye/PC kontrollerinden herhangi bir tusa basilmadiysa, JIroskopu devreye sok
        // (Boylece bilgisayarda test ederken klavye tuslari calisir, telefonda jisroskop)
        // If no Keyboard/PC keys are pressed, enable Gyroscope
        // (This way keyboard works when testing on PC, gyroscope works on phone)
        if (moveInput == 0f)
        {
            // Eger hala yeterince saga sola hizli gitmiyorsa 2.5f'i 3f veya 4f yapabilirsin.
            // If it still doesn't go fast enough left/right, you can change 2.5f to 3f or 4f.
            moveInput = tiltAmount * 2.5f; 
        }
        
        // -1 (tam sol) ve +1 (tam sag) disina cikmasini engelle
        // Prevent going out of bounds of -1 (full left) and +1 (full right)
        moveInput = Mathf.Clamp(moveInput, -1f, 1f);

        // --- SPRITE FLIP (Yuzunu Donme) MANTIGI ---
        // --- SPRITE FLIP (Turning Face) LOGIC ---
        if (spriteRenderer != null)
        {
            if (moveInput < -0.05f)
            {
                spriteRenderer.flipX = true;  // Sola Baksın / Face Left
            }
            else if (moveInput > 0.05f)
            {
                spriteRenderer.flipX = false; // Sağa Baksın / Face Right
            }
        }

        // Screen Wrap mechanic
        // Ekrandan tasinca diger taraftan cikma mekanigi
        if (enableScreenWrap)
        {
            Vector2 screenPos = transform.position;
            float edge = (screenWidthInUnits / 2f) + wrapOffset;
            
            if (screenPos.x > edge)
            {
                screenPos.x = -edge;
                transform.position = screenPos;
            }
            else if (screenPos.x < -edge)
            {
                screenPos.x = edge;
                transform.position = screenPos;
            }
        }
    }

    void FixedUpdate()
    {
        // Apply horizontal velocity, keep current vertical velocity
        // Yatay hizi uygula, mevcut dikey hizi koru
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;
    }
}
