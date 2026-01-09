using UnityEngine;
using System.Collections;
using Mirror.BouncyCastle.Asn1.Mozilla;
using Mirror;

public class BaseWeapon : NetworkBehaviour
{
    [Header("Weapon References")]
    public PlayerUI PlayerUI;
    public GameObject PlayerView;
    

    [Header("Weapon Elements")]
    public GameObject _handGrip;
    public GameObject _handGripADS;
    public AudioSource _shotSource;
    public TrailRenderer _bulletTrail;
    public ParticleSystem _muzzleFlash;

    [Header("Weapon Elements Data")]
    public AudioClip _shotSound;
    public AudioClip _hitMarkerSound;
    public GameObject _bulletHolePrefab;
    public Sprite _weaponSprite;

    [Header("Weapon Data")]
    public string _weaponName;
    public WeaponType WeaponType;
    public float _fireRate;
    public float _reloadTime;
    public int _magSize;
    public int _range;
    [Range(0, 3f)] public float _recoilAmountX;
    [Range(0, 3f)] public float _recoilAmountY;
    [Range(0, 5f)] public float _maxRecoilTime;
    public float _headDmg;
    public float _bodyDmg;
    public float _armDmg;
    public float _legsDmg;
    public bool _isTPSweapon;
    public bool _ads;

    [Header("Weapon Utility")]
    public Vector3 SpawnPosition;
    public Vector3 Scale;
    public Vector3 Rotation;
    public Vector3 ADS_Position;
    public Vector3 TPS_Position;
    public Vector3 TPS_Scale;

    [Header("Weapon Variables")]
    public RaycastHit Hit;
    public float nextTimeToFire;
    public int currentMag;
    public bool isReloading;
    public float currentRecoilXPos;
    public float currentRecoilYPos;
    public float timePressed;
    public void InitWeapon()
    {
        isReloading = false;
        currentMag = _magSize;
    }

    public virtual void Shoot() { }
    public virtual void Reload() { }
    public virtual void TryShoot() { }
    public virtual void TryShootTPS(Vector3 hitpoint) { }
    public virtual IEnumerator TryReload() { yield return null; }

    public virtual void Hold_ADS(bool ads) { }

}


