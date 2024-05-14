using System;
using UnityEngine;

public class MineflayerBot : Entity
{
    private int botNumber;
    public float sensitivity = 1;
    public bool acceptingInput;

    void Start()
    {
        type = "player";
        botNumber = Mineflayer.instance.bots.Count;
        Mineflayer.instance.bots.Add(this);
        Mineflayer.instance.playerUI.bot = this;
        acceptingInput = true;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            acceptingInput = !acceptingInput;
        }

        if (acceptingInput)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ProcessInput(KeyCode.W, "forward");
            ProcessInput(KeyCode.S, "back");
            ProcessInput(KeyCode.A, "left");
            ProcessInput(KeyCode.D, "right");
            ProcessInput(KeyCode.Space, "jump");
            ProcessInput(KeyCode.LeftShift, "sprint");
            ProcessInput(KeyCode.LeftControl, "sneak");

            if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
            {
                SendLook(pitch + Input.GetAxisRaw("Mouse Y") * sensitivity, yaw - Input.GetAxisRaw("Mouse X") * sensitivity);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ProcessInput(KeyCode keyCode, string control)
    {
        if (Input.GetKeyDown(keyCode))
        {
            SendControl(control, true);
        }
        else if (Input.GetKeyUp(keyCode))
        {
            SendControl(control, false);
        }
    }

    public void OnMessage(JsonMessage message)
    {
        if (message.botNumber != botNumber)
        {
            return;
        }

        switch(message.type) {
            case "entityupdate":
            {
                EntityInfoMessage entityInfo = JsonUtility.FromJson<EntityInfoMessage>(message.message);
                position = entityInfo.position;
                pitch = entityInfo.pitch * 180f / Mathf.PI;
                yaw = entityInfo.yaw * 180f / Mathf.PI;
                break;
            }
            default:
            {
                Debug.Log("No handler for message type " + message.type);
                break;
            }
        }
    }

    public void SendChat(string message)
    {
        Mineflayer.instance.Send(JsonUtility.ToJson(
            new JsonMessage {
                botNumber = botNumber,
                type = "chat",
                message = message
            }
        ));
    }

    public void SendControl(string control, bool state)
    {
        Mineflayer.instance.Send(JsonUtility.ToJson(
            new JsonMessage {
                botNumber = botNumber,
                type = "control",
                message = control + ":" + state
            }
        ));
    }

    public void SendLook(float pitch, float yaw)
    {
        Mineflayer.instance.Send(JsonUtility.ToJson(
            new JsonMessage {
                botNumber = botNumber,
                type = "look",
                message = (Mathf.Clamp(pitch, -90, 90) * Mathf.PI / 180f) + "," + (yaw * Mathf.PI / 180f)
            }
        ));
    }
}
