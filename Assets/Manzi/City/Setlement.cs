using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
[System.Serializable]
public class SettlementItem
{
    public ResourceSO resourceSO;
    public int amount;
    public int price;

}
public class Setlement : MonoBehaviour
{
    [Header("Setlements")]
    [SerializeField] private string setlementName;
    [SerializeField] public List<SettlementItem> settlementItems = new List<SettlementItem>();

    [Header("UI")]
    [SerializeField] private GameObject CityObject;

    [Header("Components")]
    [SerializeField] private CityUI cityUI;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] public Player_M player;

    private void Start()
    {
        cityUI = GetComponent<CityUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //player in range
    }


}
