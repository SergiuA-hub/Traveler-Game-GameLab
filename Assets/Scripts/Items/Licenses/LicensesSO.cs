using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName ="LicensesSO",fileName ="LicenseSO")]
public class LicensesSO : ScriptableObject
{
    public string LicenseName;
    public Image icon;
    public float cost;
    public ResourceSO[] resourse_unlock;

    public bool canBuy;
    public bool canSell;    
}
