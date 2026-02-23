using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public Transform target;
    [Tooltip("Eger bu deger true ise, bu objeyi bir CameraTarget olarak kullanabilir ve Cinemachine'e takip ettirebilirsiniz.")]
    public bool isCinemachineTarget = false;

    void LateUpdate()
    {
        // Hedef bossa GameManager'dan bulmaya calis
        if (target == null && GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player;
        }

        if (target != null)
        {
            // Kamera veya Takip objesi sadece yukari dogru hareket eder
            if (target.position.y > transform.position.y)
            {
                Vector3 newPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
                transform.position = newPos;
            }
        }
    }
}
