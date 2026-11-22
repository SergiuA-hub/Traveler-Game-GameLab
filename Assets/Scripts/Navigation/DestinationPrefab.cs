using TMPro;
using UnityEngine;

public class DestinationPrefab : MonoBehaviour
{
    public TMP_Text destinationText;
    public NavigationUI callback;
    private Transform destination;
    private string locationName;

    public void setup(string destinationName, Transform dest, NavigationUI cb)
    {
        callback = cb;
        destination = dest;
        destinationText.text = destinationName;
        locationName = destinationName;
    }
    
    public void destinationSelected()
    {
        callback.selectedDestionation(destination);

        if (GameServices.Instance.internalTesting)
        {
            GameServices.Instance.teleport.movePlayer(locationName);
        }
    }

}
