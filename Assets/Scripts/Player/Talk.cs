using UnityEngine;

public class Talk : MonoBehaviour
{
    public NpcChatBubble chat;

    public void say(string say)
    {
        chat.Show(say,1f);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
