using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class Helmet : Item
{
    public Player player;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
    }

    public override void NetworkedStart()
    {
        base.NetworkedStart();
    }
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(1.5f);
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
    public void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            switch (other.tag)
            {
                case "Knight":
                    {
                        MyCore.NetDestroyObject(MyId.NetId);
                        break;
                    }
            }
        }
    }
}
