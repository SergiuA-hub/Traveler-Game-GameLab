using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private ItemsSO itemsSO;

    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI Name;

    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI Amount;


    private void Awake()
    {
        int randomAmount = Random.Range(1,3);
        displayImage.sprite = itemsSO.sprite;
        Name.text =  itemsSO.name;
        Price.text = "Price" + itemsSO.baseValue.ToString();
        Amount.text = "Amount" +randomAmount.ToString();
    }

}
