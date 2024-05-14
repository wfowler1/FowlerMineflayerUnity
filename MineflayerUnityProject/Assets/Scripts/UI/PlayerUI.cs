using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public MineflayerBot bot;
    public ChatPanel chatPanel;

    public void Start()
    {
        chatPanel.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!chatPanel.open)
            {
                chatPanel.open = true;
            }
        }
    }
}
