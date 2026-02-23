using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Prefabs")]
    [Tooltip("Normal ziplayabilecegin platform prefabiniz")]
    public GameObject normalPlatformPrefab;
    [Tooltip("Saga sola hareket eden platform prefabiniz")]
    public GameObject movingPlatformPrefab;
    [Tooltip("Ziplandiginda kirilip dusuren platform prefabiniz")]
    public GameObject fragilePlatformPrefab;
    [Tooltip("Normalden 10 kat daha uzaga ziplatan (Yay/Roket) platform prefabiniz")]
    public GameObject highJumpPlatformPrefab;

    public int platformCount = 10;
    
    [Header("Spawn Settings")]
    [Tooltip("Platformlar arasindaki minimum dikey mesafe")]
    public float minY = 1f;
    [Tooltip("Platformlar arasindaki maksimum dikey mesafe")]
    public float maxY = 2.5f;
    
    [Tooltip("Platformlarin sag-sol yayilma limiti (Ekran kenarlarina uzaklik). Bu deger ekranin 1.5 kati vb. olabilir")]
    public float horizontalLimit = 2.5f;

    [Tooltip("Ilk platformun baslangic Y degeri")]
    public float startY = -4f;
    [Tooltip("İlk zemin icin yan yana basilacak platform sayisi")]
    public int basePlatformCount = 7;
    [Tooltip("Yan yana basilacak platformlarin aralarindaki yatay mesafe")]
    public float basePlatformSpacing = 1f;
    
    // levelWidth = ekran yari genisligi (Camera.main.aspect * Camera.main.orthographicSize eksi biraz pay)
    private float levelWidth;
    private float highestPlatformY;
    private float lastPlatformX;

    void Start()
    {
        highestPlatformY = startY; 
        lastPlatformX = 0f; 

        // 1- Oyuncu ilk doğduğunda kesin düşmemesi için YAN YANA zemin oluşturalım
        // Matematik: Eger 7 platform varsa tam ortadakinin (4. platformun) X degeri SIFIR olmalidir.
        float startX = -basePlatformSpacing * ((basePlatformCount - 1) / 2f); 
        for (int j = 0; j < basePlatformCount; j++)
        {
            Vector3 basePos = new Vector3(startX + (j * basePlatformSpacing), highestPlatformY, 0);
            // Guvenlik kontrolu (eger prefab atanmadiysa bos referans hatasini engelle)
            GameObject prefabToSpawn = normalPlatformPrefab;
            if (prefabToSpawn == null) continue; // veya bir uyari yazdirilabilir
            
            GameObject basePlatform = Instantiate(prefabToSpawn, basePos, Quaternion.identity, transform);
            
            PlatformExtended pScript = basePlatform.GetComponent<PlatformExtended>();
            if (pScript != null) 
            {
                pScript.type = PlatformType.Normal;
                // Renk ayarlari kullanicinin tasarimina karismamasi icin kaldirildi
            }
        }

        // Geri kalanları üret (Yukarı doğru)
        for (int i = 0; i < platformCount - 1; i++)
        {
            SpawnPlatform();
        }
    }

    void Update()
    {
        // 1- Eger su an uretilmis olan ILERIDEKI (en yukaridaki) platformlarin sayisi, 
        // kameranin algilayabilecegi belli bir mesafenin altina duserse yeni platformlar uret
        
        // Matematik: Kameranin gorusunun en ust noktasi + (platformCount * ortalama mesafe)
        // Eger highestPlatformY (uretilmis son platform), olmasi gereken bu tepe degerinden kucukse uret!
        
        float desiredHighestY = Camera.main.transform.position.y + (platformCount * ((minY + maxY) / 2f));
        
        if (highestPlatformY < desiredHighestY)
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        // Platformların birbirinin üstüne binmesini engellemek için Inspector ayarları kullanılır
        float yDist = Random.Range(minY, maxY);
        
        Vector3 spawnPosition = new Vector3();
        spawnPosition.y = highestPlatformY + yDist;
        
        // Bir onceki platformla ayni "dikey hizada (X ekseninde)" cikmasini onlemek (Ust uste dusmeyi engeller)
        // Yeni X pozisyonunu bulana kadar (veya 10 denemeye ulasana kadar) rastgele X uretimi yapalim
        float newX = 0f;
        int attempts = 0;
        bool validPosition = false;
        
        while (!validPosition && attempts < 10)
        {
            newX = Random.Range(-horizontalLimit, horizontalLimit);
            // Eger yeni X pozisyonu, bir oncekinin 1.2 birim saginda veya solundaysa gecerli say (Hemen tepesinde cikmasin)
            if (Mathf.Abs(newX - lastPlatformX) > 1.2f)
            {
                validPosition = true;
            }
            attempts++;
        }
        
        spawnPosition.x = newX;
        lastPlatformX = newX; // Sonraki uretim icin bunu hafizada tut
        
        // 1- Hangi tur platform uretilecegini ihtimallere gore secelim
        float randomVal = Random.Range(0f, 100f);
        GameObject prefabToSpawn = normalPlatformPrefab; // Varsayilan
        PlatformType spawnType = PlatformType.Normal;
        
        if (randomVal > 95f) 
        {
            // %5 İhtimal (Yay/Roket - Cok yuksege ziplatan platform)
            prefabToSpawn = highJumpPlatformPrefab;
            spawnType = PlatformType.HighJump;
        }
        else if (randomVal > 85f) 
        {
            // %10 İhtimal (Kirilan platform)
            prefabToSpawn = fragilePlatformPrefab;
            spawnType = PlatformType.Fragile;
        }
        else if (randomVal > 65f)
        {
            // %20 İhtimal (Hareketli platform)
            prefabToSpawn = movingPlatformPrefab;
            spawnType = PlatformType.Moving;
        }

        // Guvenlik kontrolu (eger Inspector'dan o prefab atanmamissa çökmeyi önle, normali koy)
        if (prefabToSpawn == null) prefabToSpawn = normalPlatformPrefab;
        if (prefabToSpawn == null) return; // Hala null ise methoddan cik
        
        // 2- Secilen Ozel Prefab'i Uret
        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
        
        // 3- Eger PlatformExtended scripti varsa tipini ve hareket ozelliklerini rastgele verelim (Renk atamalari kaldirildi)
        PlatformExtended platformScript = newPlatform.GetComponent<PlatformExtended>();
        if (platformScript != null)
        {
            platformScript.type = spawnType;
            if (spawnType == PlatformType.Moving)
            {
                // Hareket hizini ve mesafesini rastgele belirle
                platformScript.moveSpeed = Random.Range(1f, 3f);
                platformScript.moveDistance = Random.Range(1f, 2f); 
            }
        }

        highestPlatformY = spawnPosition.y;
    }
}
