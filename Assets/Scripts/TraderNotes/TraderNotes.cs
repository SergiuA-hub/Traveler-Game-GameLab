using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TradeNote
{
    public ResourceSO good;
    public float price;
    public string settlementName;
    public DateTime noteDate;

    public TradeNote(ResourceSO res, float price, string settlement, DateTime time)
    {
        good = res;
        this.price = price;
        settlementName = settlement;
        noteDate = time;
    }
}


[System.Serializable]
public class TraderNotes : MonoBehaviour
{
    public List<TradeNote> toSell = new();
    public List<TradeNote> toBuy = new();

    private void Start()
    {
        GameServices.Instance.timeManager.onDayChanged.AddListener(dailyUpdate);
    }

    public void AddBuyNote(ResourceSO res, float price, string settlement, DateTime currentDate)
    {
        foreach(var good in toBuy)
        {
            if (good.good == res && good.settlementName == settlement)
            {
                good.price = price;
                good.settlementName = settlement;
                good.noteDate = currentDate;
                return;
            }
        }

        toBuy.Add(new TradeNote(res, price, settlement, currentDate));
    }

    public void AddSellNote(ResourceSO res, float price, string settlement, DateTime currentDate)
    {
        foreach (var good in toSell)
        {
            if (good.good == res && good.settlementName == settlement)
            {
                good.price = price;
                good.settlementName = settlement;
                good.noteDate = currentDate;
                return;
            }
        }

        toSell.Add(new TradeNote(res, price, settlement, currentDate));
    }

    public void dailyUpdate(DateTime currentDate)
    {
        toBuy.RemoveAll(note => (currentDate - note.noteDate).TotalDays >= 7);
        toSell.RemoveAll(note => (currentDate - note.noteDate).TotalDays >= 7);
    }
}
