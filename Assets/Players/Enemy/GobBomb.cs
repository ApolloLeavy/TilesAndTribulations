using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class GobBomb : EnemyProjectile
{
    public Vector2 lastInput;
    public List<Vector2[]> tileLibrary;
    
    public int speed;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
        StartCoroutine(Timer());
    }

    public override IEnumerator SlowUpdate()
    {

        yield return new WaitForSeconds(.1f);

    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(4);
        BlowUp();
    }
    public void BlowUp()
    {
        MyCore.NetCreateObject(27, Owner, transform.position, Quaternion.identity);
        MyCore.NetDestroyObject(NetId);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        tileLibrary = new List<Vector2[]>();
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(1, 0) });
        tileLibrary.Add(new Vector2[] { new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(-1, 0) });
        speed = 1;
        StartCoroutine(Move(tileLibrary[Random.Range(0,2)]));
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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
    public new void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.tag == "Rogue" || collision.gameObject.tag == "Knight" || collision.gameObject.tag == "Ranger" || collision.gameObject.tag == "Wizard")
            {
                BlowUp();
            }
        }
    }
}

