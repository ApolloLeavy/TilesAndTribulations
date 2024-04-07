using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.InputSystem;
public class Player : NetworkComponent
{
    public int hp;
    public int tileCount;
    public int maxTiles;
    public int tileGainSec;
    public bool isGenerating;
    public int qcd;
    public int wcd;
    public int ecd;
    public int rcd;
    public Rigidbody myRig;
    public float speed;
    public Vector2 lastInput;
    public bool canPlace;
    public bool canAttack;
    public bool canQ;
    public bool canW;
    public bool canE;
    public bool canR;
    public List<Vector2[]> tiles;
    public List<Vector2[]> tileLibrary; 
    public int activeTile;
    public bool isInvincible;
    public int kills;
    public int deaths;
    public int assists;
    public bool isDead;
    public float deathTimer;
    public GameObject npm;
    public override void HandleMessage(string flag, string value)
    {
        if (IsServer)
        {
            if (flag == "MV")
            {
                string[] tmp = value.Split(',');
                lastInput = new Vector2(float.Parse(tmp[0]), float.Parse(tmp[1]));
                IsDirty = true;
                if ((lastInput.x > 0 || lastInput.x < 0) && (lastInput.y >0||lastInput.y<0))
                {
                    lastInput.x = 0;
                }
                
            }

        }
        if (IsClient)
        {


        }
    }
 
    public override void NetworkedStart()
    {
        
    }

    public override IEnumerator SlowUpdate()
    {
        if (IsConnected)
        {
            while(IsServer)
            {
                if (IsDirty)
                {
                    IsDirty = false;
                }
                yield return new WaitForSeconds(MyId.UpdateFrequency);
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);

        }
    }
    public void OnPlace(InputAction.CallbackContext ev)
    {
        if (IsServer && ev.performed && canPlace)
        {
            Debug.Log("Place");
            canPlace = false;
            StartCoroutine(Move(tiles[activeTile]));
        }
    }
    public void OnMove(InputAction.CallbackContext ev)
    {
        if (IsLocalPlayer)
        {
            if (ev.started || ev.performed)
            {
                Debug.Log("Input");
                Vector2 tempCmd = ev.ReadValue<Vector2>();
                SendCommand("MV", tempCmd.x + "," + tempCmd.y);

            }
            if (ev.canceled)
            {
                SendCommand("MV", lastInput.x + "," + lastInput.y);
            }
        }
    }
    public IEnumerator Move(Vector2[] dir)
    {
        if (IsServer)
        {
            Debug.Log("Move");
            for (int i = 0; i < dir.Length;i++)
            {
                myRig.velocity = new Vector3(lastInput.x, 0, lastInput.y);
                yield return new WaitForSeconds(dir[i].magnitude / speed);
                
            }
            myRig.velocity = new Vector3(0, 0, 0);
            canPlace = true;
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        myRig = GetComponent<Rigidbody>();
        maxTiles = 5;
        tileCount = 0;
        tileGainSec = 2;
        canAttack = true;
        canPlace = true;
        canQ = true;
        canW = true;
        canE = true;
        canR = true;
        tiles = new List<Vector2[]>();
        tiles.Add(new Vector2[] { new Vector2(0, 4), new Vector2(2, 0) });
        activeTile = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            
        }
        if (IsClient)
        {
            //animation logic pref with synchronzied variables
        }
        if (IsLocalPlayer)
        {

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position + new Vector3(0,0, -9), myRig.position, speed / 2 * Time.deltaTime);

        }
    }
}
