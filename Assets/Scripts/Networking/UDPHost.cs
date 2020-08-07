using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using UnityEngine;

class UDPHost : MonoBehaviour
{

    int MY_PORT = 9876;
    int CLIENT_PORT = 9999;
    Thread udpHostThread;
    UdpClient receivingUdpClient;

    void OnDisable()
    {
        EndThread();
    }

    //Triggereado desde los eventos de la UI
    public void StartHost()
    {
        if (receivingUdpClient == null)
        {
            receivingUdpClient = new UdpClient(MY_PORT);
            receivingUdpClient.Client.ReceiveTimeout = 20000;// 20 segs
        }

        udpHostThread = new Thread(new ThreadStart(WaitForClientSignal));
        udpHostThread.IsBackground = true;
        udpHostThread.Start();       
    }

    void WaitForClientSignal()
    {
        try
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, CLIENT_PORT);
            //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Broadcast, 5000);

            // Blocks until a message returns on this socket from a remote host.
            byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

            if (receiveBytes != null)
            {
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                Debug.Log("Client Message Received " + returnData.ToString());
                Debug.Log("Address IP Sender " + RemoteIpEndPoint.Address.ToString());
                Debug.Log("Port Number Sender " + RemoteIpEndPoint.Port.ToString());

                //if (returnData.ToString() == "send")
                //{
                    Debug.Log("recibido papu");
                    var data = Encoding.UTF8.GetBytes(MyLocalIPAddress());
                    receivingUdpClient.Send(data, data.Length, "255.255.255.255", CLIENT_PORT);
                    Debug.Log("ip enviada papu");
                //}
            }


        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }

        EndThread();
    }

    public static string MyLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        Console.WriteLine(localIP);
        return localIP;
    }

    void EndThread()
    {
        //if (receivingUdpClient != null) receivingUdpClient.Close();
        if (udpHostThread != null) udpHostThread.Abort();
    }

}
