using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Icespike : Projectile
{
    public Player player;
    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {
        StartCoroutine(Timer());
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(5);
        MyCore.NetDestroyObject(NetId);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
