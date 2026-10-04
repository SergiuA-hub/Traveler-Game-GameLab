using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;
public enum LootType
{
    SmallSack,
    BigSack
}
public class LootPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public LootType type;

    private LootManager callback;
    public void setup(LootManager cb)
    {
        callback = cb;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGERED: " + other.name + " -> " + other.tag);
        if (other.CompareTag("Player"))
        {
            callback.playerCloseToLoot(this);
        }

        //cam was deployed on top of the loot without being picked so it wil get destroyed
        if (other.CompareTag("Camp"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        callback.playerLeftLootArea();
    }
}
