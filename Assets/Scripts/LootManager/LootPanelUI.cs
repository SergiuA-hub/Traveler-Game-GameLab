using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootPanelUI : MonoBehaviour
{
    public Player_M player;
    public LootManager lootManager;
    public Image lootOutcomeIcon;
    public TMP_Text outcomeStory;

    public GameObject coinsPanel;
    public TMP_Text coinsAmount;

    public GameObject resourcesPanel;
    public GameObject resoruceList;
    public GameObject resourcePrefab;
    
    public GameObject statLossPanel;
    public GameObject staminaLossPanel;
    public TMP_Text staminaLossAmount;
    public GameObject hpLossPanel;
    public TMP_Text hpLossAmount;
    public GameObject noLootPanel;
    public GameObject lootPanel;
    private List<ResourceDrop> resourceDrops = new List<ResourceDrop>();
    private int coinsAmountValue;

    private LootOutcome outcome;
    public void setup(LootOutcome oc)
    {
        this.outcome = oc;

        lootOutcomeIcon.sprite = outcome.outcomeImage;
        outcomeStory.text = outcome.outcomeStory;

        if (outcome.outcomeCoins > 0)
        {
            lootPanel.SetActive(true);
            coinsAmountValue = Random.Range(1, outcome.outcomeCoins + 1);
            coinsPanel.SetActive(true);
            coinsAmount.text = coinsAmountValue.ToString();
        }
        else
        {
            coinsPanel.SetActive(false);
        }



        if (outcome.lootDrop.Count > 0)
        {
            lootPanel.SetActive(true);
            calculateResourceDrop();
            //cleanup if any previous resource drops were displayed
            foreach (Transform child in resoruceList.transform)
            {
                Destroy(child.gameObject);
            }

            resourcesPanel.SetActive(true);
            foreach (ResourceDrop drop in resourceDrops)
            {
                GameObject go = Instantiate(resourcePrefab, resoruceList.transform);
                go.GetComponent<ResourceDropPrefab>().setup(drop);
            }
        }
        else
        {
            resourcesPanel.SetActive(false);
        }

        if (outcome.hpLoss > 0 || outcome.staminaLoss > 0)
        {
            statLossPanel.SetActive(true);
            if (outcome.hpLoss > 0)
            {
                hpLossPanel.SetActive(true);
                hpLossAmount.text = "Health: - " + outcome.hpLoss.ToString();
            }
            else
            {
                hpLossPanel.SetActive(false);
            }

            if (outcome.staminaLoss > 0)
            {
                staminaLossPanel.SetActive(true);
                staminaLossAmount.text = "Stamina: - " + outcome.staminaLoss.ToString();
            }
            else
            {
                staminaLossPanel.SetActive(false);
            }
        }
        else
        {
            statLossPanel.SetActive(false);
        }

        if (outcome.outcomeCoins <= 0 && outcome.lootDrop.Count <= 0 && outcome.hpLoss <= 0 && outcome.staminaLoss <= 0)
        {
            lootPanel.SetActive(false);
            noLootPanel.SetActive(true);
        }
        else
        {
            noLootPanel.SetActive(false);
        }
    }

    public void calculateResourceDrop()
    {
        resourceDrops.Clear();
        foreach (ResourceDrop drop in outcome.lootDrop)
        {
            float roll = Random.Range(0f, 1f);
            ResourceDrop newDrop = new ResourceDrop(drop.resource, Random.Range(1, drop.amount + 1), drop.dropChance);
            if (roll <= drop.dropChance)
            {
                resourceDrops.Add(newDrop);
            }
        }
    }

    public void panelDismissed()
    {
        foreach(ResourceDrop drop in resourceDrops)
        {
            player.invetory.AddItem(drop.resource, drop.amount);
        }

        player.invetory.CurrentCoins += coinsAmountValue;

        if(outcome.hpLoss > 0)
        {
            player.stats.currentHp -= outcome.hpLoss;
        }

        if(outcome.staminaLoss > 0)
        {
            player.stats.currentStamina -= outcome.staminaLoss;
        }

        lootManager.LootCompleted();
    }
}
