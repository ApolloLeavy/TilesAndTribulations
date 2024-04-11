using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Knight : Player
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hp = 10;
        speed = 4;
        acd = .75f;
        qcd = 5;
        wcd = 5;
        ecd = 5;
        rcd = 5;
        isResisting = true;
    }
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if (flag == "Q")
        {
            if (IsServer)
            {
                if (canQ)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 19);
                    canQ = false;
                    StartCoroutine(Q());
                    SendUpdate("Q", canQ.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canQ = bool.Parse(value);
            }
        }
        if (flag == "W")
        {
            if (IsServer)
            {
                if (canQ)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 20);
                    canW = false;
                    StartCoroutine(W());
                    SendUpdate("W", canW.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canW = bool.Parse(value);
            }
        }
        if (flag == "E")
        {
            if (IsServer)
            {
                if (canE)
                {
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 21);

                    canE = false;
                    StartCoroutine(E());
                    SendUpdate("E", canE.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canW = bool.Parse(value);
            }
        }
        if (flag == "R")
        {
            if (IsServer)
            {
                if (canR)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 22);
                    canR = false;
                    StartCoroutine(R());
                    SendUpdate("R", canR.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canR = bool.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }
    public override void Update()
    {
        base.Update();
    }

}
