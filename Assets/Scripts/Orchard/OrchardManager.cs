using System;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class OrchardManager : MonoBehaviour
{
    public AudioSource harvestAudtioSource;
    public AudioClip harvestAudioClip;
    public Slider harvestProgressSlider;
    public TMP_Text harvestingTimerText;
    public Button harvestButton;
    public TMP_Text harvestButtonText;

    public List<Transform> orchards;
    private List<FruitTree> orchardTrees = new();
    private FruitTree inRangeTree;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        harvestButton.gameObject.SetActive(false);

        orchardTrees.Clear();
        foreach (Transform orchard in orchards)
        {
            foreach(Transform treeObj in orchard)
            {
                FruitTree fruitTree = treeObj.GetComponent<FruitTree>();
                if (fruitTree != null)
                    orchardTrees.Add(fruitTree);
            }
        }

        GameServices.Instance.timeManager.onHourChanged.AddListener(growTick);
    }

    private void OnDestroy()
    {
        GameServices.Instance.timeManager.onHourChanged.RemoveListener(growTick);
    }

    public void growTick(DateTime t)
    {
        foreach(var tree in orchardTrees)
        {
            tree.growTick();
        }
    }

    public void playerCanInteractWith(FruitTree tree)
    {
        inRangeTree = tree;
        setuButton();
        harvestButton.gameObject.SetActive(true);
    }

    public void setuButton()
    {
        if (inRangeTree.hasFruits)
        {
            harvestButtonText.text = "Harvest";
            harvestButton.interactable = true;
        }
        else
        {
            harvestButtonText.text = "Nothing to harvest yet";
            harvestButton.interactable = false;
        }
    }

    public void justGrow(FruitTree tree)
    {
        if (inRangeTree != tree)
            return;

        setuButton();
    }

    public void playerMovedAwayFromTree(FruitTree tree)
    {
        inRangeTree = null;
        harvestButton.gameObject.SetActive(false);
    }

    public void harvest()
    {
        if (harvesting || inRangeTree == null)
            return;

        if (!inRangeTree.hasFruits)
            return;

        prepareHarvestingActionSlider();
    }

    public void prepareHarvestingActionSlider()
    {
        if (harvestAudtioSource != null)
        {
            harvestAudtioSource.loop = true;
            harvestAudtioSource.pitch = UnityEngine.Random.Range(0.97f, 1.03f);
            harvestAudtioSource.volume = UnityEngine.Random.Range(0.93f, 1f);
            harvestAudtioSource.Play();
        }

        GameServices.Instance.player.Root();
        GameServices.Instance.uiManager.dismissInventoryUI();

        campButtonState = GameServices.Instance.uiManager.campButton.activeSelf;
        GameServices.Instance.uiManager.campingButtonState(false);

        harvestButton.gameObject.SetActive(false);

        harvesting = true;
        harvestTimer = 0f;

        harvestProgressSlider.minValue = 0f;
        harvestProgressSlider.maxValue = 1f;
        harvestProgressSlider.value = 0f;

        harvestingTimerText.text = $"{harvestDuration:0.0}s";

        harvestProgressSlider.gameObject.SetActive(true);
    }

    public void HarvestingFinished()
    {
        harvestProgressSlider.gameObject.SetActive(false);

        if (harvestAudtioSource != null)
            harvestAudtioSource.Stop();

        if (GameServices.Instance.player.invetory.CanCarry(inRangeTree.getTotalVolume(), inRangeTree.getTotalWeight()))
        {
            foreach (var res in inRangeTree.yield)
                GameServices.Instance.player.invetory.AddItem(res.resourceSO, res.amount);
        }
        else GameServices.Instance.player.talk.say("Cannot carry that!");

        inRangeTree.setNoFruitState();
        GameServices.Instance.player.UnRoot();
        GameServices.Instance.uiManager.campingButtonState(campButtonState);
    }

    public void LootCompleted()
    {
        //lootPanelUI.gameObject.SetActive(false);
        GameServices.Instance.player.UnRoot();
        GameServices.Instance.uiManager.campingButtonState(campButtonState);
    }

    // Update is called once per frame
    public float harvestDuration = 2f;

    private float harvestTimer = 0f;
    private bool harvesting;
    private bool campButtonState;

    void Update()
    {
        if (!harvesting)
            return;

        harvestTimer += Time.deltaTime;

        harvestProgressSlider.value = harvestTimer / harvestDuration;

        float remaining = Mathf.Max(0f, harvestDuration - harvestTimer);

        harvestingTimerText.text = $"{remaining:0.0}s";

        if (harvestTimer >= harvestDuration)
        {
            harvestProgressSlider.value = 1f;
            harvesting = false;

            HarvestingFinished();
        }
    }
}
