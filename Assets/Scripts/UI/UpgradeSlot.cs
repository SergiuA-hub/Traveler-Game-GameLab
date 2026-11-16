using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    public Image icon;
   
    public void PopulateSlot(CargoSO cargo)
    {
        icon.sprite = cargo.icon;
    }
}



