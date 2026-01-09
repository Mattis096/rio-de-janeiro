using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using TMPro;

public class PlayerInteractions : NetworkBehaviour
{

    [Header("References Locales")]
    public GameObject PlayerHandGrip;       // where weapon will be attached to the player
    public GameObject PlayerEyes;           // ref to the player look at direction 
    public PlayerMovement PlayerMovement;   // ref to player mvmt script 

    [Header("Debug DrawLine")]
    private RaycastHit hit;

    [Header("Player Loadouts")]
    public Loadout[] Loadouts;
    public Loadout[] TPSLoadouts;

    [Header("Player Current Loadouts")]
    public List<GameObject> FPSweapons;
    public List<GameObject> TPSweapons;

    [Header("Player Base Knife")]
    public GameObject KnifeFPS;
    public GameObject KnifeTPS;

    [Header("Player Current Weapon")]
    public GameObject activeWeapon;
    public GameObject activeWeaponTPS;

    [Header("ADS Control")]
    public bool isADS = false;

    [Header("Hand Position Control")]
    public Vector3 currentHandPos;
    public Quaternion currentHandRot;

    [Header("DEBUG")]
    public bool brk = false;

    [Header("Variables Sync")]
    [SerializeField] private int selectedWeapon = 0;
    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 0;
    [SerializeField] private string selectedLoadout = "";
    [SyncVar(hook = nameof(OnLoadoutChanged))]
    public string selectedLoadoutSynced = "";

    public PlayerUI playerUI;

    public bool canShoot = true;

    //public Transform weaponHolderParent;

    public int currentLoadout = 0;

    private void Start()
    {
        if (!isLocalPlayer) return;
        FPSweapons = new List<GameObject>(new GameObject[3]);
        TPSweapons = new List<GameObject>(new GameObject[3]);

        FPSweapons[0] = activeWeapon;
        TPSweapons[0] = activeWeaponTPS;
        
        activeWeapon = KnifeFPS;
        activeWeaponTPS = KnifeTPS;

        activeWeapon.gameObject.SetActive(true);
        activeWeaponTPS.gameObject.SetActive(true);

        currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localPosition;
        currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localRotation;
    }

    RaycastHit h;

    private void Update()
    {
        Vector3 origin = Vector3.zero;
        Vector3 direction = Vector3.zero;

        origin = PlayerEyes.transform.position;
        direction = PlayerEyes.transform.forward;

        if (Physics.Raycast(origin, direction, out h, Mathf.Infinity))
        {
            Debug.DrawLine(origin, h.point, Color.red);
        }


        if (isLocalPlayer)
        {
            InputManager();
            
            
            
            UpdatePlayerHandGrip(); 
            activeWeapon.GetComponent<BaseWeapon>().Hold_ADS(isADS);
            
            
        }
    }

    public void DisableAllWeaponsFPS()
    {
        foreach (var weapon in FPSweapons)
        {
            weapon.SetActive(false);
        }
    }

    public void DisableAllWeaponsTPS()
    {
        foreach(var weapon in TPSweapons)
        {
            weapon.SetActive(false);
        }
    }

    public void SetCurrentWeaponActivateFPS()
    {
        DisableAllWeaponsFPS();
        activeWeapon = FPSweapons[0];
        
        activeWeapon.SetActive(true);
        currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localPosition;
        currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localRotation;
    }

    public void SetCurrentWeaponActivateTPS()
    {
        DisableAllWeaponsTPS();
        activeWeaponTPS = TPSweapons[0];
        activeWeaponTPS.SetActive(true);

        // management mains vue tps

    }

    public void ChangeLoadout(string name)
    {
        for(int i = 0; i < Loadouts.Length; i ++)
        {
            if(string.Equals(name, Loadouts[i].name, StringComparison.OrdinalIgnoreCase))
            {
                // FPS 

                Loadouts[currentLoadout].DisableLoadout();
                Loadouts[i].EquipLoadout(FPSweapons, KnifeFPS);
                SetCurrentWeaponActivateFPS();

                // TPS 

                TPSLoadouts[currentLoadout].DisableLoadout();
                TPSLoadouts[i].EquipLoadout(TPSweapons, KnifeTPS);
                SetCurrentWeaponActivateTPS();

                currentLoadout = i;
                break;
            }
        }
    }

