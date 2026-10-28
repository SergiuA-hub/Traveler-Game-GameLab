using NUnit.Framework;
using UnityEngine;

public class PlayerAttachments_M : MonoBehaviour
{
    public Cargo currentItemSelected;
    

    public Cargo ReturnCurrenntItem()
    {
        if (currentItemSelected == null)
        {
            Debug.Log("Player doesent have a item attached to him ");
            return null;
        }

        return currentItemSelected;
    }

    public void SetCurrentPlayerSprite()
    {
        //set the player from the cargoSO 
    }

}
