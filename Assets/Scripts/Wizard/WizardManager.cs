using UnityEngine;
using UnityEngine.UI;

public class WizardManager : MonoBehaviour
{
    public Button InteractionButton;
    public GameObject WizardInteractionUI;
    //called by WizardInteraction button
    public void interactWithWizard()
    {
        GameServices.Instance.player.Root();
        WizardInteractionUI.gameObject.SetActive(true);
    }

    public void finalizeInteractionWithWizard()
    {
        GameServices.Instance.player.UnRoot();
    }

    public void showInteractionButton()
    {
        InteractionButton.gameObject.SetActive(true);
    }

    public float getBuffCost(int timeValue)
    {
        return 10f;
    }

    public void hideInteractionButton()
    {
        InteractionButton.gameObject.SetActive(false);
    }
}
