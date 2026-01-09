using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class KnifeScript : NetworkBehaviour
{
    public AudioClip shotSound;           // Bruit du Tir
    public AudioSource shotSource;

    private float nextTimeToFire = 0f;     // Prochaine fois que l'arme peut tirer
    public float fireRate = 1.2f;
    //tmp ref
    public GameObject playerView;
    RaycastHit hit;

    public Vector3 handPos;
    public Vector3 weaponScale;

    public LayerMask targetLayer;

    private void Awake()
    {
        handPos = new Vector3(0, 0,0);
        weaponScale = new Vector3(1, 1, 1);
    }

    void Start()
    {

    }
    void Update()
    {
    }

    public string getGunName()
    {
        return "knife";
    }

    public bool isCurrentlyEquiped()
    {
        return false;
    }
    public void setWeaponStatus(bool status)
    {
    }

    public void IT_Shot(GameObject _playerView, PlayerUI playerUI)
    {
        playerView = _playerView;

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot(playerUI);
        }
    }

    public void IT_Reload(PlayerUI playerUI)
    {
    }

    [Command]
    void CmdHandleShotEvents()
    {
        RpcHandleShotEvents();
    }

    [ClientRpc]
    void RpcHandleShotEvents()
    {
        //shotSource.PlayOneShot(shotSound);
    }


    

    


    private void Shoot(PlayerUI playerUI)
    {

        
        CmdHandleShotEvents();


        //playerUI.setPlayerAmmo(0,0);


        Vector3 shootDirection = playerView.transform.forward;

        int layerMask = LayerMask.GetMask("Hitbox", "Map");


        if (Physics.Raycast(playerView.transform.position, shootDirection, out hit, 10, layerMask))
        {

            Debug.Log($"Hit {hit.collider.name}");

            if (hit.collider.tag == "PlayerBody")
            {
                GameObject player = hit.collider.gameObject.GetComponent<PlayerPrefabRef>().RetreivePlayerRef();
                print(player.name);
                PlayerEvents playerEvents = player.GetComponent<PlayerEvents>();

                int dmg = 50;

                CmdDealDamage(player, dmg);
                Debug.Log("HIT PLAYER B");

                //DamageField(playerEvents.GetPlayerName(), dmg);
            }

        }

    }

    [Command]
    public void CmdDealDamage(GameObject target, int dmg)
    {
        if (target.TryGetComponent<PlayerData>(out PlayerData playerData))
        {
            //playerData.TakeDamage(dmg);
        }
    }


    [Command]
    public void DamageField(string name, int dmg)
    {
        Debug.Log(name + " take " + dmg + " damages");
    }

}
