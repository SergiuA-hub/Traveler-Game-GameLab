using System.Collections.Generic;

[System.Serializable]
public class Cargo
{
    //Total Stats
    public float weight;
    public float volume;

    public CargoSO cargoSO;

    //Upgrading Items
    public int upgradesSpaces;
    public List<CargoSO> Upgrades = new List<CargoSO>();


    //Item enums
    public CargoCategory category;
    public ItemType type;


    public Cargo(CargoSO cargoSO, int upSpace, List<CargoSO> upgrades, CargoCategory category, ItemType type)
    {
        this.cargoSO = cargoSO;
        this.upgradesSpaces = upSpace;
        this.Upgrades = upgrades;
        this.category = category;
        this.type = type;

    }

    public void CalculateTotalWeightAndVolume()
    {
        weight = cargoSO.itemMaxWeight;
        volume = cargoSO.itemMaxVolume;
        //Adding Upgrades
        foreach (CargoSO item in Upgrades)
        {
            weight += item.itemMaxWeight;
            volume += item.itemMaxVolume;
        }
    }

    public bool CanAddUpgrade(CargoSO cargo)
    {
        if (Upgrades.Count < upgradesSpaces)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}

