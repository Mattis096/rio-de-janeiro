using UnityEngine;

[System.Serializable]
public class InventoryItem : MonoBehaviour
{
    public int itemID;       // Identifiant unique de l'objet
    public string itemName;  // Nom de l'objet
    public int quantity;     // Quantité dans l'inventaire

    public InventoryItem(int id, string name, int qty)
    {
        itemID = id;
        itemName = name;
        quantity = qty;
    }

}
