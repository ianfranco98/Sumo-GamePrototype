using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Raiz de la escena en modo Single Player. Inicializa todo y luego gestiona lo externo a la lógica del juego
// como interfaz, cámara, activacion y desactivacion del input, etc.

public class SinglePlayerMode : GameMode
{
    enum SinglePlayerGameState
    {
        LOADING,
        ON_GAME,
        PAUSE,
        EXITING
    }

    void Start()
    {
        EndGameMenu = GameObject.Find("GameEndedDisplay");
        EndGameMenu.SetActive(false);

        GameObject player = GameObject.Find("Player");
        GameObject opponent = GameObject.Find("Dummy");

        player.GetComponent<PlayerStateMachine>().Setup(opponent.transform);
        opponent.GetComponent<PlayerStateMachine>().Setup(player.transform);

        GameObject leanTouch = GameObject.FindWithTag("LeanTouch");
        if (leanTouch == null) leanTouch = Instantiate(leanTouchPrefab);

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.Setup(player.GetComponent<PlayerStateMachine>());

        cam = GameObject.Find("CustomCamera").GetComponent<CustomCamera>();
        cam.Setup(player.GetComponent<Transform>(), opponent.GetComponent<Transform>());

        game = gameObject.AddComponent(typeof(OnGameFightLogic)) as OnGameFightLogic;
        game.Setup(player.GetComponent<Transform>(), opponent.GetComponent<Transform>(), GameStateUpdated);

    }

    void GameStateUpdated()
    {

        Debug.Log(game.GetCurrentState());

        switch (game.GetCurrentState())
        {
            case OnGameFightLogic.State.STARTING:
                EndGameMenu.SetActive(false);
                inputManager.GameControllersSetActive(false);
                // Zoom in al estadio o alguna animación. Se despliega la GUI.
                cam.SwingAroundCenter();

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
            case OnGameFightLogic.State.GRAB_BATTLE:
                cam.GrabBattleSwing();
            break;
        }
    }
}
