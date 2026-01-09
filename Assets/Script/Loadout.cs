using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Loadout : MonoBehaviour
{
    //public string name;
    public GameObject PrimaryWeapon;
    public GameObject SecondaryWeapon;

    //public void Start()
    //{
    //    PrimaryWeapon.SetActive(false);
    //    SecondaryWeapon.SetActive(false);
    //}

    public void EnableLoadout()
    {
        PrimaryWeapon.SetActive(true);
        SecondaryWeapon.SetActive(true);
    }

    public void DisableLoadout()
    {
        PrimaryWeapon.SetActive(false);
        SecondaryWeapon.SetActive(false);
    }

    public void EquipLoadout(List<GameObject> weapons, GameObject Knife)
    {
        EnableLoadout();
        weapons.Clear();
        weapons.Add(Knife);
        weapons.Add(PrimaryWeapon);
        weapons.Add(SecondaryWeapon);
        //weapons[0] = PrimaryWeapon;
        //weapons[1] = SecondaryWeapon;
    }
}
