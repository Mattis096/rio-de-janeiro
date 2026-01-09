using Mirror;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SniperScript : BaseWeapon
{

    public GameObject player;
    public PlayerUI playerUI;
    public PlayerMovement playerMovement;
    public GameObject PlayerEyes;
    public GameObject TPS_Weapon;
    public GameObject SniperCam;
    public GameObject elems;
    public GameObject Lens;
    public LoadingManager LoadingManager;

    public Sprite Scope;


    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerUI = player.GetComponent<PlayerSetup>().PlayerUI;
        PlayerEyes = player.GetComponent<PlayerSetup>().PlayerEyes;
        LoadingManager = player.GetComponent<PlayerSetup>().Loader;

        InitWeapon();

        if (!_isTPSweapon)
        {
            transform.localPosition = SpawnPosition;
            transform.Rotate(Rotation.x, Rotation.y, Rotation.z);
            transform.localScale = Scale;
        }
        else
        {
            transform.localPosition = TPS_Position;
            transform.Rotate(Rotation.x, Rotation.y, Rotation.z);
            transform.localScale = TPS_Scale;
        }

    }
    RaycastHit h2;

    private void Update()
    {
        if(_isTPSweapon)
        {
            SniperCam.SetActive(false);
        }

        if (!_isTPSweapon)
        {
            Hold_ADS(_ads);
        }

        Vector3 origin = Vector3.zero;
        Vector3 direction = Vector3.zero;

        origin = SniperCam.transform.position;
        direction = SniperCam.transform.forward;
        
        if (Physics.Raycast(SniperCam.transform.position, direction, out h2, Mathf.Infinity))
        {
            Debug.DrawLine(origin, h2.point, Color.blue);
        }
    }

    public override void Shoot()
    {
        if (currentMag <= 0)
        {
            Debug.Log("No ammo!");
            timePressed = 0;
            return;
        }

        //RecoilMath();

        currentMag--;
        timePressed += Time.deltaTime;
        timePressed = timePressed >= _maxRecoilTime ? _maxRecoilTime : timePressed;

        playerUI.setPlayerAmmo(currentMag, _magSize);
        
        Vector3 shootDirection = PlayerEyes.transform.forward;
        //if (_ads)
        //{
        //    shootDirection = SniperCam.transform.forward;
        //}

        int layerMask = LayerMask.GetMask("Hitbox", "Map");


        if (Physics.Raycast(PlayerEyes.transform.position, shootDirection, out Hit, Mathf.Infinity, layerMask))
        {
            TPS_Weapon.GetComponent<SniperScript>().CmdHandleShotEvents();
            TPS_Weapon.GetComponent<SniperScript>().CmdHandleBulletTrail(Hit.point);

            HandleShotEvents(Hit.point);
            //CmdHandleBulletTrail(hit.point);

            if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                Instantiate(_bulletHolePrefab, Hit.point + (Hit.normal * .01f), Quaternion.FromToRotation(Vector3.up, Hit.normal));
            }

            if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
            {
                GameObject player = Hit.collider.gameObject.GetComponent<PlayerBodyParts>().player;
                PlayerEvents playerEvents = player.GetComponent<PlayerEvents>();

                float dmg = 0;

                BodyParts bp = Hit.collider.gameObject.GetComponent<PlayerBodyParts>().bodyPart;

                switch (bp)
                {
                    case BodyParts.Head:
                        dmg = _headDmg;
                        break;
                    case BodyParts.Body:
                        dmg = _bodyDmg;
                        break;
                    case BodyParts.Arm:
                        dmg = _armDmg;
                        break;
                    case BodyParts.Leg:
                        dmg = _legsDmg;
                        break;
                }

                HandleHitMarker();

                CmdDealDamage(player, dmg);
            }
        }
    }

    public void HandleHitMarker()
    {
        _shotSource.PlayOneShot(_hitMarkerSound);
    }

    [Command]
    public void CmdDealDamage(GameObject target, float dmg)
    {
        if (target.TryGetComponent<PlayerData>(out PlayerData playerData))
        {
            bool isDead = playerData.TakeDamage(dmg, player.GetComponent<PlayerData>().playerNameSynced);
            if (isDead)
            {
                //Debug.Log("PLAYER HEEEERREEEE DEAD");
                RpcAddKill();
            }
        }
    }

    [ClientRpc]
    public void RpcAddKill()
    {
        if (isLocalPlayer)
        {
            player.GetComponent<PlayerData>().KillCounter++;
        }
    }

    public void HandleShotEvents(Vector3 Hit)
    {
        _muzzleFlash.Play();
        _shotSource.PlayOneShot(_shotSound);

        TrailRenderer trail = Instantiate(_bulletTrail, _muzzleFlash.transform.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, Hit));

        //Play Sound Locally / Online
        // Play Bullet Trail / Online
    }

    public void HandleDamage(GameObject player, float damage)
    {
        //player.GetComponent<PlayerOnlineEvents>().POE_DealDamage(player, damage);
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 Hit)  // Gere le deplacement du trailrender lors du tir
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, Hit, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = Hit;
        Destroy(trail.gameObject, trail.time);
    }



    //public void RecoilMath()
    //{
    //    currentRecoilXPos = ((UnityEngine.Random.value - .5f) / 2) * _recoilAmountX;
    //    currentRecoilYPos = ((UnityEngine.Random.value - .5f) / 2) * (timePressed >= _maxRecoilTime ? _recoilAmountY / 4 : _recoilAmountY);
    //    playerMovement.currentRecoilX += (currentRecoilXPos);
    //    playerMovement.currentRecoilY += Math.Abs(currentRecoilYPos);
    //}
    public override void TryShoot()
    {
        Debug.Log("shoot from here : " + _weaponName);
        if (Time.time >= nextTimeToFire && currentMag > 0 && !isReloading)
        {
            nextTimeToFire = Time.time + _fireRate;
            Shoot();
        }
    }

    public override void TryShootTPS(Vector3 hitpoint)
    {
        if (Time.time >= nextTimeToFire && currentMag > 0 && !isReloading)
        {
            nextTimeToFire = Time.time + _fireRate;
            // Online Player Shoot Effect
            CmdHandleBulletTrail(hitpoint);
            CmdHandleShotEvents();
        }
    }

    [Command]
    void CmdHandleShotEvents(NetworkConnectionToClient sender = null)
    {
        RpcHandleShotEvents();
    }

    [ClientRpc]
    void RpcHandleShotEvents()
    {
        if (!isLocalPlayer)
        {
            _muzzleFlash.Play();
            _shotSource.PlayOneShot(_shotSound);
        }
    }

    [Command]
    void CmdHandleBulletTrail(Vector3 hitpoint)
    {
        RpcHandleBulletTrail(hitpoint);
    }

    [ClientRpc]
    void RpcHandleBulletTrail(Vector3 hitpoint)
    {
        if (!isLocalPlayer)
        {
            TrailRenderer trail = Instantiate(_bulletTrail, _muzzleFlash.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitpoint));
        }
    }

    public override IEnumerator TryReload()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        LoadingManager.timeToLoad = _reloadTime;
        LoadingManager.StartLoading();

        yield return new WaitForSeconds(_reloadTime);

        currentMag = _magSize;
        isReloading = false;
        Debug.Log("Reloaded!");
        playerUI.setPlayerAmmo(currentMag, _magSize);
    }

    public override void Reload()
    {
        StartCoroutine(TryReload());
    }

    public override void Hold_ADS(bool ads)
    {
        if (ads)
        {
            //PlayerEyes.GetComponent<Camera>().fieldOfView = Mathf.Lerp(PlayerEyes.GetComponent<Camera>().fieldOfView, 40, Time.deltaTime * 10);
            transform.localPosition = Vector3.Lerp(transform.localPosition, ADS_Position, Time.deltaTime * 10);
            Lens.GetComponent<Renderer>().material.color = Color.Lerp(Lens.GetComponent<Renderer>().material.color, new Color(0, 0, 0, 0), 10 * Time.deltaTime);
            
        }
        else
        {
            //PlayerEyes.GetComponent<Camera>().fieldOfView = Mathf.Lerp(PlayerEyes.GetComponent<Camera>().fieldOfView, 90, Time.deltaTime * 10);
            transform.localPosition = Vector3.Lerp(transform.localPosition, SpawnPosition, Time.deltaTime * 10);
            Lens.GetComponent<Renderer>().material.color = Color.Lerp(Lens.GetComponent<Renderer>().material.color, new Color(0, 0, 0, 1), 10 * Time.deltaTime);
        }
    }



}
