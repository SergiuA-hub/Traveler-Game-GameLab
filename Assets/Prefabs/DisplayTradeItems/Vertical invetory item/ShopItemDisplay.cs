using TMPro;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image IconImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI sellPoint;
    public TextMeshProUGUI type;
    public TextMeshProUGUI status;
    public TextMeshProUGUI price;

    //Components
    public CityUI cityUI;


    public void PopulateIcons(ItemsSO item,CityUI ui)
    {
        cityUI = ui;
        IconImage = item.icon;
        itemName.text = item.itemName;
        price.text = item.price.ToString();
        

    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        cityUI.SetCurrentShopItem(this);
    }
}
