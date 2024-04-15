using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class LightningStrike : Projectile
{
    public Player player;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        StartCoroutine(Timer());
        if(IsServer)
        {
            MyCore.NetCreateObject(36, Owner, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
            MyCore.NetCreateObject(36, Owner, transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
            MyCore.NetCreateObject(36, Owner, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
            MyCore.NetCreateObject(36, Owner, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }
    public  IEnumerator Timer()
    {
        
        yield return new WaitForSeconds(1);
        MyCore.NetDestroyObject(NetId);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
