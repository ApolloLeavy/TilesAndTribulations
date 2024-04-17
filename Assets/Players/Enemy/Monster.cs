using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Monster : NetworkComponent
{
    public int hp;
    public float speed;
    public bool isSlowed;
    public bool isStunned;
    public bool isInvincible;
    public Player target;
    public List<Player> players;
    public bool canAct;
    public bool isAttack;
    public float acd;
    public Transform point;
    public int poisonStacks;
    public Animator myAnim;
    public bool isTaunted;
    public Vector3 distance;
    public Vector2 lastInput;
    public GameObject previewBlock;
    public List<Vector2[]> tileLibrary;
    public SpriteRenderer spriteRender;
    public Rigidbody myRig;
    public int statusAssist;
    public List<int> dmgAssist;
    public int attackNum;
    public override void HandleMessage(string flag, string value)
    {
        if(flag == "DMG")
        {
            if(IsClient)
            {
                hp -= int.Parse(value);
                
                StartCoroutine(AnimStart("isHit", attackNum));
            }
        }
        if(tag == "DIE")
        {
            if(IsClient)
            {
                StartCoroutine(Die());
            }
        }
        if (tag == "ATK")
        {
            if (IsClient)
            {
                StartCoroutine(AnimStart("isAttack", int.Parse(value)));
            }
        }
        if (tag == "MOVE")
        {
            if (IsClient)
            {
                lastInput.x = float.Parse(value);
            }
        }

    }

    public override void NetworkedStart()
    {
        
        if (IsServer)
            StartCoroutine(TakeAction());
    }
    public override IEnumerator SlowUpdate()
    {
        while (MyCore.IsConnected)
        {
            if (myRig.velocity.magnitude > 0)
            {
                myAnim.SetBool("isRun", true);
            }
            else
            {
                myAnim.SetBool("isRun", false);
            }
            if (lastInput.x < 0)
            {
                spriteRender.flipX = true;
            }
            else
            {
                spriteRender.flipX = false;
            }
            yield return new WaitForSeconds(.1f);
        }
        
    }
    public virtual void Start()
    {
        isSlowed = false;
        isStunned = false;
        isInvincible = false;
        isTaunted = false;
        myRig = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        poisonStacks = 0;
        attackNum = 0;
        statusAssist = -1;
        dmgAssist = new List<int>();
        dmgAssist.Add(0);
        dmgAssist.Add(0);
        dmgAssist.Add(0);
        dmgAssist.Add(0);
        isAttack = false;
        canAct = true;
        tileLibrary = new List<Vector2[]>();
        spriteRender = GetComponent<SpriteRenderer>();
        point = transform.GetChild(0);
        players.Add(GameObject.FindObjectOfType<Rogue>());
        players.Add(GameObject.FindObjectOfType<Ranger>());
        players.Add(GameObject.FindObjectOfType<Wizard>());
        players.Add(GameObject.FindObjectOfType<Knight>());
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, 1) });
    }
    public virtual void Update()
    {

    }
    public IEnumerator TakeAction()
    {
        if(!isStunned)
        {
            if (!isTaunted)
            {
                distance = Vector3.zero;
                foreach (Player p in players)
                {
                    if(!p.isDead)
                    {
                        if (distance == Vector3.zero || (myRig.position - p.myRig.position).magnitude < distance.magnitude)
                        {
                            distance = myRig.position - p.myRig.position;
                            target = p;
                        }
                    }
                }
            }
            distance *= -1;
            if(distance.x > 0 && distance.y > 0)
            {
                if(distance.x > distance.y)
                    lastInput = new Vector2(1, 0);
                else
                    lastInput = new Vector2(0, 1);
            }
            else if (distance.x < 0 && distance.y < 0)
            {
                if (distance.x < distance.y)
                    lastInput = new Vector2(-1, 0);
                else
                    lastInput = new Vector2(0, -1);
            }
            else if (distance.x > 0 && distance.y < 0)
            {
                if (distance.x > Mathf.Abs(distance.y))
                    lastInput = new Vector2(1, 0);
                else
                    lastInput = new Vector2(0, -1);
            }
            else
            {
                if (Mathf.Abs(distance.x) > distance.y)
                    lastInput = new Vector2(-1, 0);
                else
                    lastInput = new Vector2(0, 1);
            }
            SendUpdate("MOVE", lastInput.x.ToString());
            if(isAttack)
            {
                Attack();
                isAttack = false;
            }
            else
            {
                Move();
                isAttack = true;
            }
        }
        yield return new WaitForSeconds(acd);
        
    }
    public void PreviewMove(Vector2[] dir)
    {
        for (int i = 0; i < dir.Length; i++)
        {

            if (lastInput.y < 0)
            {
                point.position += new Vector3(-dir[i].x, -dir[i].y, 0);
            }
            else if (lastInput.x > 0)
            {
                point.position += new Vector3(dir[i].y, -dir[i].x, 0);
            }
            else if (lastInput.x < 0)
            {
                point.position += new Vector3(-dir[i].y, dir[i].x, 0);
            }
            else
            {
                point.position += new Vector3(dir[i].x, dir[i].y, 0);
            }
            GameObject.Instantiate(previewBlock, point.position, Quaternion.identity);

        }
        point.position = transform.position;


    }
    public IEnumerator Move(Vector2[] dir)
    {
        if (IsServer)
        {
            for (int i = 0; i < dir.Length; i++)
            {

                if (lastInput.y < 0)
                {
                    myRig.velocity = new Vector3(-dir[i].x, -dir[i].y, 0);
                }
                else if (lastInput.x > 0)
                {
                    myRig.velocity = new Vector3(dir[i].y, -dir[i].x, 0);
                }
                else if (lastInput.x < 0)
                {
                    myRig.velocity = new Vector3(-dir[i].y, dir[i].x, 0);
                }
                else
                {
                    myRig.velocity = new Vector3(dir[i].x, dir[i].y, 0);
                }
                myRig.velocity = myRig.velocity.normalized * speed;

                yield return new WaitForSecondsRealtime(1 / speed);

            }
            myRig.velocity = new Vector3(0, 0, 0);
            yield return new WaitForSecondsRealtime(acd);
            StartCoroutine(TakeAction());
        }
    }
    public void PreviewAbility(Vector2[] dir, int type)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (lastInput.y < 0)
            {
                point.position += new Vector3(-dir[i].x, -dir[i].y, 0);
            }
            else if (lastInput.x > 0)
            {
                point.position += new Vector3(dir[i].y, -dir[i].x, 0);
            }
            else if (lastInput.x < 0)
            {
                point.position += new Vector3(-dir[i].y, dir[i].x, 0);
            }
            else
            {
                point.position += new Vector3(dir[i].x, dir[i].y, 0);
            }
            MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity);
        }
        point.position = transform.position;
        
    }
    
    public IEnumerator Slow(float t)
    {
        isSlowed = true;
        yield return new WaitForSeconds(t);
        isSlowed = false;
    }
    public IEnumerator Taunt(float t)
    {
        isTaunted = true;
        yield return new WaitForSeconds(t);
        isTaunted = false;
    }
    public IEnumerator Stun(float t)
    {
        isStunned = true;
        yield return new WaitForSeconds(t);
        isStunned = false;
    }

    
    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(acd);
        StartCoroutine(TakeAction());
    }
    public virtual void Attack()
    {
        StartCoroutine(AnimStart("isAttack", attackNum));
        SendUpdate("ATK", attackNum.ToString());
        attackNum++;
        if (attackNum >= 3)
            attackNum = 0;
        StartCoroutine(AttackDelay());

    }
    public virtual void Move()
    {
        StartCoroutine(AnimStart("isRun", attackNum));
        Vector2[] piece = tileLibrary[Random.Range(0, (tileLibrary.Count))];
        StartCoroutine(Move(piece));

    }
    public IEnumerator AnimStart(string anim, int attack)
    {
        if(anim.Equals("isAttack"))
        {
            myAnim.SetBool(anim, true);
            myAnim.SetInteger("Attack", attack);
        }
        else
        {
            myAnim.SetBool(anim, true);
        }
        yield return new WaitForSeconds(1);
        if (anim == "isAttack")
        {
            myAnim.SetBool(anim, false);
            myAnim.SetInteger("Attack", attack);
        }
        else
        {
            myAnim.SetBool(anim, false);
        }
    }
    public IEnumerator Die()
    {
        GameMaster gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        gm.monsters.Remove(gameObject);
        int i = Random.Range(37, 46);
        
        if (gm.items.Remove(i))
        {
            MyCore.NetCreateObject(i, gm.Owner, myRig.position, Quaternion.identity);
        }
        else
            MyCore.NetCreateObject(37, gm.Owner, myRig.position, Quaternion.identity);
        hp = 0;
        StartCoroutine(AnimStart("isDead", attackNum));
        yield return new WaitForSeconds(1);
        if(IsServer)
        {
            MyCore.NetDestroyObject(MyId.NetId);
        }
    }
    public IEnumerator Poison()
    {
        if (poisonStacks > 0)
        {
            yield return new WaitForSeconds(1);
            Damage(poisonStacks, 0);
            poisonStacks -= 1;
            StartCoroutine(Poison());
            StartCoroutine(Slow(1));
        }

    }
    public void Poison(int poison)
    {
        
        poisonStacks += poison;
        if (poisonStacks == poison)
            StartCoroutine(Poison());
    }
    public IEnumerator Hit()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1);
        isInvincible = false;
    }
    public IEnumerator Knockback(Vector3 dir)
    {
        myRig.velocity += dir;
        yield return new WaitForSeconds(1);
        myRig.velocity -= dir;
    }
    public void Damage(int i, int p)
    {
        hp -= i;
        dmgAssist[p] += i;
        if (hp <= 0)
        {
            int assistMax = 0;
            int assistTarget = -1;
            hp = 0;
            SendUpdate("DIE", "");
            players[p].kills += 1;
            players[p].IsDirty = true;
            int j = 0;
            foreach(int q in dmgAssist)
            {
                if(q > assistMax)
                {
                    assistMax = q;
                    assistTarget = j;
                }
                j++;
            }
            if(assistTarget != -1)
            {
                players[assistTarget].assists += 1;
                players[assistTarget].IsDirty = true;
            }
            if(statusAssist != -1)
            {
                players[statusAssist].assists += 1;
                players[statusAssist].IsDirty = true;
            }
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(Hit());
            SendUpdate("DMG", i.ToString());
        }
        
    }
    public IEnumerator AssistTimer(int p)
    {
        yield return new WaitForSeconds(7);
        if(statusAssist == p)
        {
            statusAssist = -1;
        }
    }
    public void Assist(int p)
    {
        if(statusAssist != p)
        {
            statusAssist = p;
            StartCoroutine(AssistTimer(p));
        }

    }
    public void OnCollisionEnter(Collision collision)
    {
        if (IsServer && !isInvincible)
        { 
            if(collision.gameObject.tag == "Knight")
            {
                if (players[3].GetComponent<Knight>().chestplate)
                {
                    Damage(5, 3);
                }
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (IsServer && !isInvincible)
        {
            switch (other.tag)
            {
                case "Fireball":
                    {
                        Damage(15, 2);
                        break;
                    }
                case "IceSpike":
                    {
                        Damage(10, 2);
                        StartCoroutine(Slow(1));
                        Assist(0);
                        break;
                    }
                case "Caltrops":
                    {
                        Poison(players[0].GetComponent<Rogue>().poison);
                        Damage(5, 0);
                        Assist(0);
                        StartCoroutine(Slow(3));
                        break;
                    }
                case "CriticalStrike":
                    {
                        Poison(players[0].GetComponent<Rogue>().poison);
                        Damage(30, 0);
                        break;
                    }
                case "SleepingGas":
                    {
                        Poison(players[0].GetComponent<Rogue>().poison);
                        Assist(0);
                        StartCoroutine(Stun(1));
                        break;
                    }
                case "Tumble":
                    {
                        StartCoroutine(Knockback((transform.position - other.transform.position).normalized));
                        break;
                    }
                case "Gale":
                    {
                        Damage(8, 1);
                        break;
                    }
                case "LightningStrike":
                    {
                        Damage(5, 1);
                        StartCoroutine(Stun(4));
                        Assist(1);
                        break;
                    }
                case "RockSlide":
                    {
                        StartCoroutine(Slow(1));
                        Assist(1);
                        break;
                    }
                case "Arrow":
                    {
                        Damage(5, 1);
                        break;
                    }
                case "Spear":
                    {
                        Poison(players[0].GetComponent<Rogue>().poison);
                        Damage(5, 0);
                        break;
                    }
                case "LightShot":
                    {
                        Damage(5, 2);
                        break;
                    }
                case "BeyBlade":
                    {
                        Damage(10, 3);
                        break;
                    }
                case "ShieldBash":
                    {
                        Damage(5, 3);
                        StartCoroutine(Knockback((transform.position - other.transform.position).normalized));
                        break;
                    }
                case "Shout":
                    {
                        target = other.gameObject.GetComponent<Shout>().player;
                        StartCoroutine(Taunt(7));
                        Assist(3);
                        break;
                    }
            }
        }
    }
}
