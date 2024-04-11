using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Projectile : NetworkComponent
{
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

    // Start is called before the first frame update
    public virtual void Start()
    {
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
}
