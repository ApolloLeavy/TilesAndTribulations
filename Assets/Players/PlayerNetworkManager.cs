using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class PlayerNetworkManager : NetworkComponent
{
    public bool isReady;
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
        if (flag == "RDY")
        {
            if (IsServer)
            {

                isReady = bool.Parse(value);
                gameMaster.GetComponent<GameMaster>().ReadyCheck();
                ReadyButton.isOn = isReady;
                SendUpdate("RDY", value);
            }
            if (IsClient)
            {
                isReady = bool.Parse(value);
                ReadyButton.isOn = isReady;

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
                    gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
                }
                    
            }
            if (IsClient)
            {
                classIndex = i;
                if(!gameMaster.classesTaken.Contains(i))
                gameMaster.classesTaken.Add(i);
                
            }
            if (IsLocalPlayer)
            {
                
                
            }
        }
        if(flag == "CLSBUT")
        {
            
            if(IsClient)
            {
                int k = int.Parse(value);
                if (gameMaster.classesTaken.Contains(k)) 
                    gameMaster.classesTaken.Remove(k);
                foreach (int o in gameMaster.classesTaken)
                {
                    classButtons[o].GetComponent<Button>().interactable = false;
                    if (k != -1)
                        classButtons[k].GetComponent<Button>().interactable = false;
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
    public void Name(string value)
    {
        IsDirty = true;
        playerName = value;
        SendUpdate("NAME", value);
    }
    public void Ready(bool value)
    {
        IsDirty = true;
        isReady = value;
        NameField.interactable = false;
        SendUpdate("RDY", value.ToString());
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

            while (IsServer)
            {

                if (IsDirty)
                {
                    SendUpdate("NAME", playerName);
                    SendUpdate("RDY", isReady.ToString());
                    SendUpdate("CLASS", classIndex.ToString());
                    SendUpdate("CLSBUT", "");
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
