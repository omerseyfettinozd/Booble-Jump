# Booble Jump 🚀

[![English](https://img.shields.io/badge/Language-English-blue)](#english-version) 
[![Türkçe](https://img.shields.io/badge/Dil-T%C3%BCrk%C3%A7e-red)](#türkçe-sürüm)

*Developed with Unity 2D*

---

<div id="english-version"></div>

## 🇬🇧 English Version

Booble Jump is an endless vertical platformer developed with Unity 2D, featuring classic "Doodle Jump" style mechanics.

### 🎮 Features

*   **Gyroscope (Tilt) Controls:** Simply tilt your device left and right to move the character on mobile devices. It offers smooth sensitivity, fully compatible with both the original accelerometer hardware and the new generation Input System sensors.
*   **Endless Level Generation:** As you ascend, platforms are generated with random algorithms. Platforms left below are instantly destroyed to save device memory (Object Pooling/Destroy optimization).
*   **Screen Wrap:** If you exit from the left of the screen, you instantly appear on the right, and vice versa!
*   **4 Different Platform Types:**
    *   **Normal:** Standard platforms you can bounce on.
    *   **Moving:** Challenging platforms that constantly move left and right.
    *   **Fragile:** Platforms you can only step on once; they break and disappear after you jump off them.
    *   **High Jump (Spring/Rocket):** Rare (5% chance) power-up platforms that launch the character much higher than usual!
*   **Score and Save System:** The highest point you reach determines your score. Thanks to the `PlayerPrefs` system, your "Best Score" is permanently saved on your device even if you close the game.
*   **Optimized Flow:** The 30 FPS power lock on the Android platform has been broken, providing a smooth (60/120 FPS) floating experience on modern phone screens. Anti-jitter interpolation has been added for fast-paced vertical jumps.

### 🛠️ How to Play / Test

*   **On Mobile Device (Android/iOS):** After installation, launch the game and tilt your phone left or right to steer. There is no need to touch the screen.
*   **On PC (Unity Editor):** While developing the game, you can control Booble with the directional (arrow) keys or the "A" - "D" keys in "Play" mode. The gyroscope script automatically allows your keyboard control.

### 🎨 Customization

All the magic numbers and ratios of the game (Visual tolerance margins, platform spawn probabilities, game over height, jump force, etc.) can be changed directly via `LevelGenerator`, `GameManager`, and `PlayerController` in the Unity Inspector without opening the code, thanks to the fully bilingual tooltips provided in the scripts. In addition, all platform visuals can be converted into your own `Prefab` designs with a single click!

---

<div id="türkçe-sürüm"></div>

## 🇹🇷 Türkçe Sürüm

Booble Jump, klasik "Doodle Jump" tarzı mekaniklere sahip, Unity 2D ile geliştirilmiş sonsuz (endless) tarzda dikey bir platform oyunudur.

### 🎮 Özellikler

*   **Jiroskop (Tilt) Kontrolleri:** Mobil cihazlarda karakteri hareket ettirmek için telefonu sağa ve sola yatırmanız yeterlidir. Hem orijinal ivmeölçer donanımı hem de yeni nesil Input System sensörleriyle tam uyumlu, akıcı bir hassasiyete sahiptir.
*   **Sonsuz Seviye Üretimi:** Siz yukarı çıktıkça platformlar rastgele algoritmalarla üretilir, aşağıda kalan platformlar ise cihazın belleğini yormamak adına anında imha edilir (Object Pooling/Destroy).
*   **Screen Wrap (Ekran Kayması):** Ekranın solundan çıkarsanız sağından, sağından çıkarsanız solundan anında belirirsiniz!
*   **4 Farklı Platform Türü:**
    *   **Normal:** Üzerinden zıplayabileceğiniz standart platformlar.
    *   **Hareketli (Moving):** Sağa ve sola sürekli hareket eden zorlu platformlar.
    *   **Kırılan (Fragile):** Üzerine sadece bir kez basabileceğiniz, zıpladıktan sonra yok olan platformlar.
    *   **High Jump (Yay/Roket):** Karakteri normalden kat kat daha yükseğe fırlatan, nadir (%5 ihtimalli) güçlendirici platformlar!
*   **Skor ve Kayıt Sistemi:** Çıktığınız en yüksek nokta skorunuzu belirler. `PlayerPrefs` sistemi sayesinde oyunu kapatsanız bile "Best Score" (En İyi Skor) cihazınızda kalıcı olarak tutulur.
*   **Optimize Edilmiş Akış:** Android platformundaki 30 FPS güç limiti kırılarak, güncel telefon ekranlarında pürüzsüz (60/120 FPS) süzülme deneyimi sağlanmıştır. Çarpışma anındaki yüksek hız "titremesi" (Jitter) için interpolasyon eklenmiştir.

### 🛠️ Nasıl Oynanır / Test Edilir?

*   **Mobil Cihazda (Android/iOS):** Kurulum yapıldıktan sonra oyuna girin ve yönünüzü belirlemek için telefonunuzu sağa sola yatırın. Ekrana dokunmanıza gerek yoktur.
*   **PC (Unity Editor) Üzerinde:** Oyunu geliştirirken "Play" modunda yön (ok) tuşları veya "A" - "D" tuşları ile Booble'ı kontrol edebilirsiniz. Jiroskop otomatik olarak klavye kontrolünüze izin verecektir.

### 🎨 Özelleştirme

Oyunun tüm sihirli sayıları ve oranları (Görsel tolerans payları, platform çıkma ihtimalleri, oyun bitiş yüksekliği, zıplama gücü vs.) kodu açmaya gerek kalmadan doğrudan Unity Inspector'daki `LevelGenerator`, `GameManager` ve `PlayerController` üzerinden "Tooltipler" ile açıklanmış biçimde değiştirilebilmektedir. Ayrıca tüm platform görselleri tek bir tıkla kendi `Prefab` tasarımlarınıza dönüştürülebilir!
