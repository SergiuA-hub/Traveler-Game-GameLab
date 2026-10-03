using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;

    [Header("Day")]
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private float dayIntensity;

    [Header("Night")]
    [SerializeField] private Color nightColor = new Color(0.1f, 0.16f, 0.31f);
    [SerializeField] private float nightIntensity;

    [Header("Transition")]
    [SerializeField] private float transitionSeconds = 5f; 
    [SerializeField] private bool startsAtNight = false;

    //Time Manager
    public TimeManager timeManager;

    private void Start()
    {
    }

    private void OnEnable()
    {
        timeManager.onMorning.AddListener(SetDay);
        timeManager.onEvening.AddListener(SetNight);
    }

    public void SetNight(DateTime time)
    {
        Debug.Log("IS night");
        globalLight.color = nightColor;
        globalLight.intensity = nightIntensity;
    }

    public void SetDay(DateTime time)
    {
        Debug.Log("IS day");
        globalLight.color = dayColor;
        globalLight.intensity = dayIntensity;
    }

}
