using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

class UDPClient : MonoBehaviour
{
    int MY_PORT = 9999;
    int HOST_PORT = 9876;

    NetworkModeHandler networkModeHandler;
    UdpClient MyUdpClient;
    Thread udpClientThread;
    String key = "send";

    [SerializeField]
    SearchingDisplayBehaviour button;

    void Start()
    {
        networkModeHandler = gameObject.GetComponent<NetworkModeHandler>();
    }

    void OnDisable()
    {
        EndThread();
    }

    public void StartClient()
    {
        if (MyUdpClient == null)
        {
            MyUdpClient = new UdpClient(MY_PORT);
            MyUdpClient.Client.ReceiveTimeout = 3000;
        }

        udpClientThread = new Thread(new ThreadStart(CheckIfHostIsAvailable));
        udpClientThread.IsBackground = true;
        udpClientThread.Start();
    }

    public void CheckIfHostIsAvailable()
    {
        String hostIP = "";

        int trials = 0;
        bool signalReceived = false;

        while (!signalReceived && trials < 10)
        {
            trials++;
            Debug.Log("Intento numero: " + trials);
            //Listening 
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, HOST_PORT);

                var data = Encoding.UTF8.GetBytes(key);
                MyUdpClient.Send(data, data.Length, "255.255.255.255", HOST_PORT);

                byte[] receiveBytes = MyUdpClient.Receive(ref RemoteIpEndPoint);

                if (receiveBytes != null)
                {
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    Debug.Log("Host Message Received" + returnData.ToString());
                    Debug.Log("Address IP Sender" + RemoteIpEndPoint.Address.ToString());
                    Debug.Log("Port Number Sender" + RemoteIpEndPoint.Port.ToString());

                    hostIP = RemoteIpEndPoint.Address.ToString();

                    signalReceived = true;

                    networkModeHandler.SaveClientIPAddress(hostIP);
                    button.HostFinded();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
            }
        }

        if (!signalReceived)
        {
            Debug.Log("Host no encontrado");
            button.FindingHostFailed();
        }
        EndThread();
    }

    void EndThread()
    {
        if (MyUdpClient != null) MyUdpClient.Close();
        if (udpClientThread != null) udpClientThread.Abort();
    }
}
