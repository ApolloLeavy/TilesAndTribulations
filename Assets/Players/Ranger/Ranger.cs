using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Ranger : Player
{
    public bool gatito;
    public bool fairy;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        gatito = false;
        fairy = false;
        hp = 25;
        speed = 4;
        acd = 2.5f;
        qcd = 7;
        wcd = 7;
        ecd = 8;
        rcd = 13;
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, 1)});

        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 1)});
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 11);
                    isFlipped = !isFlipped;
                    if(!gatito)
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 11);
                    else
                        PreviewAbility(tileLibrary[tiles[activeTile]], 11);
                    isFlipped = !isFlipped;
                    if (fairy)
                    {
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 11);
                        lastInput *= -1;
                        StartCoroutine(FairyTimer());
                    }
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 12);
                    isFlipped = !isFlipped;
                    if (!gatito)
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 12);
                    else
                        PreviewAbility(tileLibrary[tiles[activeTile]], 12);
                    isFlipped = !isFlipped;
                    if (fairy)
                    {
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 12);
                        lastInput *= -1;
                        StartCoroutine(FairyTimer());
                    }
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 13);
                    isFlipped = !isFlipped;
                    if (!gatito)
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 13);
                    else
                        PreviewAbility(tileLibrary[tiles[activeTile]], 13);
                    isFlipped = !isFlipped;
                    if (fairy)
                    {
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 13);
                        lastInput *= -1;
                        StartCoroutine(FairyTimer());
                    }
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
                    PreviewAbility(tileLibrary[tiles[activeTile]], 14);
                    isFlipped = !isFlipped;
                    if (!gatito)
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 14);
                    else
                        PreviewAbility(tileLibrary[tiles[activeTile]], 14);
                    isFlipped = !isFlipped;
                    if(fairy)
                    {
                        lastInput *= -1;
                        PreviewAbility(tileLibrary[tiles[activeTile]], 14);
                        lastInput *= -1;
                        StartCoroutine(FairyTimer());
                    }
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
        GameObject o = MyCore.NetCreateObject(32, Owner, myRig.position + new Vector3(lastInput.x, lastInput.y, 0));
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * 3;
    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        myRig.position += new Vector3(0, -1, 0);
    }
    public override void Update()
    {
        base.Update();
    }
    public IEnumerator FairyTimer()
    {
        fairy = false;
        yield return new WaitForSeconds(7);
        fairy = true;
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Gatito":
                    {
                        gatito = true;
                        break;
                    }
                case "Fairy":
                    {
                        rcd = 10;
                        fairy = true;
                        break;
                    }
            }
        }
    }
}
