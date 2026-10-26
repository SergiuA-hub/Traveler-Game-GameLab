using UnityEngine;
using System.Text;
using TMPro;
public class BulletLIstCreator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bulletList;
    [SerializeField] private string[] bulletItems;


    public void CreateBulletList(string[] il)
    {
        var str = new StringBuilder();
        foreach (var item in bulletItems)
        {
            str.Append("\u2022 <indent=1em>");
            str.Append(il);
            str.Append("</indent>\n");
        }
    }
}
