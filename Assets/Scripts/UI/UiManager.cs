using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject inventoryDialog;
    public GameObject notesDialog;
    public GameObject inventoryButton;
    public GameObject campButton;
    private bool inventoryToggleSwitch;
    public void toggleInventory()
    {
        inventoryToggleSwitch = !inventoryToggleSwitch;

        if (!inventoryDialog.activeSelf && !notesDialog.activeSelf)
            inventoryToggleSwitch = true;

        inventoryDialog.SetActive(inventoryToggleSwitch);
        notesDialog.SetActive(inventoryToggleSwitch);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryToggleSwitch = false;

        inventoryDialog.SetActive(inventoryToggleSwitch);
        notesDialog.SetActive(inventoryToggleSwitch);
    }

    public void dismissInventoryUI()
    {
        inventoryDialog.SetActive(false);
        notesDialog.SetActive(false);
    }

    public void campingButtonState(bool sw)
    {
        campButton.SetActive(sw);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
