using System.Net;
using UnityEngine;
using System;

public class DNSTest : MonoBehaviour
{
    void Start()
    {
        try
        {
            // Enter MongoDB Connection string here
            IPHostEntry hostInfo = Dns.GetHostEntry("");
            foreach (IPAddress ip in hostInfo.AddressList)
            {
                Debug.Log("###############  IP Address: " + ip.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogError("################# DNS resolution failed: " + e.Message);
        }
    }
}
