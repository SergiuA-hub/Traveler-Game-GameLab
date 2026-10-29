using System.Collections.Generic;
using UnityEngine;

public class SettlementContractsPanel : MonoBehaviour
{
    public GameObject newContractsImage;
    public List<Contract> contractBoardList;
    private bool newContracts;
    public void AddContract(Contract contract)
    {
        contractBoardList.Add(contract);
    }

    public void setNewFlag(bool flag)
    {
        newContracts = flag;
        newContractsImage.SetActive(newContracts);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player_M>() != null)
        {
            GameServices.Instance.contractManager.playerMovedCloseTo(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameServices.Instance.contractManager.playetMovedAwayFrom(this);
    }
}
