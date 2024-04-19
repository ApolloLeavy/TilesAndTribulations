using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Rogue : Player
{

    public int poison;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        poison = 1;
        hp = 30;
        hpM = 30;
        speed = 4;
        acd = .75f;
        qcd = 5;
        wcd = 5;
        ecd = 10;
        rcd = 8;
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1) });

        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0) });
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
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 7);
                    canQ = false;
                    StartCoroutine(Q());
                    SendUpdate("Q", canQ.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canQ = bool.Parse(value);
                if (!canQ)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
        if (flag == "W")
        {
            if (IsServer)
            {
                if (canW)
                {
                    PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 8);
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 9);

                    canE = false;
                    StartCoroutine(E());
                    SendUpdate("E", canE.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canE = bool.Parse(value);
                if (!canE)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
        if (flag == "R")
        {
            if (IsServer)
            {
                if (canR)
                {
                    PreviewAbility(tileLibrary[tiles[activeTile]], 10);
                    canR = false;
                    StartCoroutine(R());
                    SendUpdate("R", canR.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                canR = bool.Parse(value);
                if (!canR)
                {
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
            }
        }
    }
    public override void Attack2()
    {
        GameObject o = MyCore.NetCreateObject(34, Owner, myRig.position + new Vector3(lastInput.x, lastInput.y, 0), Quaternion.Euler(0,0, lastInput.y * 90));
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * 2.5f;
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        myRig.position += new Vector3(-1, 0, 0);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Boots":
                    {
                        SendUpdate("ITEM", "");
                        speed += 1;
                        qcd -= 1;
                        wcd -= 1;
                        ecd -= 1;
                        rcd -= 1;
                        break;
                    }
                case "Vial":
                    {
                        SendUpdate("ITEM", "");
                        poison = 2;
                        break;
                    }
            }
        }
    }
}
