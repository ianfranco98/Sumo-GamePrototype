using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using MLAPI;
using MLAPI.Transports;
using MLAPI.Messaging;
using MLAPI.Connection;
using MLAPI.Serialization.Pooled;

// Raiz de la escena en modo Local Multiplayer. Inicializa todo y luego gestiona lo externo a la lógica del juego
// como interfaz, cámara, activacion y desactivacion del input. Tambien ejecuta la inicializaciones de conexión
// con los script UDP.

public class LocalMultiplayerMode : GameMode
{
    public GameObject playerPrefab;
    NetworkModeHandler networkModeHandler;
    enum LocalMultiplayerGameState
    {
        CONNECTION_INTERRUPTED,
        SELECT_NETWORK_MODE,
        STARTING,
        ON_GAME,
        ROUND_ENDED,
        FIGHT_ENDED
    }

    private ulong opponentClientId;

    public void Start()
    {
        GameObject leanTouch = GameObject.FindWithTag("LeanTouch");
        if (leanTouch == null) leanTouch = Instantiate(leanTouchPrefab);

        EndGameMenu = GameObject.Find("GameEndedDisplay");
        EndGameMenu.SetActive(false);

        cam = GameObject.Find("CustomCamera").GetComponent<CustomCamera>();

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        networkModeHandler = GameObject.Find("NetworkingManager").GetComponent<NetworkModeHandler>();

        NetworkingManager.Singleton.OnClientDisconnectCallback += BackToInitialState;
    }

    public void BackToInitialState(ulong client)
    {
        if (game != null) Destroy(game);
        GameObject player = GameObject.Find("Player");
        if (player != null) Destroy(player);
        if (inputManager != null && inputManager.IsSetupRealized()) inputManager.Disconnect();
        cam.Disconnect();
        networkModeHandler.EndCurrentMode();
        GameObject netManager = GameObject.Find("NetworkingManager");
        
        Destroy(netManager);

        
        BackToMenu();
    }
    
    public void BackToInitialState()
    {
        if (game != null) Destroy(game);
        GameObject player = GameObject.Find("Player");
        if (player != null) Destroy(player);
        if (inputManager != null && inputManager.IsSetupRealized()) inputManager.Disconnect();
        cam.Disconnect();
        networkModeHandler.EndCurrentMode();
        GameObject netManager = GameObject.Find("NetworkingManager");
        
        Destroy(netManager);

        
        BackToMenu();
    }

    //------------------------------------
    //------------MODO HOST---------------
    //------------------------------------


    // Ejecutado por los botones de la GUI
    public void StartHost()
    {
        Debug.Log("-Inicializando Host-");
        NetworkingManager.Singleton.OnClientConnectedCallback += CheckPlayers;
    }



    IEnumerator DelayedInitializeHostGame()
    {
        foreach (NetworkedClient nc in NetworkingManager.Singleton.ConnectedClientsList)
        {
            if (nc.ClientId != NetworkingManager.Singleton.ServerClientId) opponentClientId = nc.ClientId;
        }

        Debug.Log("-Inicializando juego Local-");

        SpawnPlayer(NetworkingManager.Singleton.ServerClientId);

        SpawnPlayer(opponentClientId);

        yield return new WaitForSeconds(1.5f);

        GameObject player = GameObject.Find("Player");
        GameObject opponent = GameObject.Find("Dummy");

        PlayerStateMachine playerSM = player.GetComponent<PlayerStateMachine>();

        playerSM.Setup();
        inputManager.Setup(playerSM);
        inputManager.GameControllersSetActive(true);

        cam.Setup(player.GetComponent<Transform>(), opponent.GetComponent<Transform>());

        playerSM.SetOponnent(opponent.GetComponent<Transform>());

        game = gameObject.AddComponent(typeof(OnGameFightLogic)) as OnGameFightLogic;
        game.Setup(player.GetComponent<Transform>(), opponent.GetComponent<Transform>(), NetworkedGameStateUpdated, true);
    }

    void CheckPlayers(ulong client)
    {
        if (NetworkingManager.Singleton.ConnectedClientsList.Count == 2)
        {
            StartCoroutine(DelayedInitializeHostGame());

        }
    }

    // Esta funcion controla el flujo de los GameObject externos a la logica de juego.
    void NetworkedGameStateUpdated()
    {
        Debug.Log(game.GetCurrentState());

        switch (game.GetCurrentState())
        {
            case OnGameFightLogic.State.STARTING:
                EndGameMenu.SetActive(false);
                inputManager.GameControllersSetActive(false);
                // Zoom in al estadio o alguna animación. Se despliega la GUI.

                break;
            case OnGameFightLogic.State.ON_GAME:
                inputManager.GameControllersSetActive(true);
                cam.FollowPlayers();
                break;
            case OnGameFightLogic.State.ROUND_ENDED:

                StartCoroutine(RoundEndedAnim());

                //Mostrar en GUI ganó p1 o p2
                //Activar anim para el modelo del p1 o p2

                // Actualizar score en la GUI
                // Quien ganó y esperar input para continuar con la siguiente ronda.
                break;
            case OnGameFightLogic.State.FIGHT_ENDED:
                EndGameMenu.SetActive(true);
                FocusOnWinner();

                // Mostrar pantalla de pelea finalizada: quien gano y eso.
                // Mostrar opciones de revancha y volver al menú inicial.
                break;
        }
    }

    //---------------------------------------------------------------------------------


    //------------------------------------
    //------------MODO CLIENTE------------
    //------------------------------------

    // Ejecutado por los botones de la GUI
    public void StartClient()
    {
        StartCoroutine(DelayedStartClient());
    }

    IEnumerator DelayedStartClient()
    {
        Debug.Log("-Inicializando Cliente-");

        yield return new WaitForSeconds(1.5f);

        try
        {

            GameObject player = GameObject.Find("Player");
            GameObject opponent = GameObject.Find("Dummy");

            player.GetComponent<PlayerStateMachine>().Setup(opponent.transform);

            inputManager.Setup(player.GetComponent<PlayerStateMachine>());
            inputManager.GameControllersSetActive(true);

            cam.Setup(opponent.GetComponent<Transform>(), player.GetComponent<Transform>());
            cam.FollowPlayers();

        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }


    //---------------------------------------------------------------------------------

    private void SpawnPlayer(ulong clientId)
    {
        NetworkedObject netObj = Instantiate(playerPrefab).GetComponent<NetworkedObject>();

        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                netObj.SpawnAsPlayerObject(clientId, stream, true);
            }
        }
    }
}
