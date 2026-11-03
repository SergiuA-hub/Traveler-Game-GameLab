using UnityEngine;

public class WizardTower : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {
            GameServices.Instance.wizardManager.showInteractionButton();
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {
            GameServices.Instance.wizardManager.hideInteractionButton();
        }
            
    }
}
