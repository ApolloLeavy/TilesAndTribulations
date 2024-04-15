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
    public Player[] players;
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
        if(IsServer)
            StartCoroutine(TakeAction());
    }
    public override IEnumerator SlowUpdate()
    {
        if(myRig.velocity.magnitude > 0)
        {
            myAnim.SetBool("isRun", true);
        }
        else
        {
            myAnim.SetBool("isRun", false);
        }
        if(lastInput.x < 0)
        {
            spriteRender.flipX = true;
        }
        else
        {
            spriteRender.flipX = false;
        }
        yield return new WaitForSeconds(.1f);
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
        isAttack = false;
        canAct = true;
        tileLibrary = new List<Vector2[]>();
        spriteRender = GetComponent<SpriteRenderer>();
        point = transform.GetChild(0);
        players = GameObject.FindObjectsOfType<Player>();
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

    public IEnumerator Poison()
    {
        if (poisonStacks > 0)
        {
            yield return new WaitForSeconds(1);
            hp -= poisonStacks;
            poisonStacks -= 1;
            StartCoroutine(Poison());
            StartCoroutine(Slow(1));
        }
        
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
        Vector2[] piece = tileLibrary[Random.Range(0, (tileLibrary.Count - 1))];
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
        hp = 0;
        StartCoroutine(AnimStart("isDead", attackNum));
        yield return new WaitForSeconds(1);
        if(IsServer)
        {
            MyCore.NetDestroyObject(MyId.NetId);
        }
    }
    public void Posion()
    {
        poisonStacks++;
        if (poisonStacks == 1)
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
    public void Damage(int i)
    {
        hp -= i;
        if (hp <= 0)
        {
            hp = 0;
            SendUpdate("DIE", "");
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(Hit());
            SendUpdate("DMG", i.ToString());
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
                        Damage(15);
                        break;
                    }
                case "IceSpike":
                    {
                        Damage(10);
                        StartCoroutine(Slow(1));
                        break;
                    }
                case "Caltrops":
                    {
                        Poison();
                        Damage(5);
                        StartCoroutine(Slow(3));
                        break;
                    }
                case "CriticalStrike":
                    {
                        Poison();
                        Damage(30);
                        break;
                    }
                case "SleepingGas":
                    {
                        Poison();
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
                        Damage(8);
                        break;
                    }
                case "LightningStrike":
                    {
                        Damage(5);
                        StartCoroutine(Stun(4));
                        break;
                    }
                case "RockSlide":
                    {
                        StartCoroutine(Slow(1));
                        break;
                    }
                case "WaterBall":
                    {
                        Damage(5);
                        break;
                    }
                case "ThrowingKnife":
                    {
                        Poison();
                        Damage(5);
                        break;
                    }
                case "LightShot":
                    {
                        Damage(5);
                        break;
                    }
                case "BeyBlade":
                    {
                        Damage(10);
                        break;
                    }
                case "ShieldBash":
                    {
                        Damage(5);
                        StartCoroutine(Knockback((transform.position - other.transform.position).normalized));
                        break;
                    }
                case "Shout":
                    {
                        target = other.gameObject.GetComponent<Shout>().player;
                        StartCoroutine(Taunt(7));
                        break;
                    }
            }
        }
    }
}
