# Booble Jump 🚀

Booble Jump, klasik "Doodle Jump" tarzı mekaniklere sahip, Unity 2D ile geliştirilmiş sonsuz (endless) tarzda dikey bir platform oyunudur. 

## 🎮 Özellikler

*   **Jiroskop (Tilt) Kontrolleri:** Mobil cihazlarda karakteri hareket ettirmek için telefonu sağa ve sola yatırmanız yeterlidir. Hem orijinal ivmeölçer donanımı hem de yeni nesil Input System sensörleriyle tam uyumlu, akıcı bir hassasiyete sahiptir.
*   **Sonsuz Seviye Üretimi:** Siz yukarı çıktıkça platformlar rastgele algoritmalarla üretilir, aşağıda kalan platformlar ise cihazın belleğini yormamak adına anında imha edilir (Object Pooling/Destroy).
*   **Screen Wrap (Ekran Kayması):** Ekranın solundan çıkarsanız sağından, sağından çıkarsanız solundan anında belirirsiniz!
*   **4 Farklı Platform Türü:**
    *   **Normal:** Üzerinden zıplayabileceğiniz standart platformlar.
    *   **Hareketli (Moving):** Sağa ve sola sürekli hareket eden zorlu platformlar.
    *   **Kırılan (Fragile):** Üzerine sadece bir kez basabileceğiniz, zıpladıktan sonra yok olan platformlar.
    *   **High Jump (Yay/Roket):** Karakteri normalden kat kat daha yükseğe fırlatan, nadir (%5 ihtimalli) güçlendirici platformlar!
*   **Skor ve Kayıt Sistemi:** Çıktığınız en yüksek nokta skorunuzu belirler. `PlayerPrefs` sistemi sayesinde oyunu kapatsanız bile "Best Score" (En İyi Skor) cihazınızda kalıcı olarak tutulur.
*   **Optimize Edilmiş Akış:** Android platformundaki 30 FPS güç limiti kırılarak, güncel telefon ekranlarında pürüzsüz (60/120 FPS) süzülme deneyimi sağlanmıştır.

## 🛠️ Nasıl Oynanır / Test Edilir?

*   **Mobil Cihazda (Android/iOS):** Kurulum yapıldıktan sonra oyuna girin ve yönünüzü belirlemek için telefonunuzu sağa sola yatırın. Ekrana dokunmanıza gerek yoktur.
*   **PC (Unity Editor) Üzerinde:** Oyunu geliştirirken "Play" modunda yön (ok) tuşları veya "A" - "D" tuşları ile Booble'ı kontrol edebilirsiniz. Jiroskop otomatik olarak klavye kontrolünüze izin verecektir.

## 🎨 Özelleştirme

Oyunun tüm sihirli sayıları ve oranları (Görsel tolerans payları, platform çıkma ihtimalleri, oyun bitiş yüksekliği, zıplama gücü vs.) kodu açmaya gerek kalmadan doğrudan Unity Inspector'daki `LevelGenerator`, `GameManager` ve `PlayerController` üzerinden "Tooltipler" ile açıklanmış biçimde değiştirilebilmektedir. Ayrıca tüm platform görselleri tek bir tıkla kendi `Prefab` tasarımlarınıza dönüştürülebilir!

---
*Developed with Unity 2D*
