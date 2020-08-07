using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Checkea el input recibido por el joystick y los dos botones y, en base a eso,
// cambia el estado del jugador.

// TODO: Repensar las funciones de setup y cambiarlas en los scripts que las utilice
// (Mode).

public class InputManager : MonoBehaviour
{
    PlayerStateMachine player;
    JoystickBehaviour movementJoystick;
    CustomButton focusButton;
    CustomButton pushButton;

    public bool manualConnection;

    void Awake(){
        enabled = false;
    }

    public void Setup(PlayerStateMachine p){

        player = p;
        
        movementJoystick = GameObject.Find("MovementJoystick").GetComponent<JoystickBehaviour>();
        pushButton = GameObject.Find("PushButton").GetComponent<CustomButton>();
        focusButton = GameObject.Find("FocusButton").GetComponent<CustomButton>();

        
        pushButton.Setup(OnPushButtonJustPressed);
        focusButton.Setup(OnFocusButtonJustPressed);

        movementJoystick.Setup();

        //Ojo con esta linea cuando se inicia el juego
        //GameControllersSetActive(true);
    }

    // Quizas esto pueda ir en Start().
    public void Setup(){
        movementJoystick = GameObject.Find("MovementJoystick").GetComponent<JoystickBehaviour>();
        pushButton = GameObject.Find("PushButton").GetComponent<CustomButton>();
        focusButton = GameObject.Find("FocusButton").GetComponent<CustomButton>();

        
        pushButton.Setup(OnPushButtonJustPressed);
        focusButton.Setup(OnFocusButtonJustPressed);

        movementJoystick.Setup();

        GameControllersSetActive(false);
    }

    public void SetPlayerReference(PlayerStateMachine p){
        player = p;
    }

    public void Disconnect()
    {
        player = null;
        GameControllersSetActive(false);
    }

    public bool IsSetupRealized() => player != null;

    void OnValidate(){
        if (manualConnection){
            Setup(GameObject.Find("Player").GetComponent<PlayerStateMachine>());
            manualConnection = false;
        }
    }

    public void GameControllersSetActive(bool b){
        movementJoystick.gameObject.SetActive(b);
        pushButton.gameObject.SetActive(b);
        focusButton.gameObject.SetActive(b);
        if (player != null) player.SetMovInputDirection(Vector3.zero);
        enabled = b;
        gameObject.SetActive(b);
    }

    //Input para controlar al objeto del jugador
    void Update(){
        if(player != null && movementJoystick.gameObject.activeSelf){
            Vector2 processedMovInput = Vector2.ClampMagnitude(movementJoystick.GetRawInput(), 1f);
            player.SetMovInputDirection(new Vector3(processedMovInput.x,0,processedMovInput.y));
        }
    }

    void OnPushButtonJustPressed(){
        player.ChangeState((int) PlayerStateMachine.Event.PUSH);
    }

    void OnFocusButtonJustPressed(){
        player.ChangeState((int) PlayerStateMachine.Event.SWITCH_FOCUS_MOVE);
    }

    /*void OnFocusButtonJustReleased(){
        //Debug.Log("focus just released ok");
    }*/
}
