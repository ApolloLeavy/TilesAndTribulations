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
    Rigidbody myRig;
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
                if ((syncPosition - myRig.position).magnitude > eThreshold)
                {
                    myRig.position = syncPosition;
                }
                else if ((syncPosition - myRig.position).magnitude > threshold)
                {
                    adaptVelocity = (syncPosition - myRig.position) / .1f;
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
                if ((syncRotation - myRig.rotation.eulerAngles).magnitude > eThreshold && useAdapt)
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
            
            
                SendUpdate("POS", myRig.position.ToString());
                syncPosition = myRig.position;
            
            
                SendUpdate("VEL", myRig.velocity.ToString());
                syncVelocity = myRig.velocity;
            
            
                SendUpdate("ROT", myRig.rotation.eulerAngles.ToString());
                syncRotation = myRig.rotation.eulerAngles;
            
            
                SendUpdate("ANG", myRig.angularVelocity.ToString());
                syncAngVelocity = myRig.angularVelocity;
            
            if (IsDirty)
            {
                SendUpdate("POS", myRig.position.ToString());
                SendUpdate("VEL", myRig.velocity.ToString());
                SendUpdate("ROT", myRig.rotation.ToString());
                SendUpdate("ANG", myRig.angularVelocity.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
        yield return new WaitForSeconds(MyId.UpdateFrequency);

    }

    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {
            myRig.velocity = syncVelocity;
            myRig.angularVelocity = syncAngVelocity;
        }
    }
}
