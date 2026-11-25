using UnityEngine;

public class MapDisplayUI : MonoBehaviour
{
    public AudioClip openMapSound;
    public AudioClip closeMapSound;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        GameServices.Instance.soundManager.UISound(openMapSound);
    }

    private void OnDisable()
    {
        GameServices.Instance.soundManager.UISound(closeMapSound);
    }
}
