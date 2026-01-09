using System.Collections;
using UnityEngine;
using Mirror;
using System;
using Unity.Burst.CompilerServices;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEditor;
using Unity.VisualScripting;
using Mirror.BouncyCastle.Pkcs;
using System.Runtime.CompilerServices;


public class GunScript : NetworkBehaviour
{
    public GunData GunData;

    [Header("References")]
    public ParticleSystem muzzleFlash;     // Effet de flash de tir
    public GameObject impactEffectPrefab;  // Effet d'impact
    public AudioClip shotSound;           // Bruit du Tir
    public AudioSource shotSource;

    private float nextTimeToFire = 0f;     // Prochaine fois que l'arme peut tirer

    //tmp ref
    public GameObject playerView;
    public TrailRenderer trailRenderer;
    RaycastHit hit;

    public Vector3 handPos;
    public Vector3 weaponScale;

    public LayerMask targetLayer;

    public int startingAmmo = 0;
    public int currentMagAmmo = 0;

    public bool isReloading = false;

    public GameObject bulletHole;

    private void Awake()
    {
        startingAmmo = GunData.startingAmmo;
        currentMagAmmo = startingAmmo;
        handPos = GunData.inHandPosition;
        weaponScale = GunData.inHandWeaponScale;
    }

    public int GetCurrentAmmo()
    {
        return currentMagAmmo;
    }

    public int GetMaxAmmo()
    {
        return GunData.maxAmmoPerMag;
    }

    void Start()
    {
        transform.localPosition = Vector3.zero + GunData.inHandPosition;
        transform.localRotation = GunData.inHandWeaponRotation;
        transform.localScale= GunData.inHandWeaponScale;
        
        Debug.Log("Weapon : " + GunData.gunName + " mag ammo " + currentMagAmmo);
    }

    public string GetWeaponName()
    {
        return GunData.gunName;
    }

    public bool isCurrentlyEquiped()
    {
        return GunData.isEquiped;
    }
    public void setWeaponStatus(bool status)
    {
        GunData.isEquiped = status;
    }

    private PlayerMovement PlayerMovement;

    public void IT_Shot(GameObject _playerView, PlayerUI playerUI, PlayerMovement _playerMvmt)
    {
        playerView = _playerView;
        PlayerMovement = _playerMvmt;

        if (Time.time >= nextTimeToFire && currentMagAmmo > 0 && !isReloading)
        {
            nextTimeToFire = Time.time + GunData.fireRate;
            Shoot(playerUI);
        }
    }

    public void IT_Reload(PlayerUI playerUI)
    {
        StartCoroutine(Reload(playerUI));
    }

    public float recoilAmoutX;
    public float recoilAmoutY;
    public float currentRecoilXPos;
    public float currentRecoilYPos;
    [Range(0, 10f)] public float maxRecoilTime = 4;
    private float timePressed = 0;
    //public GameObject tmpPlayerRef;

    public void RecoilMath()
    {
        currentRecoilXPos = ((Random.value - .5f) / 2) * recoilAmoutX;
        currentRecoilYPos = ((Random.value - .5f) / 2) * (timePressed >= maxRecoilTime ? recoilAmoutY / 4 : recoilAmoutY );
        PlayerMovement.currentRecoilX += (currentRecoilXPos);
        PlayerMovement.currentRecoilY += Math.Abs(currentRecoilYPos);
    }


    private void Shoot(PlayerUI playerUI)
    {

        if (currentMagAmmo <= 0)
        {
            Debug.Log("No ammo!");
            timePressed = 0;
            return;
        }

        CmdHandleShotEvents(connectionToClient);
        RecoilMath();

        currentMagAmmo--;
        timePressed += Time.deltaTime;
        timePressed = timePressed >= maxRecoilTime ? maxRecoilTime : timePressed;

        //playerUI.setPlayerAmmo(this.GetComponent<GunScript>().GetCurrentAmmo(), this.GetComponent<GunScript>().GetMaxAmmo());

        Vector3 shootDirection = playerView.transform.forward;


        int layerMask = LayerMask.GetMask("Hitbox", "Map");


        if (Physics.Raycast(playerView.transform.position, shootDirection, out hit, GunData.range, layerMask))
        {
            CmdHandleBulletTrail(hit.point);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                Instantiate(bulletHole, hit.point + (hit.normal * .01f), Quaternion.FromToRotation(Vector3.up, hit.normal));
            }



            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                GameObject player = hit.collider.gameObject.GetComponent<PlayerBodyParts>().player;
                PlayerEvents playerEvents = player.GetComponent<PlayerEvents>();

                float dmg = 0;

                BodyParts bp = hit.collider.gameObject.GetComponent<PlayerBodyParts>().bodyPart;

                switch (bp)
                {
                    case BodyParts.Head:
                        dmg = GunData.HeadDMG;
                        break;
                    case BodyParts.Body:
                        dmg = GunData.BodyDMG;
                        break;
                    case BodyParts.Arm:
                        dmg = GunData.BodyDMG;
                        break;
                    case BodyParts.Leg:
                        dmg = GunData.LegsDMG;
                        break;
                }

                CmdDealDamage(player, dmg);

                //DamageField(playerEvents.GetPlayerName(), dmg);
            }
        }

    }

    // Network

    [Command]
    void CmdHandleShotEvents(NetworkConnectionToClient sender = null)
    {
        RpcHandleShotEvents();
    }

    [ClientRpc]
    void RpcHandleShotEvents()
    {
        muzzleFlash.Play();
        shotSource.PlayOneShot(shotSound);
    }


    [Command]
    void CmdHandleBulletTrail(Vector3 hitpoint)
    {
        RpcHandleBulletTrail(hitpoint);
    }

    [ClientRpc]
    void RpcHandleBulletTrail(Vector3 hitpoint)
    {
        TrailRenderer trail = Instantiate(trailRenderer, muzzleFlash.transform.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hitpoint));
    }

    [Command]
    public void CmdDealDamage(GameObject target, float dmg)
    {
        if (target.TryGetComponent<PlayerData>(out PlayerData playerData))
        {
            //playerData.TakeDamage(dmg);
        }
    }


    [Command]
    public void DamageField(string name, float dmg)
    {
        Debug.Log(name + " take " +  dmg + " damages");
    }

    // Couroutines

    
    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 Hit)  // Gere le deplacement du trailrender lors du tir
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, Hit, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = Hit;
        Destroy(trail.gameObject, trail.time);
    }

    private IEnumerator Reload(PlayerUI playerUI)           // Couroutine lors du reload
    {
        Debug.Log("Reloading...");
        isReloading = true;

        yield return new WaitForSeconds(GunData.reloadTime);

        currentMagAmmo = GunData.maxAmmoPerMag;
        isReloading = false;
        Debug.Log("Reloaded!");
        //playerUI.setPlayerAmmo((GunData.maxAmmo.ToString(), GunData.maxAmmo.ToString()));
        //playerUI.setPlayerAmmo(this.GetComponent<GunScript>().GetCurrentAmmo(), this.GetComponent<GunScript>().GetMaxAmmo());
    }
}
