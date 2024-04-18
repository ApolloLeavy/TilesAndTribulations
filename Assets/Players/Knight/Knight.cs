using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Knight : Player
{
    public List<Vector2[]> knightLibrary;
    public bool chestplate;
    public bool helmet;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        knightLibrary = new List<Vector2[]>();
        hp = 40;
        hpM = 40;
        speed = 4;
        acd = 1.25f;
        qcd = 7;
        wcd = 8;
        ecd = 11;
        rcd = 15;
        isResisting = true;
        helmet = false;
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(-1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, 1)});
        knightLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, -1) });

        knightLibrary.Add(new Vector2[] { new Vector2(0, 1),new Vector2(1, 0),new Vector2(0, -1),new Vector2(-1, 0),
    new Vector2(-1, 0),new Vector2(-1, 0),new Vector2(0, 1),new Vector2(1, 0),new Vector2(0, 1),
    new Vector2(1, 0),new Vector2(0, 1),new Vector2(1, 0),new Vector2(0, -1),new Vector2(1, 0),
    new Vector2(0, -1),new Vector2(1, 0),new Vector2(0, -1),new Vector2(-1, 0),new Vector2(0, -1),
    new Vector2(-1, 0),new Vector2(0, -1),new Vector2(-1, 0),new Vector2(0, 1),new Vector2(-1, 0)});
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
                    if(tiles[activeTile] == 4 || tiles[activeTile] == 5)
                    {
                        PreviewAbility(knightLibrary[tiles[activeTile] - 4], 19);
                    }
                    else
                    {
                        PreviewAbility(tileLibrary[tiles[activeTile]], 19);
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
                    if (tiles[activeTile] == 5 || tiles[activeTile] == 6)
                    {
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 46);
                        PreviewAbility(knightLibrary[tiles[activeTile] - 4], 20);
                    }
                    else
                    {
                        PreviewAbilityEnd(tileLibrary[tiles[activeTile]], 46);
                        PreviewAbility(tileLibrary[tiles[activeTile]], 20);
                    }
                    canW = false;
                    StartCoroutine(W());
                    SendUpdate("W", canW.ToString());
                }
            }
            if (IsClient)
            {
                StartCoroutine(AnimStart("isDash"));
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
                    if (tiles[activeTile] == 5 || tiles[activeTile] == 6)
                    {
                        PreviewAbility(knightLibrary[tiles[activeTile - 4]], 21);
                    }
                    else
                    {
                        PreviewAbility(tileLibrary[tiles[activeTile]], 21);
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
                    if (tiles[activeTile] == 5 || tiles[activeTile] == 6)
                    {
                        PreviewAbility(knightLibrary[tiles[activeTile - 4]], 22);
                    }
                    else
                    {
                        PreviewAbility(tileLibrary[tiles[activeTile]], 22);
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
        GameObject o = MyCore.NetCreateObject(35, Owner, myRig.position + new Vector3(lastInput.x, lastInput.y, 0), Quaternion.Euler(-lastInput.x * 90, 0, 0));
        o.GetComponent<Rigidbody>().velocity = new Vector3(lastInput.x, lastInput.y, 0).normalized * 1.5f;

    }
    public override void NetworkedStart()
    {
        base.NetworkedStart();
        myRig.position += new Vector3(0, 1, 0);
    }
    public override void Update()
    {
        base.Update();
    }
    public new IEnumerator TakeDamage(int i)
    {
        if (!helmet)
        {
            hp -= i;
            if (hp > 0)
            {
                SendUpdate("HP", i.ToString());
                isInvincible = true;
                yield return new WaitForSeconds(1);
                isInvincible = false;
            }
            else
            {
                StartCoroutine(Die());

            }
        }
        else
        {
            StartCoroutine(HelmetTimer());
        }
    }
    public IEnumerator HelmetTimer()
    {
        helmet = false;
        yield return new WaitForSeconds(7);
        helmet = true;
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Helmet":
                    {
                        helmet = true;
                        break;
                    }
                case "Chestplate":
                    {
                        chestplate = true;
                        break;
                    }
            }
        }
        
    }
}
