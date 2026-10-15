using UnityEngine;

public class BarterNpc : MonoBehaviour, INpcInteraction
{
    public void Interact()
    {
        Debug.Log("Open Barter UI");
        GameServices.Instance.barterNpcUI.Open(this, GameServices.Instance.player);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
