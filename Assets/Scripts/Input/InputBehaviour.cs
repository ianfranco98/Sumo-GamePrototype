using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Lean.Touch;

public class InputBehaviour : MonoBehaviour
{

    protected LeanSelectable leanSelectable;
    protected Vector3 originalPosition;

    void Awake(){

        LeanSelectable checkComp = GetComponent<LeanSelectable>();
        leanSelectable = checkComp == null ? gameObject.AddComponent(typeof(LeanSelectable)) as LeanSelectable : checkComp;

        leanSelectable.OnSelect.AddListener(JustPressed);
        leanSelectable.OnSelectSet.AddListener(Pressed);
        leanSelectable.OnSelectUp.AddListener(JustReleased);
        leanSelectable.OnDeselect.AddListener(Released);
    }

	protected virtual void JustPressed(LeanFinger finger){
	}

    protected virtual void Pressed(LeanFinger finger){
	}

    protected virtual void JustReleased(LeanFinger finger){
	}

    protected virtual void Released(){
	}


    public virtual void Setup(){

    }

    protected virtual void Process(){

    }

}
