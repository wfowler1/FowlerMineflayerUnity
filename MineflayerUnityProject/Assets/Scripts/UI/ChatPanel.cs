using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatPanel : MonoBehaviour
{
    public PlayerUI parent;
    public TMP_InputField messageField;
    public Button submitButton;
    public RectTransform chatBackground;
    public TMP_Text chatView;

    private bool _open;
    public bool open
    {
        get
        {
            return _open;
        }
        set
        {
            _open = value;
            if (parent?.bot != null)
            {
                parent.bot.acceptingInput = !value;
            }

            messageField.gameObject.SetActive(value);
            submitButton.gameObject.SetActive(value);
            if (value)
            {
                messageField.Select();
                chatBackground.gameObject.SetActive(true);
            }
        }
    }
    private string chat = "";
    private float hideChatTimer = 0;

    public void Start()
    {
        open = false;
        hideChatTimer = 10;
    }

    public void Update()
    {
        hideChatTimer += Time.unscaledDeltaTime;

        if (!_open)
        {
            chatBackground.gameObject.SetActive(hideChatTimer < 10);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (open)
            {
                OnSubmitClicked();
            }
            else
            {
                open = true;
            }
        }
        
        chatView.text = chat;
    }

    /// <summary>
    /// When the submit button is clicked.
    /// </summary>
    public void OnSubmitClicked()
    {
        if (!string.IsNullOrWhiteSpace(messageField.text))
        {
            parent.bot.SendChat(messageField.text);
            messageField.text = "";
            open = false;
        }
    }

    public void AddMessage(string text)
    {
        chat += '\n' + text;
        hideChatTimer = 0;
        Debug.Log(text);
    }
}
