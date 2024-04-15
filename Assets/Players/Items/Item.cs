using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Item : NetworkComponent
{
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            switch (collision.gameObject.tag)
            {
                case "Knight":
                case "Rogue":
                case "Wizard":
                case "Ranger":
                    {
                        MyCore.NetDestroyObject(MyId.NetId);
                        break;
                    }
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    public override void HandleMessage(string flag, string value)
    {

    }

    public override void NetworkedStart()
    {

    }
}
