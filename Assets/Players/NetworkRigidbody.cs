using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class NetworkRigidbody : NetworkComponent
{
    public float threshold = .1f;
    Vector3 syncPosition;
    Vector3 syncRotation;
    Vector3 syncVelocity;
    Vector3 syncAngVelocity;
    public Rigidbody MyRig;
    public Vector3 adaptVelocity;
    public float eThreshold = 1f;
    public bool useAdapt = false;
    public override void HandleMessage(string flag, string value)
    {

        if (flag == "POS")
        {
            if (IsServer)
            {

            }
            if (IsClient)
            {
                syncPosition = NetworkCore.Vector3FromString(value);
                if ((syncPosition - MyRig.position).magnitude > eThreshold)
                {
                    MyRig.position = syncPosition;
                }
                else if ((syncPosition - MyRig.position).magnitude > threshold)
                {
                    adaptVelocity = (syncPosition - MyRig.position) / .1f;
                }
                else
                {
                    adaptVelocity = Vector3.zero;
                }
            }
        }
        if (flag == "VEL")
        {
            if (IsServer)
            {

            }
            if (IsClient)
            {
                syncVelocity = NetworkCore.Vector3FromString(value);
                if (useAdapt)
                {
                    syncAngVelocity += adaptVelocity;
                }
            }
        }
        if (flag == "ROT")
        {
            if (IsServer)
            {

            }
            if (IsClient)
            {
                syncRotation = NetworkCore.Vector3FromString(value);
                if ((syncRotation - MyRig.rotation.eulerAngles).magnitude > eThreshold && useAdapt)
                {

                }
            }
        }
        if (flag == "ANG")
        {
            if (IsServer)
            {

            }
            if (IsClient)
            {
                syncAngVelocity = NetworkCore.Vector3FromString(value);
            }
        }
    }

    public override void NetworkedStart()
    {
    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            
            
                SendUpdate("POS", MyRig.position.ToString());
                syncPosition = MyRig.position;
            
            
                SendUpdate("VEL", MyRig.velocity.ToString());
                syncVelocity = MyRig.velocity;
            
            
                SendUpdate("ROT", MyRig.rotation.eulerAngles.ToString());
                syncRotation = MyRig.rotation.eulerAngles;
            
            
                SendUpdate("ANG", MyRig.angularVelocity.ToString());
                syncAngVelocity = MyRig.angularVelocity;
            
            if (IsDirty)
            {
                SendUpdate("POS", MyRig.position.ToString());
                SendUpdate("VEL", MyRig.velocity.ToString());
                SendUpdate("ROT", MyRig.rotation.ToString());
                SendUpdate("ANG", MyRig.angularVelocity.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
        yield return new WaitForSeconds(MyId.UpdateFrequency);

    }

    // Start is called before the first frame update
    void Start()
    {
        MyRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MyCore && IsClient)
        {
            MyRig.velocity = syncVelocity;
            MyRig.angularVelocity = syncAngVelocity;
        }
            
        
    }
}
