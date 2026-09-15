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
    [SerializeField] private GameObject CityUI;

    [Header("Components")]
    [SerializeField] private CityUI cityUI;
    [SerializeField] private TimeManager timeManager;

    private void Start()
    {
        cityUI = GetComponent<CityUI>();
    }
    


    //am nevoie sa stiu mereu unde este playerul in UI,
    //Trebuie sa adaug un script care se ocupa de UI separat scriptul "Setlementuluil"

    //Fiecare Setlement trebuie sa aiba un so cu anumite date in el 


}
