using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomButton : InputBehaviour
{
    public delegate void Callback();

    Callback OnJustPressedDo;
    Callback OnPressedDo;
    Callback OnJustReleasedDo;

    Callback ReferenceOnJustPressedDo;
    Callback ReferenceOnPressedDo;
    Callback ReferenceOnJustReleasedDo;
    //Callback OnReleasedDo;

    void Start()
    {
        if (OnJustPressedDo == null) OnJustPressedDo = new Callback(NullCallbackWarning);
        if (OnPressedDo == null) OnPressedDo = new Callback(EmptyCallback);//leanSelectable.OnSelectSet.RemoveListener(Pressed);
        if (OnJustReleasedDo == null) OnJustReleasedDo = new Callback(EmptyCallback);//leanSelectable.OnSelectUp.RemoveListener(JustReleased);
    }

    //-------------------------

    public void Setup(Callback justPressedMethod){
        OnJustPressedDo = justPressedMethod;
        ReferenceOnJustPressedDo = justPressedMethod;
    }

    public void Setup(Callback justPressedMethod, Callback justReleasedMethod){
        OnJustPressedDo = justPressedMethod;
        OnJustReleasedDo = justReleasedMethod;

        ReferenceOnJustPressedDo = justPressedMethod;
        ReferenceOnJustReleasedDo = justReleasedMethod;
    }

    public void Setup(Callback justPressedMethod, Callback pressedMethod, Callback justReleasedMethod){
        OnJustPressedDo = justPressedMethod;
        OnPressedDo = pressedMethod;
        OnJustReleasedDo = justReleasedMethod;

        ReferenceOnJustPressedDo = justPressedMethod;
        ReferenceOnPressedDo = pressedMethod;
        ReferenceOnJustReleasedDo = justReleasedMethod;
    }

    //-------------------------

    public void Connect(){
        if (ReferenceOnJustPressedDo != null) OnJustPressedDo = ReferenceOnJustPressedDo;
        if (ReferenceOnPressedDo != null) OnPressedDo = ReferenceOnPressedDo;
        if (ReferenceOnJustReleasedDo != null) OnJustReleasedDo = ReferenceOnJustReleasedDo;

        gameObject.SetActive(true);
    }
    public void Disconnect(){
        if (OnJustPressedDo != null) OnJustPressedDo = new Callback(NullCallbackWarning);
        if (OnPressedDo != null) OnPressedDo = new Callback(EmptyCallback);
        if (OnJustReleasedDo != null) OnJustReleasedDo = new Callback(EmptyCallback);

        gameObject.SetActive(false);
    }

    protected override void JustPressed(Lean.Touch.LeanFinger finger){
        OnJustPressedDo();
    }
    protected override void Pressed(Lean.Touch.LeanFinger finger){
        OnPressedDo();
	}

    protected override void JustReleased(Lean.Touch.LeanFinger finger){
        OnJustReleasedDo();
	}

    /*protected override void Released(){
        
    }*/

    private void EmptyCallback(){
        //Quise remover los listener que no usaba pero eso hace que se borre en todas las instancias.
        //Esto es lo que se me ocurrio por ahora.
    }

    private void NullCallbackWarning(){
        Debug.LogWarning(gameObject.name + " didn't execute setup.");
    }

}
