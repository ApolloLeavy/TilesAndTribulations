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
                    ReadyButton.interactable = true;
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
    }

    public override void NetworkedStart()
    {
        isReady = false;
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        gameMaster.players.Add(this.gameObject);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
