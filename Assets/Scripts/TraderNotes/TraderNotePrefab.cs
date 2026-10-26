using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderNotePrefab : MonoBehaviour
{
    public Image resIcon;
    public TMP_Text resName;
    public TMP_Text price;
    public TMP_Text settlement;
    public TMP_Text since;
    
    private TradeNote traderNote;
    public void setup(TradeNote note)
    {
        traderNote = note;

        resIcon.sprite = traderNote.good.sprite;
        resName.text = $"{traderNote.good.itemName}";
        price.text = $"{traderNote.price:F1}";
        settlement.text = $"{traderNote.settlementName}";

        double sinceDays = (GameServices.Instance.timeManager.currentTime - traderNote.noteDate).TotalDays;

        int days = Mathf.Max(0, (int)System.Math.Floor(sinceDays));

        if (days == 0)
            since.text = "Today";
        else if (days == 1)
            since.text = "Yesterday";
        else
            since.text = $"{days} days ago";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
