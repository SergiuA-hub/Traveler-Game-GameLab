using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject inventoryDialog;
    public GameObject notesDialog;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
