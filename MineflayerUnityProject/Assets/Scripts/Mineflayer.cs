using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using UnityEngine;

public class Mineflayer : MonoBehaviour
{
    public static Mineflayer instance;
    public MineflayerBot botObject;
    public List<MineflayerBot> bots;
    
    public PlayerUI playerUI;
    public MinecraftWorld world;

    private WebSocket ws;

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        OpenSocket();
    }

    private void Update()
    {
        if (ws != null)
        {
            if (ws.ReadyState == WebSocketState.Closed)
            {
                ws.Connect();
            }
            else if (ws.ReadyState == WebSocketState.Open)
            {
                if (bots.Count == 0)
                {
                    Instantiate(botObject, world.transform);
                }
            }
        }
    }

    private void OnDestroy()
    {
        CloseSocket();
    }

    public void OpenSocket()
    {
        if (ws == null)
        {
            ws = new WebSocket("ws://localhost:3000");
            ws.OnMessage += OnMessage;
            ws.Connect();
        }
    }

    /// <summary>
    /// Sends <paramref name="message"/> to the server.
    /// </summary>
    /// <param name="message">Message to send.</param>
    public void Send(JsonMessage message)
    {
        Send(JsonUtility.ToJson(message));
    }

    public void Send(string st)
    {
        if (ws.ReadyState != WebSocketState.Open)
        {
            return;
        }

        ws.Send(st);
    }

    public void CloseSocket()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }

    private void OnMessage(object sender, MessageEventArgs data)
    {
        //Debug.Log("Server sent message: " + Encoding.UTF8.GetString(data.RawData));
        JsonMessage message = JsonUtility.FromJson<JsonMessage>(Encoding.UTF8.GetString(data.RawData));

        if (message.botNumber >= 0)
        {
            bots[message.botNumber].OnMessage(message);
        }
        else
        {
            switch(message.type) {
                case "entityupdate":
                {
                    EntityInfoMessage entityInfo = JsonUtility.FromJson<EntityInfoMessage>(message.message);
                    world.updates.Enqueue(entityInfo);
                    break;
                }
                case "time":
                {
                    if (int.TryParse(message.message, out int ticks))
                    {
                        world.SetTimeOfDay(ticks);
                    }
                    break;
                }
                case "chat":
                {
                    playerUI.chatPanel.AddMessage(message.message);
                    break;
                }
                default:
                {
                    Debug.Log("No handler for message type " + message.type);
                    break;
                }
            }
        }
    }
}
