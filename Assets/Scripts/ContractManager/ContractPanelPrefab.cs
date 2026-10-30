using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContractPanelPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image contractResIcon;
    public Image contractTypeIcon;
    public TMP_Text contractTypeText;

    private Contract contract;

    [Header("Contract Colors")]
    [SerializeField] private Color exportColor = new Color32(84, 122, 69, 255);
    [SerializeField] private Color importColor = new Color32(163, 78, 66, 255);
    [SerializeField] private float hoverScale = 1.1f;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private void Start()
    {
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
    }
    public void setup(Contract c)
    {
        contract = c;

        contractResIcon.sprite = contract.contractRequest.resourceSO.sprite;

        setContractType(contract);
    }

    public void setContractType(Contract contract)
    {
        if (contract.contractType == ContractType.Export)
        {
            contractTypeIcon.color = exportColor;
            contractTypeText.text = "Export";
        }
        else if (contract.contractType == ContractType.Import)
        {
            contractTypeIcon.color = importColor;
            contractTypeText.text = "Import";
        }

        contractTypeText.color = new Color32(245, 231, 200, 255);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localRotation = originalRotation;
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameServices.Instance.contractManager.showContractDetails(contract);
    }
}
