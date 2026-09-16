using System;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class SeltelmentItem
{
    ResourceSO resourceSO;
    int amount;
    int Price;

}
public class Setlement : MonoBehaviour
{
    [Header("Setlements")]
    [SerializeField] private string setlementName;

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
    
    


}
