using Mirror;
using UnityEngine;

public class KnifeWeapon : BaseWeapon
{
    public GameObject Player;

    public GameObject PlayerEyes;

    public GameObject TPS_Weapon;
    public float knifeCooldown = 0.4f;

    private void Start()
    {
        PlayerEyes = Player.GetComponent<PlayerSetup>().PlayerEyes;
        _fireRate = knifeCooldown;

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

    public override void TryShoot()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + _fireRate;
            Shoot();
        }
    }

    public override void Shoot()
    {
        Vector3 shootDirection = PlayerEyes.transform.forward;


        int layerMask = LayerMask.GetMask("Hitbox", "Map");


        if (Physics.Raycast(PlayerEyes.transform.position, shootDirection, out Hit, _range, layerMask))
        {
            Debug.Log("here");

            TPS_Weapon.GetComponent<KnifeWeapon>().CmdHandleKnifeEvents();

            HandleKnifeEvents();

            if (Hit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                // Maybe instantiate little knife effect on walls
                //Instantiate(_bulletHolePrefab, Hit.point + (Hit.normal * .01f), Quaternion.FromToRotation(Vector3.up, Hit.normal));
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
            bool isDead = playerData.TakeDamage(dmg, Player.GetComponent<PlayerData>().playerNameSynced);
            if (isDead)
            {
                RpcAddKill();
            }
        }
    }

    [ClientRpc]
    public void RpcAddKill()
    {
        if (isLocalPlayer)
        {
            Player.GetComponent<PlayerData>().KillCounter++;
        }
    }

    public void HandleKnifeEvents()
    {
        Debug.Log("Shot sound");
        _shotSource.PlayOneShot(_shotSound);
    }

    [Command]
    void CmdHandleKnifeEvents(NetworkConnectionToClient sender = null)
    {
        RpcHandleShotEvents();
    }

    [ClientRpc]
    void RpcHandleShotEvents()
    {
        if (!isLocalPlayer)
        {
            _shotSource.PlayOneShot(_shotSound);
        }
    }

}
