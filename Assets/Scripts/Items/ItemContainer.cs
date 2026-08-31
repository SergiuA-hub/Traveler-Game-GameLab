using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private ItemsSO itemsSO;

    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI Name;

    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI Amount;
    CityManager city;




    private void Awake()
    {
        //Refrence the manager;
         city = GetComponentInParent<CityManager>();
        
        //displayImage.sprite = itemsSO.sprite;
        Name.text =  itemsSO.name;
        Price.text = "Price: " + itemsSO.baseValue.ToString();
        Amount.text = "Amount: " + city.amount;
    }

    //ButonFunctions -TO DO
    private void Update()
    {
        
    }
    public void Buy()
    {
        city.amount -= 1;
        //take money from Player;
        city.playerInventory.AddItem(itemsSO);
        //Display Again
        Amount.text = "Amount: " + city.amount;
    }

}
