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
    [Tooltip("Kameranin ne kadar asagisinda olursak oyun bitecek? / How far below the camera should the game end?")]
    public float deathLineDistance = 6f;
    [Tooltip("Y koordinatini kacla carparak skor elde edecegiz? / By how much should we multiply the Y coordinate to get the score?")]
    public float scoreMultiplier = 10f;

    // Aktif oyuncu referansi / Active player reference
    [HideInInspector]
    public Transform player;

    private float maxScore = 0f;
    private int currentScoreInt = 0;
    private bool isGameOver = false;
    private bool isGameStarted = false;

    // Restart dedigimizde Main Menu'yu atlayip dogrudan oyuna baslamak icin
    // To skip Main Menu and start the game directly when we say Restart
    private static bool skipMainMenu = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Android'de FPS kilidini kaldir / Ekranin destekledigi en yuksek hiza (60, 90, 120 FPS vb.) cikar
        // Remove FPS lock on Android / Increase to maximum supported refresh rate (60, 90, 120 FPS etc.)
        Application.targetFrameRate = 120; // -1 yaparsak sinirsiz yapar ama telefonu isitabilir. 60 veya 120 idealdir. / -1 makes it unlimited but might heat the phone. 60 or 120 is ideal.
        QualitySettings.vSyncCount = 0; // VSync'i kapat ki kare hizi cihazin motoruna birakilsin / Turn off VSync to let the device handle framerate
        
        // Ekrana dokunulmadigi icin (sadece telefonu egerek oynandigindan) ekranin uyku moduna gecip kararmasini engelle
        // Prevent the screen from going to sleep / darkening since it's only played by tilting (no screen touching)
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
            // On normal start, keep Main Menu open, Game Over and In-Game UI closed
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            
            // En yuksek skoru PlayerPrefs'ten oku ve Main Menu'de goster
            // Read highest score from PlayerPrefs and display on Main Menu
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            if (mainMenuBestScoreText != null)
                mainMenuBestScoreText.text = "Best Score: " + bestScore.ToString();
        }
    }

    // Bu metodu Main Menu'deki 'Play' butonuna baglayacagiz
    // Bind this method to the 'Play' button in the Main Menu
    public void PlayGame()
    {
        isGameStarted = true;
        
        // Ekranda daha onceden acik kalmis olabilecek panelleri kapat!
        // Close any panels that might have been left open on the screen!
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // Skoru goster / Show score
        if (scoreText != null) scoreText.gameObject.SetActive(true);

        // Oyuncuyu prefabdan sahneye uret / Spawn player into the scene from prefab
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject p = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player = p.transform;
    }

    void Update()
    {
        if (!isGameStarted || isGameOver || player == null) return;

        // Skoru en yuksek Y pozisyonuna gore guncelle (Doodle Jump mantigi)
        // Update score based on the highest Y position (Doodle Jump logic)
        if (player.position.y > maxScore)
        {
            maxScore = player.position.y;
            currentScoreInt = Mathf.FloorToInt(maxScore * scoreMultiplier);
            if (scoreText != null) scoreText.text = "Score: " + currentScoreInt.ToString();
        }

        // Oyuncu kamera asagisindaki "olum cizgisine" duserse
        // If player falls below the "death line" under the camera
        if (Camera.main.transform.position.y - player.position.y > deathLineDistance)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        
        // Best Score kaydetme / guncelleme mantigi
        // Best Score save / update logic
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScoreInt > bestScore)
        {
            bestScore = currentScoreInt;
            PlayerPrefs.SetInt("BestScore", bestScore); // Cihaza kaydet / Save to device
            PlayerPrefs.Save();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (scoreText != null) scoreText.gameObject.SetActive(false); // Sol ustteki anlik skoru gizle / Hide current score at top left
            
            // Game Over ekranina skorlari yazdir / Write scores to Game Over screen
            if (gameOverScoreText != null)
                gameOverScoreText.text = "Score: " + currentScoreInt.ToString();
            
            if (gameOverBestScoreText != null)
                gameOverBestScoreText.text = "Best Score: " + bestScore.ToString();
        }
    }

    // Direkt olarak oyunu tekrar baslatir (Main Menu'yu atlar)
    // Starts the game directly (Skips Main Menu)
    public void RestartGame()
    {
        skipMainMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Ana Menuye doner / Returns to Main Menu
    public void GoToMainMenu()
    {
        skipMainMenu = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
