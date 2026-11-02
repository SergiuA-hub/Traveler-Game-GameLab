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
        clearBoard();

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

    public void refreshQuestPanel()
    {
        clearBoard();
        setup(boardPanel);
    }

    public void clearBoard()
    {
        for(int i=0; i< placeholdersList.Count; i++)
        {
            if (placeholdersList[i].transform.childCount > 0)
            {
                foreach (Transform child in placeholdersList[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void closeBoard()
    {
        clearBoard();
        GameServices.Instance.player.UnRoot();
        this.gameObject.SetActive(false);
    }
}
