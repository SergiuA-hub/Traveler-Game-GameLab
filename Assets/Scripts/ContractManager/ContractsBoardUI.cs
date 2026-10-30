using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ContractsBoardUI : MonoBehaviour
{

    public GameObject contractsPrefab;

    public List<GameObject> placeholdersList;
    private SettlementContractsPanel boardPanel;    

    public void setup(SettlementContractsPanel panel)
    {
        boardPanel = panel;

        int index = 0;
        foreach(var contract in boardPanel.contractBoardList)
        {
            GameObject go = Instantiate(contractsPrefab, placeholdersList[index].transform);
            go.GetComponent<ContractPanelPrefab>().setup(contract);
            
            float randomRotation = Random.Range(-10f, 10f);
            float randomX = Random.Range(-15f, 15f);
            float randomY = Random.Range(-5f, 5f);

            go.transform.localRotation = Quaternion.Euler(randomX, randomY, randomRotation);

            index++;
        }

    }
}
