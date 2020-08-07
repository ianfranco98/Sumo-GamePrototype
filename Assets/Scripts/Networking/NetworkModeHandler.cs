using MLAPI;
using MLAPI.Transports;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Sockets;

// TODO: Limpiar y renombrar este script.

public class NetworkModeHandler : MonoBehaviour
{
    enum NetworkState
    {
        None,
        Host,
        Client
    }

    RufflesTransport.RufflesTransport ruffles;
    NetworkState currentNetworkState = NetworkState.None;
    NetworkState lastNetworkState = NetworkState.None;

    NetworkState CurrentNetworkState
    {
        get
        {
            return CurrentNetworkState;
        }
        set
        {
            Debug.Log("net state updated: " + value + " " + currentNetworkState);
            if (value != currentNetworkState)
            {
                switch (value)
                {
                    case NetworkState.None:
                        if (NetworkingManager.Singleton.IsHost)
                        {
                            NetworkingManager.Singleton.StopHost();
                            Debug.Log("Host Off");
                            lastNetworkState = NetworkState.Host;
                        }
                        else if (NetworkingManager.Singleton.IsClient)
                        {
                            NetworkingManager.Singleton.StopClient();
                            Debug.Log("Client Off");
                            lastNetworkState = NetworkState.Client;
                        }
                        break;
                    case NetworkState.Host:
                        NetworkingManager.Singleton.StartHost();
                        Debug.Log("Host On");
                        break;
                    case NetworkState.Client:
                        //GameObject.Find("LocalMultiplayerMode").GetComponent<LocalMultiplayerMode>().StartClient();
                        NetworkingManager.Singleton.StartClient();
                        Debug.Log("Client On");
                        break;
                }
                currentNetworkState = value;
            }
        }
    }

    void Start()
    {
        ruffles = gameObject.GetComponent<RufflesTransport.RufflesTransport>();
    }

    public void SetHostMode() => CurrentNetworkState = NetworkState.Host;
    public void SetClientMode() => CurrentNetworkState = NetworkState.Client;
    public void EndCurrentMode() => CurrentNetworkState = NetworkState.None;
    public bool LastNetworkStateIsHost() => lastNetworkState == NetworkState.Host; 

    public void SaveClientIPAddress(string newText)
    {
        Debug.Log("Host Info Catched");
        ruffles.ConnectAddress = newText;
    }

    public string LocalIPAddress()
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

        Debug.Log(localIP);
        return localIP;
    }

    // El script "NetworkedManager" hace que este gameObject se vuelva Singleton, destruyendo la copia de este objeto
    // que se genera al volver a cargar esta escena (LocalMultiplayer) y haciendo que los Botones pierdan
    // vinculo con las funciones de los scripts de este objeto. Por lo que hay que destruirlo al salir de
    // escena.
    //
    // La funcion está linkeada con el boton "Back To Menu".
    public void AutoDestruction()
    {
        Destroy(gameObject);
    }


}
