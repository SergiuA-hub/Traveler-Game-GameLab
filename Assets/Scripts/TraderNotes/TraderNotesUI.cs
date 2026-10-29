using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraderNotesUI : MonoBehaviour
{
    public GameObject toBuyList;
    public GameObject toSellList;
    public GameObject notePrefab;
    public TMP_InputField searchInputField;
    public TMP_Dropdown settlementsList;

    public void Start()
    {
        settlementsList.ClearOptions();

        settlementsList.options.Add(
            new TMP_Dropdown.OptionData("All Settlements")
        );

        foreach (var settlement in GameServices.Instance.settlementManager.settlements)
        {
            settlementsList.options.Add(new TMP_Dropdown.OptionData(settlement.settlementName));
        }

        settlementsList.value = 0;
        settlementsList.RefreshShownValue();
    }
    public void OnSearchFocus(string value)
    {
        GameServices.Instance.player.move.typing = true;
    }

    public void OnSearchUnfocus(string value)
    {
        GameServices.Instance.player.move.typing = false;
    }

    private void OnEnable()
    {
        refreshResList();

    }

    private void OnDisable()
    {
        clearResources();
        searchInputField.text = "";
        settlementsList.value = 0;

        GameServices.Instance.player.move.typing = false;
    }

    public void settlementDropDownSelected()
    {
        filterResoruces();
    }

    public void clearResources()
    {
        foreach (Transform child in toBuyList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in toSellList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void filterResoruces()
    {
        string searchText = searchInputField.text.Trim();

        int selectedIndex = settlementsList.value;
        string selectedSettlement = settlementsList.options[selectedIndex].text;

        foreach (Transform res in toBuyList.transform)
        {
            bool matchesResource = res.name.ContainsInsensitive("res_" + searchText);
            bool matchesSettlement = selectedIndex == 0 || res.name.ContainsInsensitive("_set_" + selectedSettlement);

            res.gameObject.SetActive(matchesResource && matchesSettlement);
        }

        foreach (Transform res in toSellList.transform)
        {
            bool matchesResource = res.name.ContainsInsensitive("res_" + searchText);
            bool matchesSettlement = selectedIndex == 0 || res.name.ContainsInsensitive("_set_" + selectedSettlement);

            res.gameObject.SetActive(matchesResource && matchesSettlement);
        }
    }

    public void typing()
    {
        filterResoruces();
    }



    public void refreshResList()
    {
        foreach (var res in GameServices.Instance.player.notes.toBuy)
        {
            GameObject go = Instantiate(notePrefab, toBuyList.transform);
            go.name = "res_" + res.good.itemName + "_set_" + res.settlementName;
            go.GetComponent<TraderNotePrefab>().setup(res);
        }

        foreach (var res in GameServices.Instance.player.notes.toSell)
        {
            GameObject go = Instantiate(notePrefab, toSellList.transform);
            go.name = "res_" + res.good.itemName + "_set_" + res.settlementName;
            go.GetComponent<TraderNotePrefab>().setup(res);
        }
    }
}
