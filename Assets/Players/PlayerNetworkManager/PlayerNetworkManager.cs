using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class PlayerNetworkManager : NetworkComponent
{
    public bool isReady;
    public bool gameStarted = false;
    public string playerName = "";
    public InputField NameField;
    public Toggle ReadyButton;
    public GameMaster gameMaster;
    public List<GameObject> classButtons;
    public GameObject gameCanvas;
    public int classIndex = -1;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "NAME" && !(value.Equals("")))
        {
            if (IsServer)
            {

                playerName = value;
                NameField.text = value;
                SendUpdate("NAME", value);

            }
            if (IsClient)
            {
                playerName = value;
                NameField.text = value;

                if (IsLocalPlayer)
                {
                    if (playerName != "")
                        ReadyButton.interactable = true;
                    else
                        ReadyButton.interactable = false;
                }
            }
        }
        if(flag == "START")
        {
            gameStarted = true;
        }
        if (flag == "RDY")
        {
            isReady = bool.Parse(value);
            ReadyButton.isOn = isReady;
            if (IsServer)
            {
                gameMaster.GetComponent<GameMaster>().ReadyCheck();
                SendUpdate("RDY", value);
            }
            if(IsLocalPlayer)
            {
                if(isReady)
                {
                    NameField.interactable = false;
                    foreach(GameObject b in classButtons)
                    {
                        b.GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    NameField.interactable = true;
                }
            }
        }
        if (flag == "CLASS")
        {
            
            int i = int.Parse(value);
            if (IsServer)
            {
                
                if (!gameMaster.classesTaken.Contains(i))
                {
                    gameMaster.SelectClass(i, classIndex);
                    
                    classIndex = i;
                    SendUpdate("CLASS", value);
                    SendUpdate("CLSBUT", string.Join(',', gameMaster.classesTaken));
                    gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
                }
                    
            }
            if (IsClient)
            {
                classIndex = i;
                
            }
            
        }
        if(flag == "CLSBUT")
        {
            string[] i = value.Split(',');
            gameMaster.classesTaken.Clear();
            if(IsClient)
            {
                foreach (string s in i)
                {
                    gameMaster.classesTaken.Add(int.Parse(s));
                    
                }
                

            }
            
        }
    }

    public override void NetworkedStart()
    {
        if (IsClient && !IsLocalPlayer)
        {
            ReadyButton.interactable = false;
            NameField.interactable = false;
            foreach (GameObject cb in classButtons)
            {
                cb.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void OnDestroy()
    {
        gameMaster.classesTaken.Remove(classIndex);
        gameMaster.players.Remove(gameObject);
    }
    public void Name(string value)
    {
        IsDirty = true;
        playerName = value;
        SendCommand("NAME", value);
    }
    public void Ready(bool value)
    {
        IsDirty = true;
        SendCommand("RDY", value.ToString());
    }
    public void ClassSelect(int i)
    {
        IsDirty = true;
        SendCommand("CLASS", i.ToString());
    }
    public override IEnumerator SlowUpdate()
    {
        while (IsConnected)
        {

            while(!gameStarted)
            {
                while (IsServer)
                {

                    if (IsDirty)
                    {
                        SendUpdate("NAME", playerName);
                        SendUpdate("RDY", isReady.ToString());
                        SendUpdate("CLASS", classIndex.ToString());
                        SendUpdate("CLSBUT", string.Join(',', gameMaster.classesTaken));
                        IsDirty = false;
                    }

                    yield return new WaitForSeconds(.1f);

                }
                if (IsLocalPlayer && !isReady)
                {
                    for (int o = 0; o < 4; o++)
                    {
                        if (gameMaster.classesTaken.Contains(o))
                            classButtons[o].GetComponent<Button>().interactable = false;
                        else
                            classButtons[o].GetComponent<Button>().interactable = true;
                    }
                }
                yield return new WaitForSeconds(.1f);
            }
            while(IsServer)
            {
                if (IsDirty)
                {
                    
                    IsDirty = false;
                }
                yield return new WaitForSeconds(.1f);
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("Players").transform);
        isReady = false;
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        gameMaster.players.Add(this.gameObject);
        ReadyButton.interactable = false;
        gameCanvas = GameObject.Find("GameCanvas");


    }

    // Update is called once per frame
    void Update()
    {

    }
}