    public void EDPlayerShoot(bool status)
    {
        canShoot = status;
    }

    private void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerMovement.moveSpeed += 4;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            PlayerMovement.moveSpeed -= 4;

        }
        if (Input.GetButton("Fire1") && canShoot)
        {
            if (activeWeapon != null)
            {
                // New Weapon Class
                BaseWeapon FPSweapon = activeWeapon.GetComponent<BaseWeapon>();
                FPSweapon.TryShoot();
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (activeWeapon != null)
            {
                activeWeapon.GetComponent<BaseWeapon>().Reload();
            }
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            CmdChangeActiveWeapon(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            CmdChangeActiveWeapon(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            CmdChangeActiveWeapon(2);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            // Old Changing weapons system
            //selectedWeapon += 1;
            //if (selectedWeapon > FPSweapons.Count-1)
            //{
            //    selectedWeapon = 0;
            //}
            //CmdChangeActiveWeapon(selectedWeapon);

            isADS = true;

            activeWeapon.GetComponent<BaseWeapon>()._ads = true;
            currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGripADS.transform.localPosition;
            currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGripADS.transform.localRotation;

        }

        if (Input.GetButtonUp("Fire2"))
        {
            isADS = false;
            activeWeapon.GetComponent<BaseWeapon>()._ads = false;
            
            currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localPosition;
            currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localRotation;
        }
    }

    public void UpdatePlayerHandGrip()
    {
        PlayerHandGrip.transform.localPosition = Vector3.Lerp(PlayerHandGrip.transform.localPosition, currentHandPos, Time.deltaTime * 10);
        PlayerHandGrip.transform.localRotation = Quaternion.Lerp(PlayerHandGrip.transform.localRotation, currentHandRot, Time.deltaTime * 10);
    }

    // Weapon Index Sync Var

    void OnLoadoutChanged(string _Old, string _New)
    {
        ChangeLoadout(_New);
        UpdatePlayerUI();
    }

    void OnWeaponChanged(int _Old, int _New)
    {
        if(0 <= _Old && _Old < FPSweapons.Count && FPSweapons[_Old] != null)
        {
            FPSweapons[_Old].gameObject.SetActive(false);
            TPSweapons[_Old].gameObject.SetActive(false);
        }
        if (0 <= _New && _New < FPSweapons.Count && FPSweapons[_New] != null)
        {
            FPSweapons[_New].gameObject.SetActive(true);
            TPSweapons[_New].gameObject.SetActive(true);

            activeWeapon = FPSweapons[_New];
            activeWeaponTPS = TPSweapons[_New];

            currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localPosition;
            currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGrip.transform.localRotation;
            if(isADS && activeWeapon.GetComponent<BaseWeapon>().WeaponType != WeaponType.Knife)
            {
                activeWeapon.GetComponent<BaseWeapon>()._ads = true;
                currentHandPos = activeWeapon.GetComponent<BaseWeapon>()._handGripADS.transform.localPosition;
                currentHandRot = activeWeapon.GetComponent<BaseWeapon>()._handGripADS.transform.localRotation;
            }

            if (isLocalPlayer)
            {
                playerUI.setPlayerAmmo(activeWeapon.GetComponent<BaseWeapon>().currentMag, activeWeapon.GetComponent<BaseWeapon>()._magSize);
            }
        }
    }

    [Command]
    public void CmdChangeActiveLoadout(string newLoadout)
    {
        selectedLoadoutSynced = newLoadout;
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }

    public void UpdatePosition(GameObject go, Vector3 pos)
    {
        go.transform.position = pos;
    }

    [Command]
    void CmdPickupItem(NetworkIdentity item, NetworkConnectionToClient sender = null)
    {
        item.RemoveClientAuthority();
        item.AssignClientAuthority(sender);
    }

    public void UpdatePlayerUI()
    {
        playerUI.PrimaryWp_Img.sprite = Loadouts[currentLoadout].PrimaryWeapon.GetComponent<BaseWeapon>()._weaponSprite;
        playerUI.SecondaryWp_Img.sprite = Loadouts[currentLoadout].SecondaryWeapon.GetComponent<BaseWeapon>()._weaponSprite;
    }
}

