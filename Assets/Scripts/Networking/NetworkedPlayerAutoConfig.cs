using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.Connection;

// Convierte al prefab NetworkedPlayer en Player o en Dummy dependiendo de si el objeto
// pertenece al jugador o no.
 

public class NetworkedPlayerAutoConfig : NetworkedBehaviour
{
    public int whichPlayer = 0;
    ulong enemyID;

    Vector3 originalPositionP1 = new Vector3(-3, 0, 0);
    Vector3 originalPositionP2 = new Vector3(3, 0, 0);

    [SerializeField]
    Material playerOneColor, playerTwoColor;

    [SerializeField]
    Renderer modelRenderer; 

    void Start()
    {
        Setup();
    }

    //TODO: Ver si se puede limpiar todo esto
    void Setup()
    {
        PlayerStateMachine selfStateMachine = GetComponent<PlayerStateMachine>();
        if (IsLocalPlayer)
        {
            gameObject.name = "Player";
            if (NetworkingManager.Singleton.IsHost)
            {
                SetAsPlayer1();
                selfStateMachine.SetOriginalPosition(originalPositionP1);
            }
            else
            {
                SetAsPlayer2();
                // TODO: Acomodar mejor esto.
                //LocalMultiplayerMode lmm = GameObject.Find("LocalMultiplayerMode").GetComponent<LocalMultiplayerMode>();
                //lmm.SetupClient(gameObject);
                gameObject.GetComponent<PlayerNetworkedVar>().Setup();
                selfStateMachine.SetOriginalPosition(originalPositionP2);
            }
        }
        else
        {

            gameObject.name = "Dummy";

            if (NetworkingManager.Singleton.IsHost)
            {
                SetAsPlayer2();
            }
            else
            {
                SetAsPlayer1();
            }
            Destroy(selfStateMachine);
        }
    }

    void SetAsPlayer1()
    {
        whichPlayer = 1;
        modelRenderer.material = playerOneColor;
    }

    void SetAsPlayer2()
    {
        whichPlayer = 2;
        modelRenderer.material = playerTwoColor;
    }

}
