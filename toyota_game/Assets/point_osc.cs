using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class point_osc : MonoBehaviour
{
	
    #region Field

    public string serverId = "MaxMSP";
    public string serverIp = "127.0.0.1";
    public int    serverPort = 12000;

    public string  debugMessage = "/point";

    public bool enableShowDebugLog = true;

	public GameObject objA;
    public GameObject objB;

    #endregion Field

	int count = 0;

    void Start ()
    {
        OSCHandler.Instance.Init(this.serverId, this.serverIp, this.serverPort);
        ShowDebugLog();

    }

    void Update()
    {
        Vector3 Apos = objA.transform.position;
        Vector3 Bpos = objB.transform.position;
        float dis = Vector3.Distance(Apos,Bpos);
        //Debug.Log("Distance : " + dis);
       
	   
	   if (dis < 40 && count == 0){
        SendDebugMessageToClient();
		count = 1;
	   }


	   if (dis > 45 && count == 1){
        // SendDebugMessageToClient();
		count = 0;
	   }
    }

    private void SendDebugMessageToClient()
    {
        OSCHandler.Instance.SendMessageToClient
		(this.serverId, this.debugMessage,5);
		
        // ShowDebugLog();
    }

    private void ShowDebugLog()
    {
        if (this.enableShowDebugLog == false)
        {
            return;
        }

        Debug.Log("OSC (" + this.serverId + ", "
                          + this.serverIp + ", "
                          + this.serverPort + ")");
    }
}