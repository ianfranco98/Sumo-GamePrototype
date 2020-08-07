using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class JoystickBehaviour : InputBehaviour
{
    Vector2 inputDirection;
    RectTransform baseTransform;
    RectTransform touchTransform;
    Canvas canvas;

    float baseRadius;

    void Start(){
        //gameObject.SetActive(false);
        
    }


    public override void Setup(){
        canvas = GetComponentInParent<Canvas>();
        baseTransform = GameObject.Find("Base").GetComponent<RectTransform>();
        touchTransform = GameObject.Find("Touch").GetComponent<RectTransform>();

        originalPosition = baseTransform.position;
        baseRadius = baseTransform.sizeDelta.x/2;
        //Debug.Log("radio" + baseRadius);
        //gameObject.SetActive(true);
    }


    protected override void JustPressed(Lean.Touch.LeanFinger finger){
        touchTransform.position = RectTransformUtility.PixelAdjustPoint(
            finger.ScreenPosition,
            touchTransform,
            canvas
        );

        baseTransform.position = touchTransform.position;
    }
    protected override void Pressed(Lean.Touch.LeanFinger finger){

        Vector3 dir = (touchTransform.position - baseTransform.position).normalized;
        touchTransform.position = RectTransformUtility.PixelAdjustPoint(
            finger.ScreenPosition,
            touchTransform,
            canvas
        );

        float baseAndTouchDist = Vector3.Distance(touchTransform.position, baseTransform.position);

        if (baseAndTouchDist > baseRadius){
            baseTransform.position += dir * 9;
        }

        inputDirection = dir;
    }
 
    protected override void JustReleased(Lean.Touch.LeanFinger finger){
        inputDirection = Vector2.zero;
        baseTransform.position = originalPosition;
        touchTransform.position = originalPosition;
    }

    public Vector2 GetRawInput(){
        Vector2 input = new Vector2(inputDirection.x,inputDirection.y);
        return input;
    }
}
