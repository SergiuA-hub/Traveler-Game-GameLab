using System.Collections.Generic;
using UnityEngine;

public class TradeTierUIIndicator : MonoBehaviour
{
    public List<GameObject> tierIndicators;

    void Start()
    {
        clearIndicator();
    }

    public void clearIndicator()
    {
        foreach (var tier in tierIndicators)
        {
            tier.SetActive(false);
        }
    }

    public void indicateTier(int index)
    {
        if (index == 0)
            clearIndicator();

        for (int i = 0; i < tierIndicators.Count; i++)
        {
            tierIndicators[i].SetActive(index == i + 1);
        }
    }
}