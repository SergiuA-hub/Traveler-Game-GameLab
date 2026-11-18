using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioClip soundToTrigger;
    public float triggerChance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Random.Range(0f, 100f) < triggerChance)
        {
            GameServices.Instance.soundManager.triggerSound(soundToTrigger);
        }
    }
}
