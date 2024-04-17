using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class HeroProjectile : Projectile
{
    public Vector2 lastInput;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        
    }

    public override void NetworkedStart()
    {
        transform.right = myRig.velocity;
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        transform.right = myRig.velocity;
    }

    // Update is called once per frame
    public override void Update()
    {
        transform.right = myRig.velocity;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.gameObject.tag == "Skeleton" || collision.gameObject.tag == "Eyeball" || collision.gameObject.tag == "Goblin" || collision.gameObject.tag == "Mushroom")
            {
                MyCore.NetDestroyObject(NetId);
            }
        }

    }
}
