using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    
    [Header("Screen Wrap Settings")]
    [Tooltip("Ekranin disina ciktiginda diger taraftan belirmesini saglar")]
    public bool enableScreenWrap = true;
    [Tooltip("Diger tarafa gecerkenki tolerans (Gorsel hatalari engeller)")]
    public float wrapOffset = 0.2f;
    
    private Rigidbody2D rb;
    private float moveInput;
    private float screenWidthInUnits;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Calculate screen width in world units for screen wrapping
        float height = Camera.main.orthographicSize * 2;
        screenWidthInUnits = height * Camera.main.aspect;

        // Jiroskop (Ivmeolcer) Cihazini Yeni Sistem Yuzunden Manuel Acma
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("Accelerometer aktif edildi!");
        }
    }

    void Update()
    {
        moveInput = 0f;

        // 1. Yeni sistem PC/Klavye Controller
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }

        // 2. Yeni system Dokunmatik Ekran / Fare Kontrolleri (İPTAL EDİLDİ - YERİNE JİROSKOP GELDİ)
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
        float tiltAmount = 0f;
        
        // A) Eger YENİ Input System devrede ise oradan oku:
        if (Accelerometer.current != null)
        {
            tiltAmount = Accelerometer.current.acceleration.ReadValue().x;
        }
        // B) Eger ESKİ Input devrede ise oradan oku (Güvenlik/Fallback):
        else 
        {
            tiltAmount = Input.acceleration.x;
        }

        // Surtunme veya ufak el titremelerine karsi "Deadzone" eklenebilir ama Doodle'da genelde 
        // hassas hareket istenir. Sadece x eksenini carpalim:
        
        // Eger Klaye/PC kontrollerinden herhangi bir tusa basilmadiysa, JIroskopu devreye sok
        // (Boylece bilgisayarda test ederken klavye tuslari calisir, telefonda jisroskop)
        if (moveInput == 0f)
        {
            // Eger hala yeterince saga sola hizli gitmiyorsa 2.5f'i 3f veya 4f yapabilirsin.
            moveInput = tiltAmount * 2.5f; 
        }
        
        // -1 (tam sol) ve +1 (tam sag) disina cikmasini engelle
        moveInput = Mathf.Clamp(moveInput, -1f, 1f);

        // Screen Wrap mechanic
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
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;
    }
}
