using UnityEngine;
using Mirror;


public class Inventory : NetworkBehaviour
{

    public int InventorySize = 1;

    public Item PlayerKnife;

    public Item[] Items;

    public GameObject Hand;

   

    private void Start()
    {
        


    }

    public void CreateInventory(int inventorySize)
    {
        Items = new Item[inventorySize];
    }

    public bool isInventoryFull()
    {
        if(Items.Length == 5)
        {
            return true;
        }
        return false;
    }

    public void addItem(Item item)
    {
        print("adding item");
        bool isItemAssigned = false;
        for(int i = 0; i < Items.Length; i++)
        {
            if(Items[i] == null)
            {
                Items[i] = item;
                isItemAssigned = true;
                break;
            }
        }
        if(!isItemAssigned)
        {
            print("inventory full");
        }
    }

}
