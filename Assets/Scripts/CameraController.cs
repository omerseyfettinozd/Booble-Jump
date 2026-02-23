using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public Transform target;
    [Tooltip("Eger bu deger true ise, bu objeyi bir CameraTarget olarak kullanabilir ve Cinemachine'e takip ettirebilirsiniz. / If true, you can use this object as a CameraTarget and track it with Cinemachine.")]
    public bool isCinemachineTarget = false;

    void LateUpdate()
    {
        // Hedef bossa GameManager'dan bulmaya calis / Try to find the target from GameManager if it's null
        if (target == null && GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player;
        }

        if (target != null)
        {
            // Kamera veya Takip objesi sadece yukari dogru hareket eder / Camera or Tracking object only moves upwards
            if (target.position.y > transform.position.y)
            {
                Vector3 newPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
                transform.position = newPos;
            }
        }
    }
}
