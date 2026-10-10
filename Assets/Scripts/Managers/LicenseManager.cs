using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LicenseManager : MonoBehaviour
{
    public static LicenseManager instance { get; private set; }

    public List<LicensesSO> ownedLicenses = new List<LicensesSO>();
    //check for already owned licenses
    public bool AlreadyOwnsLicense(LicensesSO licensesSO) => ownedLicenses.Contains(licensesSO);    
    public void Awake()
    {
        instance = this;
    }

    public bool PuchaseLicenses(LicensesSO licensesSO,ref float playerGold)
    {
        //have license or no money
        if (AlreadyOwnsLicense(licensesSO) || playerGold < licensesSO.cost) return false;
        
        //Else
        playerGold -= licensesSO.cost;
        ownedLicenses.Add(licensesSO);
        return true;

    }

    public bool CanTrade(ResourceSO resource)
    {
        foreach(var license in ownedLicenses)
        {
            if(license.resourse_unlock.Contains(resource)) 
                return true;
            
        }
        return false;
    }
}
