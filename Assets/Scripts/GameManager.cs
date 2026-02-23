using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro destegi

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public GameObject playerPrefab;
    public Transform spawnPoint;

    [Header("UI References - In Game")]
    public TextMeshProUGUI scoreText;

    [Header("UI References - Main Menu")]
    public GameObject mainMenuPanel;
    public TextMeshProUGUI mainMenuBestScoreText;

    [Header("UI References - Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverBestScoreText;

    [Header("Game Settings")]
    [Tooltip("Kameranin ne kadar asagisinda olursak oyun bitecek?")]
    public float deathLineDistance = 6f;
    [Tooltip("Y koordinatini kacla carparak skor elde edecegiz?")]
    public float scoreMultiplier = 10f;

    // Aktif oyuncu referansi
    [HideInInspector]
    public Transform player;

    private float maxScore = 0f;
    private int currentScoreInt = 0;
    private bool isGameOver = false;
    private bool isGameStarted = false;

    // Restart dedigimizde Main Menu'yu atlayip dogrudan oyuna baslamak icin
    private static bool skipMainMenu = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Android'de FPS kilidini kaldir / Ekranin destekledigi en yuksek hiza (60, 90, 120 FPS vb.) cikar
        Application.targetFrameRate = 120; // -1 yaparsak sinirsiz yapar ama telefonu isitabilir. 60 veya 120 idealdir.
        QualitySettings.vSyncCount = 0; // VSync'i kapat ki kare hizi cihazin motoruna birakilsin
        
        // Ekrana dokunulmadigi icin (sadece telefonu egerek oynandigindan) ekranin uyku moduna gecip kararmasini engelle
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        if (skipMainMenu)
        {
            // Restart butonuna basilarak geldiysek menuyu gec
            skipMainMenu = false;
            PlayGame();
        }
        else
        {
            // Normal acilista Oyun basinda Main Menu acik, Game Over ve In-Game UI kapali olsun
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            
            // En yuksek skoru PlayerPrefs'ten oku ve Main Menu'de goster
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            if (mainMenuBestScoreText != null)
                mainMenuBestScoreText.text = "Best Score: " + bestScore.ToString();
        }
    }

    // Bu metodu Main Menu'deki 'Play' butonuna baglayacagiz
    public void PlayGame()
    {
        isGameStarted = true;
        
        // Ekranda daha onceden acik kalmis olabilecek panelleri kapat!
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // Skoru goster
        if (scoreText != null) scoreText.gameObject.SetActive(true);

        // Oyuncuyu prefabdan sahneye uret
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject p = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player = p.transform;
    }

    void Update()
    {
        if (!isGameStarted || isGameOver || player == null) return;

        // Skoru en yuksek Y pozisyonuna gore guncelle (Doodle Jump mantigi)
        if (player.position.y > maxScore)
        {
            maxScore = player.position.y;
            currentScoreInt = Mathf.FloorToInt(maxScore * scoreMultiplier);
            if (scoreText != null) scoreText.text = "Score: " + currentScoreInt.ToString();
        }

        // Oyuncu kamera asagisindaki "olum cizgisine" duserse
        if (Camera.main.transform.position.y - player.position.y > deathLineDistance)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        
        // Best Score kaydetme / guncelleme mantigi
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScoreInt > bestScore)
        {
            bestScore = currentScoreInt;
            PlayerPrefs.SetInt("BestScore", bestScore); // Cihaza kaydet
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (scoreText != null) scoreText.gameObject.SetActive(false); // Sol ustteki anlik skoru gizle
            
            // Game Over ekranina skorlari yazdir
            if (gameOverScoreText != null)
                gameOverScoreText.text = "Score: " + currentScoreInt.ToString();
            
            if (gameOverBestScoreText != null)
                gameOverBestScoreText.text = "Best Score: " + bestScore.ToString();
        }
    }

    // Direkt olarak oyunu tekrar baslatir (Main Menu'yu atlar)
    public void RestartGame()
    {
        skipMainMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Ana Menuye doner
    public void GoToMainMenu()
    {
        skipMainMenu = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
