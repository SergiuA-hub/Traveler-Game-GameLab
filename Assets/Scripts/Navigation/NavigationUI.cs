using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class NavigationUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SettlementsManager settlementManager;
    public GameObject destinationList;
    public GameObject destinationPrefab;

    public NavigationArrow navigationArrow;
    public GameObject navigationArrowPanel;
    public GameObject noNavigationPanel;
    public NavigationListPanel navigationDestionationsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //destinationList.SetActive(false);
        foreach(var settlement in settlementManager.settlementsList)
        {
            GameObject go = Instantiate(destinationPrefab, destinationList.transform);
            go.GetComponent<DestinationPrefab>().setup(settlement.settlementSo.settlementName,settlement.settlmentGo.transform, this);
        }

        destinationList.SetActive(false);

        noNavigationPanel.SetActive(true);
        navigationArrowPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        destinationList.SetActive(true);
        navigationDestionationsPanel.stopClosing();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        navigationDestionationsPanel.startClosing();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        destinationList.SetActive(false);
        navigationArrow.SetDestination(null);
        noNavigationPanel.SetActive(true);
        navigationArrowPanel.SetActive(false);
    }

    public void selectedDestionation(Transform destination)
    {
        destinationList.SetActive(false);
        navigationArrow.SetDestination(destination);
        noNavigationPanel.SetActive(false);
        navigationArrowPanel.SetActive(true);
    }
}
