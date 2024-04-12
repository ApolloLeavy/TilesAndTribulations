using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public abstract class Player : NetworkComponent
{
    public int hp;
    public int tileCount;
    public int maxTiles;
    public int tileGainSec;
    public bool isGenerating;
    public int qcd;
    public int wcd;
    public int ecd;
    public int rcd;
    public float acd;
    
    public Rigidbody myRig;
    public float speed;
    public Vector2 lastInput;
    public bool canPlace;
    public bool canAttack;
    public bool canQ;
    public bool canW;
    public bool canE;
    public bool canR;
    public List<int> tiles;
    public List<Vector2[]> tileLibrary; 
    public int activeTile;
    public bool isInvincible;
    public int kills;
    public int deaths;
    public int assists;
    public bool isDead;
    public float deathTimer;
    public GameObject npm;
    public bool isStunned;
    public bool isFlipped;
    public Transform point;
    public GameObject previewBlock;
    public bool isResisting;

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "MV" && activeTile != -1 && !isStunned)
        {
            if (IsServer && canPlace)
            {
                Debug.Log("Input");
                string[] tmp = value.Split(',');
                lastInput = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
                IsDirty = true;
                if ((lastInput.x > 0 || lastInput.x < 0) && (lastInput.y > 0 || lastInput.y < 0))
                {
                    lastInput.x = 0;
                    lastInput.y = 1;
                }
                SendUpdate("MV", value);
            }
            if (IsClient)
            {
                string[] tmp = value.Split(',');
                lastInput = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
                if ((lastInput.x > 0 || lastInput.x < 0) && (lastInput.y > 0 || lastInput.y < 0))
                {
                    lastInput.x = 0;
                    lastInput.y = 1;
                }
                PreviewMove(tileLibrary[tiles[activeTile]]);
            }
        }
        if(flag == "PLACE" && activeTile != -1 && !isStunned)
        {
            if(IsServer)
            {
                if(canPlace)
                {
                    
                    canPlace = false;
                    StartCoroutine(Move(tileLibrary[tiles[activeTile]]));
                    SendUpdate("PLACE", canPlace.ToString());
                }
                
            }
            if(IsLocalPlayer)
            {
                canPlace = bool.Parse(value);
                if(canPlace)
                    PreviewMove(tileLibrary[tiles[activeTile]]);

            }
        }
        if (flag == "ATTACK" && !isStunned)
        {
            if (IsServer)
            {
                if (canAttack)
                {
                    Debug.Log("Attack");
                    StartCoroutine(Attack());
                    canAttack = false;
                    SendUpdate("ATTACK", canAttack.ToString());
                }

            }
            if (IsLocalPlayer)
            {
                canAttack = bool.Parse(value);
            }
        }
        if (flag == "FLIP" && activeTile != -1)
        {
            if (IsServer)
            {
                isFlipped = !isFlipped;
                SendUpdate("FLIP", isFlipped.ToString());
            }
            if (IsLocalPlayer)
            {

                isFlipped = bool.Parse(value);
                PreviewMove(tileLibrary[tiles[activeTile]]);
            }
        }
       if(flag == "SPTL" && activeTile != -1)
        {
            if (IsServer)
            {

            }
            if(IsLocalPlayer)
            {
                tileCount--;
                tiles.Remove(tiles[activeTile]);
                if (activeTile == tileCount)
                    activeTile--;
                PreviewMove(tileLibrary[tiles[activeTile]]);
            }
        }
        if (flag == "DRTL")
        {
            if (IsServer)
            {

            }
            if (IsLocalPlayer)
            {
                tiles.Add(int.Parse(value));
                tileCount++;
                if (activeTile == -1)
                {
                    activeTile = 0;
                }
            }
        }
        if(flag == "CYCLE")
        {
            if(IsServer)
            {
                if (tileCount > 1 && canPlace)
                {
                    float t = float.Parse(value);
                    if (t > 0)
                    {
                        activeTile++;
                        if (activeTile >= tileCount)
                            activeTile = 0;
                        
                    }
                    else if (t < 0)
                    {
                        activeTile--;
                        if (activeTile <= -1)
                            activeTile = (tiles.Count - 1);
                    }
                    SendUpdate("CYCLE", activeTile.ToString());
                }
            }
            if (IsLocalPlayer)
            {
                activeTile = int.Parse(value);
                PreviewMove(tileLibrary[tiles[activeTile]]);
            }
        }
        if(flag == "STUN")
        {
            if(IsClient)
            {
                isStunned = bool.Parse(value);
            }
        }
    }
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            StartCoroutine(Draw());
        }
    }
    public override IEnumerator SlowUpdate()
    {
        if (IsConnected)
        {
            while (IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("ATTACK", canAttack.ToString());
                    SendUpdate("MV", lastInput.x + "," + lastInput.y);
                    SendUpdate("PLACE", canPlace.ToString());
                    SendUpdate("FLIP", isFlipped.ToString());
                    SendUpdate("CYCLE", activeTile.ToString());
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            if (IsLocalPlayer)
            {
                PreviewMove(tileLibrary[tiles[activeTile]]);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
            
        }
    }
    public virtual void Start()
    {
        myRig = GetComponent<Rigidbody>();
        maxTiles = 5;
        tileCount = 1;
        tileGainSec = 2;
        canAttack = true;
        canPlace = true;
        canQ = true;
        canW = true;
        canE = true;
        canR = true;
        tiles = new List<int>();
        tileLibrary = new List<Vector2[]>();
        tiles.Add(0);
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1) });
        activeTile = 0;
        isResisting = false;
        isFlipped = false;
        point = transform.GetChild(0);
        List<GameObject> indicatorList = new List<GameObject>();
    }
    public virtual void Update()
    {
        if (IsLocalPlayer)
        {
            Camera.main.transform.position = Vector3.Lerp(transform.position + new Vector3(0, 0, -9), myRig.position, speed / 2 * Time.deltaTime);
        }
    }
    public IEnumerator Draw()
    {
        
        if (tileCount < maxTiles)
        {
            int i = Random.Range(0, (tileLibrary.Count - 1));
            while (tiles.Contains(i))
            {
                
                i++;
                if (i == tileLibrary.Count)
                    i = 0;
            }
            tiles.Add(i);
            tileCount++;
            if (activeTile == -1)
            {
                activeTile = 0;
            }
            SendUpdate("DRTL", i.ToString());

        }    
            
        yield return new WaitForSeconds(2);
        StartCoroutine(Draw());
    }
    public IEnumerator Knockback()
    {
        isStunned = true;
        SendUpdate("STUN", isStunned.ToString());
        yield return new WaitForSeconds(1);
        isStunned = false;
        SendUpdate("STUN", isStunned.ToString());
    }
    public void OnAttack(InputAction.CallbackContext ev)
    {
        if (ev.started && IsLocalPlayer)
        {
            SendCommand("ATTACK", "");
        }
    }
    public IEnumerator Attack()
    {
        Attack2();
        yield return new WaitForSeconds(acd);
        canAttack = true;
        SendUpdate("ATTACK", canAttack.ToString());
    }
    public virtual void Attack2()
    {

    }
    public void OnPlace(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer && ev.performed)
        {
            Debug.Log("Place");
            SendCommand("PLACE", "");
            
        }
    }
    public void OnFlip(InputAction.CallbackContext ev)
    {
        if (ev.started)
            SendCommand("FLIP","");
    }
    public void OnMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer)
        {
            if (ev.started)
            {
                Debug.Log("Input");
                Vector2 tempCmd = ev.ReadValue<Vector2>();

                SendCommand("MV", tempCmd.x + "," + tempCmd.y);

            }
            if (ev.canceled)
            {
                SendCommand("MV", lastInput.x + "," + lastInput.y);
            }
        }
    }
    public IEnumerator Move(Vector2[] dir)
    {
        if (IsServer)
        {
            Debug.Log("Move");
            for (int i = 0; i < dir.Length;i++)
            {
                
                if(lastInput.y <0)
                {
                    myRig.velocity = new Vector3(-dir[i].x, -dir[i].y, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(dir[i].x, -dir[i].y, 0);
                }
                else if (lastInput.x > 0)
                {
                    myRig.velocity = new Vector3(dir[i].y, -dir[i].x, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(dir[i].y, dir[i].x, 0);
                }
                else if(lastInput.x < 0)
                {
                    myRig.velocity = new Vector3(-dir[i].y, dir[i].x, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(-dir[i].y, -dir[i].x, 0);
                }
                else
                {
                    myRig.velocity = new Vector3(dir[i].x, dir[i].y, 0);
                    if(isFlipped)
                        myRig.velocity = new Vector3(-dir[i].x, dir[i].y, 0);
                }
                myRig.velocity = myRig.velocity.normalized* speed;
                
                yield return new WaitForSecondsRealtime(1 / speed);
                
            }
            myRig.velocity = new Vector3(0, 0, 0);
            
            canPlace = true;
            
            SendUpdate("PLACE", canPlace.ToString());
        }
    }
    public void PreviewMove(Vector2[] dir)
    {
        for (int i = 0; i < dir.Length; i++)
        {

            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x, -dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y, -dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y, dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x, dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            GameObject.Instantiate(previewBlock, point.position, Quaternion.identity);

        }
        point.position = transform.position;


    }
    public void PreviewAbility(Vector2[] dir, int type)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x, -dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y, -dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y, dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x, dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity);
        }
        point.position = transform.position;
    }
    
    public void PreviewAbilityEnd(Vector2[] dir, int type)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (lastInput.y < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].x, -dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x > 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].y, -dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else if (lastInput.x < 0)
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(-dir[i].y, dir[i].x, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
            else
            {
                if (isFlipped)
                    dir[i].x *= -1;
                point.position += new Vector3(dir[i].x, dir[i].y, 0);
                if (isFlipped)
                    dir[i].x *= -1;
            }
        }
        MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity);
        point.position = transform.position;
    }
    public void CycleTile(InputAction.CallbackContext ev)
    {
        if(IsLocalPlayer && canPlace)
        SendCommand("CYCLE", ev.ReadValue<Vector2>().y.ToString());
        
    }
    public IEnumerator Q()
    {
        tiles.Remove(tiles[activeTile]);
        SendUpdate("SPTL", activeTile.ToString());
        if (activeTile == tileCount)
            activeTile--;
        tileCount--;
        yield return new WaitForSeconds(qcd);
        canQ = true;
        SendUpdate("Q", canQ.ToString());
    }
    public IEnumerator W()
    {
        tiles.Remove(tiles[activeTile]);
        SendUpdate("SPTL", activeTile.ToString());
        if (activeTile == tileCount)
            activeTile--;
        tileCount--;
        yield return new WaitForSeconds(wcd);
        canW = true;
        SendUpdate("W", canW.ToString());
    }
    public IEnumerator E()
    {
        tiles.Remove(tiles[activeTile]);
        SendUpdate("SPTL", activeTile.ToString());
        if (activeTile == tileCount)
            activeTile--;
        tileCount--;
        yield return new WaitForSeconds(ecd);
        canE = true;
        SendUpdate("E", canE.ToString());
    }
    public IEnumerator R()
    {
        tiles.Remove(tiles[activeTile]);
        SendUpdate("SPTL", activeTile.ToString());
        if (activeTile == tileCount)
            activeTile--;
        tileCount--;
        yield return new WaitForSeconds(rcd);
        canR = true;
        SendUpdate("R", canR.ToString());
    }
    public void OnQ(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("Q", "");
    }
    public void OnW(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("W", "");
    }
    public void OnE(InputAction.CallbackContext ev)
    {
        if (ev.started && canQ)
            SendCommand("E", "");
    }
    public void OnR(InputAction.CallbackContext ev)
    {
        if (ev.started && canW)
            SendCommand("R", "");
    }
    public IEnumerator TakeDamage()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1);
        isInvincible = false;
    }
    // Start is called before the first frame update
    
    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            switch (other.tag)
            {
                case "SkelSword":
                    {
                        break;
                    }
                case "Eyeball":
                    {
                        break;
                    }
                case "GobBomb":
                    {
                        break;
                    }
                case "Goblin":
                    {
                        break;
                    }
                case "Mushroom":
                    {
                        break;
                    }
                case "MushShot":
                    {
                        break;
                    }
                case "Skeleton":
                    {
                        break;
                    }
                case "Fireball":
                    {
                        break;
                    }
                case "Haste":
                    {
                        break;
                    }
                case "IceSpike":
                    {
                        break;
                    }
                case "Teleport":
                    {
                        break;
                    }
                case "Caltrops":
                    {
                        break;
                    }
                case "CriticalStrike":
                    {
                        break;
                    }
                case "SleepingGas":
                    {
                        break;
                    }
                case "Tumble":
                    {
                        break;
                    }
                case "Gale":
                    {
                        break;
                    }
                case "HealingSpirit":
                    {
                        break;
                    }
                case "LightningStrike":
                    {
                        break;
                    }
                case "RockSlide":
                    {
                        break;
                    }
                case "WaterBall":
                    {
                        break;
                    }
                case "BeyBlade":
                    {
                        break;
                    }
                case "Fortify":
                    {
                        break;
                    }
                case "ShieldBash":
                    {
                        break;
                    }
                case "Shout":
                    {
                        break;
                    }

            }
        }
    }
    // Update is called once per frame

}
