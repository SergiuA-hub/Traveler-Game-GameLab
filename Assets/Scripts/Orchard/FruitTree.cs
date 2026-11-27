using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitTree : MonoBehaviour
{
    public GameObject noFruitState;
    public GameObject fruitState;

    public float fruitTotalGrowTime;
    public float currentGrowTime = 0;

    public List<ResourceAmount> yield;
    public bool hasFruits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setNoFruitState();
    }

    public void setNoFruitState()
    {
        hasFruits = false;
        currentGrowTime = 0;

        noFruitState.SetActive(true);
        fruitState.SetActive(false);
    }

    public void setFruitState()
    {
        hasFruits = true;

        GameServices.Instance.orchadManager.justGrow(this);

        noFruitState.SetActive(false);
        fruitState.SetActive(true);
    }

    public void growTick()
    {
        if (hasFruits)
            return;

        currentGrowTime++;

        if (currentGrowTime >= fruitTotalGrowTime)
            setFruitState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameServices.Instance.orchadManager.playerCanInteractWith(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            GameServices.Instance.orchadManager.playerMovedAwayFromTree(this);
    }

    public float getTotalVolume()
    {
        float totalV = 0;
        foreach(var res in yield)
        {
            totalV += res.resourceSO.volume * res.amount;
        }
        return totalV;
    }

    public float getTotalWeight()
    {
        float totalW = 0f;
        foreach(var res in yield)
        {
            totalW += res.resourceSO.weight * res.amount;
        }
        return totalW;
    }
}
