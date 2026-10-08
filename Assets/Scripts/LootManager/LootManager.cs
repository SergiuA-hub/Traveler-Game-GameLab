using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootManager : MonoBehaviour
{
    public Player_M player;
    public AudioSource investigatingAudio;
    [Range(0f, 1f)]
    public float investigatingVolume = 0.7f;
    public GameObject springPlaceHoldersList;
    public GameObject summerPlaceHoldersList;
    public GameObject fallPlaceHoldersList;
    public GameObject winterPlaceHoldersList;
    public List<GameObject> sackLoots;
    public List<Transform> springLootPlaceHolders;
    public List<Transform> summerLootPlaceHolders;
    public List<Transform> fallLootPlaceHolders;
    public List<Transform> winterLootPlaceHolders;
    public List<LootOutcome> lootOutcomeList = new List<LootOutcome>();
    
    public int springMaxSpawnCount;
    public int summerMaxSpawnCount;
    public int fallMaxSpawnCount;
    public int winterMaxSpawnCount;
    
    public Button investigeButton;
    private bool isInvestigating;
    private LootPrefab lootableItem;
    private LootOutcome currentOutcome;

    public Slider investigatingProgress;
    public TMP_Text investigateTimer;

    public LootPanelUI lootPanelUI;

    public Button campButton;
    public GameObject InventoryPanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lootOutcomeList = new List<LootOutcome>(
            Resources.LoadAll<LootOutcome>("Loot")
        );

        //read spawnpoints
        foreach (Transform placeholder in summerPlaceHoldersList.transform)
        {
            summerLootPlaceHolders.Add(placeholder);
        }

        foreach (Transform placeholder in fallPlaceHoldersList.transform)
        {
            fallLootPlaceHolders.Add(placeholder);
        }

        foreach (Transform placeholder in winterPlaceHoldersList.transform)
        {
            winterLootPlaceHolders.Add(placeholder);
        }

        foreach (Transform placeholder in springPlaceHoldersList.transform)
        {
            springLootPlaceHolders.Add(placeholder);
        }

        //spawnloots
        spawnLoot(springLootPlaceHolders, springMaxSpawnCount, Season.Spring);
        spawnLoot(summerLootPlaceHolders, summerMaxSpawnCount, Season.Summer);
        spawnLoot(fallLootPlaceHolders, fallMaxSpawnCount, Season.Fall);
        spawnLoot(winterLootPlaceHolders, winterMaxSpawnCount, Season.Winter);

        investigatingProgress.gameObject.SetActive(false);
    }

    public void spawnLoot(List<Transform> spawnersList, int totalCount, Season location, string avoidLocation = null)
    {
        int currentCount = 0;
        List<Transform> noLootList = new List<Transform>();
        noLootList.Clear();

        foreach (Transform spawnPoint in spawnersList)
        {
            if (spawnPoint.childCount > 0)
            {
                currentCount++;
            }
            else noLootList.Add(spawnPoint);
        }

        Debug.Log($"Current spawn({currentCount}) vs total({totalCount})");
        if (currentCount >= totalCount)
            return;

        Debug.Log($"noLootCount before Avoid locatio {noLootList.Count}");
        if (avoidLocation != null)
        {
            Debug.Log($"AVOINDING {avoidLocation}");
            foreach(Transform loc in noLootList)
            {
                if(loc.gameObject.name == avoidLocation)
                {
                    Debug.Log($"Removed {loc.gameObject.name}");
                    noLootList.Remove(loc);
                    break;
                }
            }
            
        }
        Debug.Log($"noLootCount After Avoid locatio {noLootList.Count}");

        for (int i=0; i< totalCount - currentCount; i++)
        {
            int randomIndex = Random.Range(0, sackLoots.Count);
            int randomSpawner = Random.Range(0, noLootList.Count);

            GameObject go = Instantiate(sackLoots[randomIndex], noLootList[randomSpawner]);
            go.GetComponent<LootPrefab>().setup(this, location);
            go.transform.localPosition = Vector3.zero;

            noLootList.RemoveAt(randomSpawner);
        }
    }

    public void playerCloseToLoot(LootPrefab loot)
    {
        
        lootableItem = loot;
        investigeButton.gameObject.SetActive(true);
    }

    public void playerLeftLootArea()
    {
        lootableItem = null;
        investigeButton.gameObject.SetActive(false);
    }

    public void investigatePressed()
    {
        if(lootableItem != null)
        {
            Debug.Log($"Investigation {lootableItem.name}");
            

            List<LootOutcome> possibleOutcomes = lootOutcomeList.FindAll(x => x.outcomeForLoot == lootableItem.type);

            if (possibleOutcomes.Count == 0)
            {
                Debug.LogWarning($"No loot outcomes found for {lootableItem.type}");
                return;
            }

            int randomIndex = Random.Range(0, possibleOutcomes.Count);
            currentOutcome = possibleOutcomes[randomIndex];

            prepareInvestigationActionSlider();
            Debug.Log($"Selected outcome: {currentOutcome.name}");
            Debug.Log(currentOutcome.outcomeStory);
        }
    }

    private float investigationTimer =0f;
    public void prepareInvestigationActionSlider()
    {
        if (lootableItem == null)
            return;

        if (investigatingAudio != null)
        {
            investigatingAudio.loop = true;

            // Variații discrete la fiecare investigație.
            investigatingAudio.pitch = Random.Range(0.97f, 1.03f);

            investigatingAudio.volume = Mathf.Clamp01(
                investigatingVolume * Random.Range(0.93f, 1f)
            );

            investigatingAudio.Play();
        }

        player.Root();
        InventoryPanel.SetActive(false);
        campButton.interactable = false;

        investigeButton.gameObject.SetActive(false);

        isInvestigating = true;
        investigationTimer = 0f;

        investigatingProgress.minValue = 0f;
        investigatingProgress.maxValue = 1f;
        investigatingProgress.value = 0f;

        investigatingProgress.gameObject.SetActive(true);
        Debug.Log($"MAX VALUE IS: {investigatingProgress.maxValue}");
    }

    public void InvestigationFinished()
    {
        investigatingProgress.gameObject.SetActive(false);
        if (investigatingAudio != null)
            investigatingAudio.Stop();

        string spawnName = lootableItem.transform.parent.gameObject.name;
        Season tempSeason = lootableItem.lootLocation;
        Debug.LogWarning($"Item Destroyed{lootableItem} at {lootableItem.lootLocation}");

        lootableItem.transform.SetParent(null);
        Destroy(lootableItem.gameObject);

        switch (tempSeason)
        {
            case Season.Spring:
                spawnLoot(springLootPlaceHolders, springMaxSpawnCount, Season.Spring, spawnName);                
                break;
            case Season.Summer:
                spawnLoot(summerLootPlaceHolders, summerMaxSpawnCount, Season.Summer, spawnName);
                break;
            case Season.Fall:
                spawnLoot(fallLootPlaceHolders, fallMaxSpawnCount, Season.Fall, spawnName);
                break;
            case Season.Winter:
                spawnLoot(winterLootPlaceHolders, winterMaxSpawnCount, Season.Winter, spawnName);
                break;
            default:
                break;
        }
        

        lootPanelUI.setup(currentOutcome);
        
        lootPanelUI.gameObject.SetActive(true);
    }

    public void LootCompleted()
    {
        lootPanelUI.gameObject.SetActive(false);

        campButton.interactable = true;

        player.UnRoot();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInvestigating)
            return;

        investigationTimer += Time.deltaTime;

        investigatingProgress.value =
            investigationTimer / currentOutcome.investigationDuration;
        
        float remaining = Mathf.Max(0f, currentOutcome.investigationDuration - investigationTimer);
        investigateTimer.text = $"{remaining:0.0}s";

        if (investigationTimer >= currentOutcome.investigationDuration)
        {
            investigatingProgress.value = currentOutcome.investigationDuration;
            isInvestigating = false;

            InvestigationFinished();
        }
    }
}
