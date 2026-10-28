using TMPro;
using UnityEngine;

public class DestinationPrefab : MonoBehaviour
{
    public TMP_Text destinationText;
    public NavigationUI callback;
    private Transform destination;

    public void setup(string destinationName, Transform dest, NavigationUI cb)
    {
        callback = cb;
        destination = dest;
        destinationText.text = destinationName;
    }
    
    public void destinationSelected()
    {
        callback.selectedDestionation(destination);
    }

}
