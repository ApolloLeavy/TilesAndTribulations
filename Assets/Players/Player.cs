using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public abstract class Player : NetworkComponent
{
    public string pname;
    public int hp;
    public int hpM;
    public int tileCount;
    public int maxTiles;
    public int tileGainSec;
    public bool isGenerating;
    public int qcd;
    public int wcd;
    public int ecd;
    public int rcd;
    public float acd;
    public float tcd;
    public List<GameObject> indicatorList;
    public Animator myAnim;
    public Rigidbody myRig;
    public SpriteRenderer spriteRender;
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
    public List<Animator> cooldownsAnim;
    public List<Text> cooldownsNum;
    public GameObject HUDCanvas;
    public int activeTile;
    public bool isInvincible;
    public int kills;
    public int deaths;
    public int assists;
    public bool isDead;
    public float deathTimer;
    public PlayerNetworkManager npm;
    public bool isStunned;
    public bool isFlipped;
    public Transform point;
    public GameObject previewBlock;
    public GameObject hpBar;
    public Text nameUI;
    public bool isResisting;
    public int healingSpirit;
    public bool ring;

    public bool haste;

    public override void HandleMessage(string flag, string value)
    {
        if(flag == "CHRNM")
        {
            if (IsClient)
            {
                pname = value;
                nameUI.text = value;
            }
        }
        if(flag == "K")
        {
            if (IsClient)
            {
                kills = int.Parse(value);
            }
        }
        if (flag == "D")
        {
            if (IsClient)
            {
                deaths = int.Parse(value);
            }
        }
        if (flag == "A")
        {
            if (IsClient)
            {
                assists = int.Parse(value);
            }
        }
        if ((flag == "Q"|| flag == "W" || flag == "E" || flag == "R") && !isStunned && !isDead)
        {
            StartCoroutine(AnimStart("isAttack"));

        }
        if(flag == "Q" && canQ && !ring)
        {
            if (IsServer)
            {
                SendUpdate("CD", "0," + qcd.ToString());
            }
        }
        if (flag == "W" && canW && !ring)
        {
            if (IsServer)
            {
                SendUpdate("CD", "1," + wcd.ToString());
            }
        }
        if (flag == "E" && canE && !ring)
        {
            if (IsServer)
            {
                SendUpdate("CD", "2," + ecd.ToString());
            }
        }
        if (flag == "R" && canR && !ring)
        {
            if (IsServer)
            {
                SendUpdate("CD", "3," + rcd.ToString());
            }
        }
        if (flag == "MV" && activeTile != -1 && !isStunned && !isDead)
        {
            if (IsServer && canPlace)
            {
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
        if(flag == "PLACE" && activeTile != -1 && !isStunned && !isDead)
        {
            if(IsServer)
            {
                if(canPlace)
                {
                    canPlace = false;
                    
                    StartCoroutine(Move(tileLibrary[tiles[activeTile]]));
                    SendUpdate("PLACE", canPlace.ToString());
                    tiles.Remove(tiles[activeTile]);
                    SendUpdate("SPTL", activeTile.ToString());
                    if (activeTile == tileCount - 1)
                        activeTile--;
                    tileCount--;
                }
                
            }
            if(IsLocalPlayer)
            {
                canPlace = bool.Parse(value);
                if(canPlace)
                {
                    myAnim.SetBool("isRun", false);
                    PreviewMove(tileLibrary[tiles[activeTile]]);
                }
                else
                {
                    myAnim.SetBool("isRun", true);
                }
                    

            }
        }
        if(flag == "CD")
        {
            if (IsLocalPlayer)
            {
                string[] t = value.Split(',');
                int index = int.Parse(t[0]);
                int cooldown = int.Parse(t[1]);
                cooldownsAnim[index].speed = (1 / cooldown);
                cooldownsAnim[index].Play("CDCircle",0);
                StartCoroutine(Cooldown(index,cooldown,cooldown));
            }
            

        }
        if (flag == "ATTACK" && !isStunned && !isDead)
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
                if(!canAttack)
                {
                    StartCoroutine(AnimStart("isAttack"));
                }
            }
        }
        if (flag == "FLIP" && activeTile != -1 && !isDead)
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
       if(flag == "SPTL" && activeTile != -1 && !isDead)
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
        if(flag == "CYCLE" && !isDead)
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
        if(flag == "STUN" && !isDead)
        {
            if(IsClient)
            {
                isStunned = bool.Parse(value);
            }
        }
        if (flag == "HP" && !isDead)
        {
            if (IsClient)
            {
                int t= int.Parse(value);
                if(t < hp)
                {
                    StartCoroutine(AnimStart("isHit"));
                }
                hp = t;
                hpBar.transform.localScale.Set(2.5f * hp / hpM,2.5f,1);
            }
        }
        if (flag == "DIE" && !isDead)
        {
            if (IsClient)
            {
                isDead = bool.Parse(value);
                StartCoroutine(Die());
            }
        }

    }
    public override void NetworkedStart()
    {
        if (IsServer)
        {
            StartCoroutine(Draw());
            StartCoroutine(Healing());
        }
        if(IsClient && !IsLocalPlayer)
        {
            HUDCanvas.SetActive(false);
        }
        if (IsLocalPlayer)
        {
            cooldownsNum[0].text = qcd.ToString();
            cooldownsNum[1].text = wcd.ToString();
            cooldownsNum[2].text = ecd.ToString();
            cooldownsNum[3].text = rcd.ToString();
        }
    }
    public override IEnumerator SlowUpdate()
    {
        while (MyCore.IsConnected)
        {
            if (IsServer)
            {
                if (IsDirty)
                {
 
                    SendUpdate("MV", lastInput.x + "," + lastInput.y);
                    SendUpdate("PLACE", canPlace.ToString());
                    SendUpdate("FLIP", isFlipped.ToString());
                    SendUpdate("CYCLE", activeTile.ToString());
                    SendUpdate("K", kills.ToString());
                    SendUpdate("D", deaths.ToString());
                    SendUpdate("A", assists.ToString());
                    IsDirty = false;
                }
            }
            if (IsLocalPlayer)
            {
                if (lastInput.x < 0)
                {
                    spriteRender.flipX = true;
                }
                else
                {
                    spriteRender.flipX = false;
                }
                if(activeTile >=0 && activeTile < tiles.Count)
                PreviewMove(tileLibrary[tiles[activeTile]]);

            }
            if ((isDead || canPlace) && !isStunned)
            {
                myRig.velocity = Vector3.zero;
            }

            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
    }
    public virtual void Start()
    {
        myRig = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
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
        tcd = 2;
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
        ring = false;


        List<GameObject> indicatorList = new List<GameObject>();
    }
    public virtual void Update()
    {
        if (IsLocalPlayer)
        {
            Camera.main.transform.position = Vector3.Lerp(transform.position + new Vector3(0, 0, -9), myRig.position, speed / 2 * Time.deltaTime);
        }
    }
    public IEnumerator Cooldown(int index, float cooldown, float maxcd)
    {
        
        if(cooldown >0)
        {
            cooldownsNum[index].text = cooldown.ToString();
            yield return new WaitForSeconds(1);
            StartCoroutine(Cooldown(index, cooldown - 1, maxcd));
        }
        else
        {
            cooldownsNum[index].text = "RDY";
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
            
        yield return new WaitForSeconds(tcd);
        StartCoroutine(Draw());
    }
    
    public IEnumerator Stun()
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
        foreach (GameObject o in indicatorList)
        {
            GameObject.Destroy(o);
        }
        indicatorList.Clear();
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
            indicatorList.Add(GameObject.Instantiate(previewBlock, point.position, Quaternion.identity));

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
        if (activeTile == tileCount-1)
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
        if (ev.started && canE)
            SendCommand("E", "");
    }
    public void OnR(InputAction.CallbackContext ev)
    {
        if (ev.started && canR)
            SendCommand("R", "");
    }
    public IEnumerator TakeDamage(int i)
    {
        if(!isInvincible && !isDead)
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
        
        
    }
    public IEnumerator AnimStart(string anim)
    {
        
            myAnim.SetBool(anim, true);
        
        yield return new WaitForSeconds(1);
        
            myAnim.SetBool(anim, false);
        
    }
    public IEnumerator Die()
    {
        deaths++;
        IsDirty = true;
        myRig.velocity = Vector3.zero;
        isDead = true;
        isStunned = true;
        if (IsServer)
            SendUpdate("DIE", isDead.ToString());
        if (IsClient)
        {
            
            StartCoroutine(AnimStart("isDead"));
            if(IsLocalPlayer)
            {
                foreach (GameObject o in indicatorList)
                {
                    GameObject.Destroy(o);
                }
                indicatorList.Clear();
            }
        }
        yield return new WaitForSeconds(1);
        if (IsClient)
        {

            GetComponent<SpriteRenderer>().enabled = false;
        }
        yield return new WaitForSeconds(8);
        hp = hpM;
        SendUpdate("HP", hp.ToString());
        isStunned = false;
        isDead = false;
        if (IsServer)
            SendUpdate("DIE", isDead.ToString());
        if (IsClient)
        {

            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    public IEnumerator Fortify()
    {
        isResisting = true;
        yield return new WaitForSeconds(10);
        isResisting = false;
    }
    public IEnumerator Healing()
    {
        if( hp<hpM)
        {
            hp += 1 + healingSpirit;
            if (hp > hpM)
                hp = hpM;
            SendUpdate("HP", hp.ToString());
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(Healing());
    }
    public IEnumerator Haste()
    {
        if (!haste)
        {
            haste = true;
            speed *= 2;
            acd /= 2;
            tcd /= 2;
            yield return new WaitForSeconds(10);
            speed /= 2;
            acd *= 2;
            tcd *= 2;
            haste = false;
        }
        
    }
    // Start is called before the first frame update
    public void OnTriggerStay(Collider other)
    {
        if (IsServer)
        {
            switch(other.tag)
            {
                case "HealingSpirit":
                    {
                        healingSpirit = 2;
                        break;
                    }
            }
        }
    }
    public IEnumerator Knockback(Vector3 dir)
    {
        isStunned = true;
        myRig.velocity += dir;
        yield return new WaitForSeconds(1);
        isStunned = false;
        myRig.velocity -= dir;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (IsServer && !isDead && !isInvincible)
        {
            switch (collision.gameObject.tag)
            {
                case "Skeleton":
                case "Eyeball":
                case "Goblin":
                case "Mushroom":
                    {
                        if (!isResisting)
                        {
                            StartCoroutine(TakeDamage(5));
                            StartCoroutine(Knockback((transform.position - collision.transform.position).normalized));
                            StartCoroutine(Stun());
                        }
                        break;
                    }
            }
        }
    }
 
    public virtual void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            switch (other.tag)
            {
                
                case "EyeShot":
                    {
                        StartCoroutine(TakeDamage(8));
                        break;
                    }
                case "GobBomb":
                    {
                        StartCoroutine(TakeDamage(5));
                        StartCoroutine(Stun());
                        break;
                    }
                case "GobBombExplosion":
                    {
                        StartCoroutine(TakeDamage(10));
                        StartCoroutine(Knockback((transform.position - other.transform.position).normalized));
                        break;
                    }
                case "SkelSword":
                    {
                        StartCoroutine(TakeDamage(2));
                        
                        StartCoroutine(Stun());
                        break;
                    }
                case "MushShot":
                    {
                        StartCoroutine(TakeDamage(4));
                        break;
                    }
                case "Haste":
                    {
                        StartCoroutine(Haste());
                        break;
                    }
                case "Fortify":
                    {
                        StartCoroutine(Fortify());
                        break;
                    }

                case "Potion":
                    {
                        hp += 10;
                        if (hp > hpM)
                            hp = hpM;
                        SendUpdate("HP", hp.ToString());
                        break;
                    }                
            }
        }
        
    }
    // Update is called once per frame

}
