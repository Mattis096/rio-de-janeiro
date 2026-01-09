using UnityEngine;


[CreateAssetMenu(fileName = "GunData", menuName = "Weapons/GunData")]
public class GunData: ScriptableObject
{
    [Header("Gun Settings")]
    public string gunName = "Default Gun";  // Nom de l'arme
    public int maxAmmoPerMag = 30;               // Munition maximale
    public int startingAmmo = 30;                // Munition actuelle
    public float fireRate = 0.2f;          // Temps entre chaque tir
    public float reloadTime = 2f;          // Temps pour recharger
    public float range = 100f;             // Portée du tir
    public float HeadDMG = 30f;
    public float BodyDMG = 20f;
    public float LegsDMG = 10f;
    public float spread = 0.02f;

    
    public bool isReloading = false;      // Si le joueur recharge
    public bool isEquiped = false;

    [Header("Player Settings")]

    public Vector3 inHandPosition = Vector3.zero;
    public Vector3 inHandWeaponScale = Vector3.one;
    public Quaternion inHandWeaponRotation = Quaternion.identity;

    
}
