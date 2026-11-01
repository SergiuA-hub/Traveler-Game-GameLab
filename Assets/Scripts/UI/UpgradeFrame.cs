using UnityEngine;

public class UpgradeFrame : MonoBehaviour
{
    public Transform SlotPlace;
    public UpgradeSlot upgradeSlot;

    public Transform ReturnSlotPlace()
    {
        return SlotPlace;
    }
}
