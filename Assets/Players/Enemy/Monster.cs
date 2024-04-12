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
    public GameObject[] players;
    public bool canAct;
    public bool isAttack;
    public float acd;
    public Transform point;
    public int poisonStacks;
    
    public bool isTaunted;
    public Vector3 distance;
    public Vector2 lastInput;
    public GameObject previewBlock;
    public List<Vector2[]> tileLibrary;
    public Rigidbody myRig;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }
    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }
    public virtual void Start()
    {
        isSlowed = false;
        isStunned = false;
        isInvincible = false;
        isTaunted = false;
        myRig = GetComponent<Rigidbody>();
        poisonStacks = 0;
        isAttack = false;
        canAct = true;
        tileLibrary = new List<Vector2[]>();
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
                foreach (GameObject p in players)
                {
                    if ((transform.position - p.transform.position).magnitude < distance.magnitude)
                    {
                        distance = transform.position - p.transform.position;
                        target = p.GetComponent<Player>();
                    }
                }
            }
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
            if(isAttack)
            {
                Attack();
            }
            else
            {
                Move();
            }
        }
        yield return new WaitForSeconds(acd);
        StartCoroutine(TakeAction());
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
        }
        point.position = transform.position;
        MyCore.NetCreateObject(type, Owner, point.position, Quaternion.identity);
    }
    
    public IEnumerator Slow(float t)
    {
        isSlowed = true;
        yield return new WaitForSeconds(t);
        isSlowed = false;
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
    public virtual void Attack()
    {
        
        
    }
    public virtual void Move()
    {
        Vector2[] piece = tileLibrary[Random.Range(0, (tileLibrary.Count - 1))];
        StartCoroutine(Move(piece));

    }
    public void Posion()
    {
        poisonStacks++;
        if (poisonStacks == 1)
            StartCoroutine(Poison());
    }
    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Fireball":
                    {
                        break;
                    }

            }
        }
    }
}
