using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerData : NetworkBehaviour
{
    [SyncVar]
    public int playerCount = -1;
    
    public void SetPlayerCount(int n)
    {
        Debug.Log("playerCount SET : " + n);
        playerCount = n;
    }
}
