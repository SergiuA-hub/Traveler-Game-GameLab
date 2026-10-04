using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum RestOptions
{
    Nap,
    TilMorning,
    TilFull
}

public class RestOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text restTime;
    public TimeManager timeManager;
    public SleepingBagUI callback;
    public RestOptions currentRestOption;
    public Image backgroundImage;
    
    private Color normalColor = new Color32(0xEB, 0xD1, 0xA9, 100); // #EBD1A9
    private Color hoverColor = new Color32(0xF3, 0xDF, 0xB9, 100); // #F3DFB9
    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void updateRestTime(string infoToShow)
    {
        restTime.text = infoToShow;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = hoverColor;
        callback.showOptionOutcome(currentRestOption);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
        
        callback.hideOptionOutcome();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        callback.restSelected(currentRestOption);
    }
}
