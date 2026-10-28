using UnityEngine;
using System.Text;
using TMPro;
public class BulletLIstCreator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bulletList;
    
    private void Awake()
    {
        bulletList = GetComponent<TextMeshProUGUI>();
    }
    public void CreateBulletList(string[] il)
    {
        var str = new StringBuilder();
        foreach (var item in il)
        {
            str.Append("\u2022 <indent=1em>");
            str.Append(item);
            str.Append("</indent>\n");
        }
       bulletList.text = str.ToString();
    }
}
