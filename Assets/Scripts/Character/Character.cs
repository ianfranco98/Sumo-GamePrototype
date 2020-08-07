using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected int currentState = 0;
    protected int previousState = 0;


    protected float acceleration;
    protected float deceleration;
    protected float currentSpeed;
    protected float maxSpeed;

    protected float size;

    protected Vector3 lastMovInputDirection;
    protected Vector3 movInputDirection;

    protected Transform opponentTransform;

    protected Transform modelTransform;

    protected CharacterController characterController;

    [SerializeField]
    protected Vector3 originalPosition;

    public void SetDefaultParams(Transform opponentReference = null)
    {
        maxSpeed = 0.1f;
        currentSpeed = 0;
        acceleration = 0.15f;
        deceleration = 0.1f;
        size = 0.5f;

        characterController = GetComponent<CharacterController>();

        SetOponnent(opponentReference);

        movInputDirection = new Vector3();
        lastMovInputDirection = new Vector3();
        modelTransform = GameObject.Find("Model").GetComponent<Transform>();

        //int currentChildCount = 0;
        Debug.Log(transform.childCount);

        /*while (modelTransform != null || (currentChildCount - 1) == transform.childCount){
            Debug.Log(currentChildCount);
            GameObject child = transform.GetChild(currentChildCount).gameObject;
            if(child.name == "Model") modelTransform = child.transform;
            currentChildCount++;
        }*/

        // Fuerzo a que el primer objeto en la jerarquia tenga que ser el model,
        // a si que ojo con eso. Luego hay q cambiar
        modelTransform = transform.GetChild(0);

        if (modelTransform == null)
        {
            Debug.LogWarning(gameObject.name + ": Didn't find the model for this character. Default set is a new GameObject.Transform");
            modelTransform = new GameObject().transform;
        }

        enabled = true;
    }

    public void SetOponnent(Transform opponentReference = null){
        if (opponentReference == null)
        {
            opponentTransform = new GameObject().transform;
            Debug.LogWarning(gameObject.name + ": player without opponent. Default set is a new GameObject.Transform");
        }
        else opponentTransform = opponentReference;
    }


    protected Dictionary<int[], int> _transitions = new Dictionary<int[], int>(new MyEqualityComparer());

    protected virtual void EnterState(){
        
    }

    public void ChangeState(int newEvent)
    {
        int[] transition = new int[] { currentState, newEvent };

        if (!_transitions.ContainsKey(transition))
        {
            PlayerStateMachine.State _zero = (PlayerStateMachine.State) transition[0];
            PlayerStateMachine.State _one = (PlayerStateMachine.State) transition[1];
            string zero = _zero.ToString();
            string one = _one.ToString();
            Debug.LogWarning(gameObject.name + ": Invalid transition " + zero + " " + one);
            return;
        }

        previousState = currentState;
        currentState = _transitions[transition];
        EnterState();
    }

    public void ForceChangeState(int newState)
    {
        currentState = newState;
        EnterState();
    }

    /*public void Move(Vector2 input){

        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration);

        Vector3 dir = new Vector3(input.x, transform.position.y, input.y);
        transform.Translate(dir);
    }*/
}

public class MyEqualityComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[] x, int[] y)
    {
        if (x.Length != y.Length)
        {
            return false;
        }
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(int[] obj)
    {
        int result = 17;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked
            {
                result = result * 23 + obj[i];
            }
        }
        return result;
    }
}
