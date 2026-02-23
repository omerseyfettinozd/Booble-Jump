using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Prefabs")]
    [Tooltip("Normal ziplayabilecegin platform prefabiniz / Normal bouncy platform prefab")]
    public GameObject normalPlatformPrefab;
    [Tooltip("Saga sola hareket eden platform prefabiniz / Moving platform prefab")]
    public GameObject movingPlatformPrefab;
    [Tooltip("Ziplandiginda kirilip dusuren platform prefabiniz / Fragile breaking platform prefab")]
    public GameObject fragilePlatformPrefab;
    [Tooltip("Normalden 10 kat daha uzaga ziplatan (Yay/Roket) platform prefabiniz / 10x higher jumping (Spring/Rocket) platform prefab")]
    public GameObject highJumpPlatformPrefab;

    public int platformCount = 10;
    
    [Header("Spawn Settings")]
    [Tooltip("Platformlar arasindaki minimum dikey mesafe / Minimum vertical distance between platforms")]
    public float minY = 1f;
    [Tooltip("Platformlar arasindaki maksimum dikey mesafe / Maximum vertical distance between platforms")]
    public float maxY = 2.5f;
    
    [Tooltip("Platformlarin sag-sol yayilma limiti (Ekran kenarlarina uzaklik). Bu deger ekranin 1.5 kati vb. olabilir / Left-right spread limit. E.g. 1.5x screen width")]
    public float horizontalLimit = 2.5f;

    [Tooltip("Ilk platformun baslangic Y degeri / Initial platform start Y value")]
    public float startY = -4f;
    [Tooltip("İlk zemin icin yan yana basilacak platform sayisi / Number of platform copies side by side for the first floor")]
    public int basePlatformCount = 7;
    [Tooltip("Yan yana basilacak platformlarin aralarindaki yatay mesafe / Horizontal distance between starting platforms")]
    public float basePlatformSpacing = 1f;
    
    // levelWidth = ekran yari genisligi (Camera.main.aspect * Camera.main.orthographicSize eksi biraz pay)
    // levelWidth = screen half width
    private float levelWidth;
    private float highestPlatformY;
    private float lastPlatformX;

    void Start()
    {
        highestPlatformY = startY; 
        lastPlatformX = 0f; 

        // 1- Oyuncu ilk doğduğunda kesin düşmemesi için YAN YANA zemin oluşturalım
        // 1- Create floors SIDE BY SIDE so the player doesn't fall initially
        // Matematik: Eger 7 platform varsa tam ortadakinin (4. platformun) X degeri SIFIR olmalidir.
        // Math: If 7 platforms, middle (4th) should have X = 0.
        float startX = -basePlatformSpacing * ((basePlatformCount - 1) / 2f); 
        for (int j = 0; j < basePlatformCount; j++)
        {
            Vector3 basePos = new Vector3(startX + (j * basePlatformSpacing), highestPlatformY, 0);
            // Guvenlik kontrolu (eger prefab atanmadiysa bos referans hatasini engelle)
            // Safety check (prevent null reference if prefab is not assigned)
            GameObject prefabToSpawn = normalPlatformPrefab;
            if (prefabToSpawn == null) continue; // veya bir uyari yazdirilabilir
            
            GameObject basePlatform = Instantiate(prefabToSpawn, basePos, Quaternion.identity, transform);
            
            PlatformExtended pScript = basePlatform.GetComponent<PlatformExtended>();
            if (pScript != null) 
            {
                pScript.type = PlatformType.Normal;
                // Renk ayarlari kullanicinin tasarimina karismamasi icin kaldirildi
                // Color settings removed to avoid interfering with user's design
            }
        }

        // Geri kalanları üret (Yukarı doğru)
        // Spawn the rest (Upwards)
        for (int i = 0; i < platformCount - 1; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // 1- Eger su an uretilmis olan ILERIDEKI (en yukaridaki) platformlarin sayisi, 
        // kameranin algilayabilecegi belli bir mesafenin altina duserse yeni platformlar uret
        // 1- Spawn new ones if we are running low on prepared platforms ahead of the camera
        
        // Matematik: Kameranin gorusunun en ust noktasi + (platformCount * ortalama mesafe)
        // Math: Top camera view point + (platformCount * average distance)
        // Eger highestPlatformY (uretilmis son platform), olmasi gereken bu tepe degerinden kucukse uret!
        // If highestPlatformY is lower than this peak value, spawn!
        
        float desiredHighestY = Camera.main.transform.position.y + (platformCount * ((minY + maxY) / 2f));
        
        if (desiredHighestY > highestPlatformY)
        {
            // Eger karakter coooka hizli firladiysa eksik olan tum platformlari 
            // 1 kare icerisinde uretmek (Yetişebilmek) icin while dongusu ile uretimi sagliyoruz.
            // If the character shot up very fast, use while loop to spawn all missing platforms in 1 frame (Catching up).
            while (highestPlatformY < desiredHighestY)
            {
                SpawnPlatform();
            }
        }
    }

    void SpawnPlatform()
    {
        // Platformların birbirinin üstüne binmesini engellemek için Inspector ayarları kullanılır
        // Inspector settings are used to prevent platforms from overlapping
        float yDist = Random.Range(minY, maxY);
        
        Vector3 spawnPosition = new Vector3();
        spawnPosition.y = highestPlatformY + yDist;
        
        // Bir onceki platformla ayni "dikey hizada (X ekseninde)" cikmasini onlemek (Ust uste dusmeyi engeller)
        // Prevent spanning on the exact same vertical X align (prevents overlap)
        // Yeni X pozisyonunu bulana kadar (veya 10 denemeye ulasana kadar) rastgele X uretimi yapalim
        // Let's generate random X until a new one is found (or 10 attempts max)
        float newX = 0f;
        int attempts = 0;
        bool validPosition = false;
        
        while (!validPosition && attempts < 10)
        {
            newX = Random.Range(-horizontalLimit, horizontalLimit);
            // Eger yeni X pozisyonu, bir oncekinin 1.2 birim saginda veya solundaysa gecerli say (Hemen tepesinde cikmasin)
            // Validate if new X is +/-1.2 units from previous X (Don't spawn right above)
            if (Mathf.Abs(newX - lastPlatformX) > 1.2f)
            {
                validPosition = true;
            }
            attempts++;
        }
        
        spawnPosition.x = newX;
        lastPlatformX = newX; // Sonraki uretim icin bunu hafizada tut / Keep in memory for next spawn
        
        // 1- Hangi tur platform uretilecegini ihtimallere gore secelim
        // 1- Select platform type based on probabilities
        float randomVal = Random.Range(0f, 100f);
        GameObject prefabToSpawn = normalPlatformPrefab; // Varsayilan / Default
        PlatformType spawnType = PlatformType.Normal;
        
        if (randomVal > 95f) 
        {
            // %5 İhtimal (Yay/Roket - Cok yuksege ziplatan platform)
            // 5% Probability (Spring/Rocket - High bouncing platform)
            prefabToSpawn = highJumpPlatformPrefab;
            spawnType = PlatformType.HighJump;
        }
        else if (randomVal > 85f) 
        {
            // %10 İhtimal (Kirilan platform)
            // 10% Probability (Fragile platform)
            prefabToSpawn = fragilePlatformPrefab;
            spawnType = PlatformType.Fragile;
        }
        else if (randomVal > 65f)
        {
            // %20 İhtimal (Hareketli platform)
            // 20% Probability (Moving platform)
            prefabToSpawn = movingPlatformPrefab;
            spawnType = PlatformType.Moving;
        }

        // Guvenlik kontrolu (eger Inspector'dan o prefab atanmamissa çökmeyi önle, normali koy)
        // Safety check (if prefab isn't set from Inspector, prevent crash, set to normal)
        if (prefabToSpawn == null) prefabToSpawn = normalPlatformPrefab;
        if (prefabToSpawn == null) return; // Hala null ise methoddan cik / if still null return
        
        // 2- Secilen Ozel Prefab'i Uret
        // 2- Spawn selected Custom Prefab
        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
        
        // 3- Eger PlatformExtended scripti varsa tipini ve hareket ozelliklerini rastgele verelim (Renk atamalari kaldirildi)
        // 3- Set type and movement props randomly. (Coloring removed)
        PlatformExtended platformScript = newPlatform.GetComponent<PlatformExtended>();
        if (platformScript != null)
        {
            platformScript.type = spawnType;
            if (spawnType == PlatformType.Moving)
            {
                // Hareket hizini ve mesafesini rastgele belirle
                // Set movement speed and distance randomly
                platformScript.moveSpeed = Random.Range(1f, 3f);
                platformScript.moveDistance = Random.Range(1f, 2f); 
            }
        }

        highestPlatformY = spawnPosition.y;
    }
}
