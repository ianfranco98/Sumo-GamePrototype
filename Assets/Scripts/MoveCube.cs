using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCube : MonoBehaviour
{

    void Start()
    {
        DOTween.Init();
    }

    /*void Update()
    {
        Vector2 rawInput = joystick.getRawInput();
        Vector3 dir = new Vector3(rawInput.x,0,rawInput.y);
        switch(currentState){
            case State.FREE_MOVE:
                transform.Translate(dir * Time.deltaTime);
            break;
            case State.FREE_PUSH:
                //dir = dir == Vector3.zero ? Vector3.right: dir;
                

            break;
        }
        

        
    }*/

    /*public void startPush(){

        if (currentState == State.FREE_MOVE){
            Vector2 rawInput = joystick.getRawInput();
            Vector3 dir = new Vector3(rawInput.x,0,rawInput.y);
            dir = dir == Vector3.zero ? Vector3.right: dir;
            transform.DOMove(transform.position + dir * 2, 1).OnComplete(endPush);
        }

    }*/
}
