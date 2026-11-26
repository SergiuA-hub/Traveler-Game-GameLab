using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WizardTimeAnnouncement
{
    public int minute;
    public AudioClip clip;
}

public class WizardManager : MonoBehaviour
{
    public AudioSource wizardAudioSource;

    public Button InteractionButton;
    public GameObject WizardInteractionUI;

    public List<AudioClip> wizardBuffReplica;
    public List<WizardTimeAnnouncement> wizardTimeAnnouncements;
    public AudioClip wizardExpiredSpell;
    private bool pendingReplica;
    private bool pendingSpell;
    private int lastReplicaIndex = -1;
    public void Start()
    {
        GameServices.Instance.timeManager.onWizardTime.AddListener(perMinuteUpdate);
    }

    public void perMinuteUpdate(float minutesLeft)
    {
        var announcement = wizardTimeAnnouncements.Find(
            x => x.minute == Mathf.RoundToInt(minutesLeft)
        );

        if (announcement == null || announcement.clip == null)
            return;

        wizardAudioSource.Stop();
        wizardAudioSource.loop = false;
        wizardAudioSource.clip = announcement.clip;
        wizardAudioSource.Play();

        pendingReplica = minutesLeft > 0;
        pendingSpell = minutesLeft <= 0;
    }

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

    private void playRandomReplica()
    {
        if (wizardBuffReplica.Count == 0)
            return;

        int index;

        do
        {
            index = UnityEngine.Random.Range(
                0, wizardBuffReplica.Count);
        }
        while (index == lastReplicaIndex &&
               wizardBuffReplica.Count > 1);

        lastReplicaIndex = index;

        wizardAudioSource.loop = false;
        wizardAudioSource.clip = wizardBuffReplica[index];
        wizardAudioSource.Play();
    }

    public void stopWizardSound()
    {
        pendingReplica = false;
        pendingSpell = false;

        wizardAudioSource.Stop();
        wizardAudioSource.loop = false;
    }

    private void Update()
    {
        if (!pendingReplica && !pendingSpell)
            return;

        if (!Application.isFocused)
            return;

        if (wizardAudioSource.isPlaying)
            return;

        if (pendingReplica)
        {
            pendingReplica = false;
            playRandomReplica();
        }
        else if (pendingSpell)
        {
            pendingSpell = false;

            wizardAudioSource.clip = wizardExpiredSpell;
            wizardAudioSource.loop = true;
            wizardAudioSource.Play();
        }
    }
}