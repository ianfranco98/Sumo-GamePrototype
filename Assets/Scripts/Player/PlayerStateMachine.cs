using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Contiene todo el comportamiento posible del Player según que estado tenga
// (variable currentState de Character).
// A excepcion de los finales de animación, no es capaz de cambiar de estado
// por si sola.

public class PlayerStateMachine : Character
{
    int pushCount;
    float pushForce;
    float pushDecreasementCount;
    public float pushDistanceScalar = 7;
    public float pushTime = 1;

    float pushedDistanceScalar = 11;
    float pushedTime = 0.6f;
    public enum State
    {
        INVALID = -1,
        FREE_MOVE,
        FOCUS_MOVE,
        FREE_PUSH,
        FOCUS_PUSH,
        PUSHED,
        GRABBING,
        REVERT_PREVIOUS_MOVE,
        WIN,
        LOSE
    }

    State lastMotionType = State.FREE_MOVE;

    public enum Event
    {
        SWITCH_FOCUS_MOVE,
        PUSH,
        PUSH_FINISHED,
        BEING_PUSHED,
        GRAB,
        WIN_GRAB,
        LOSE_GRAB,
        TIED_GRAB
    }

    void Awake()
    {

        _transitions.Add(new int[] { (int)State.FREE_MOVE, (int)Event.SWITCH_FOCUS_MOVE }, (int)State.FOCUS_MOVE);
        _transitions.Add(new int[] { (int)State.FOCUS_MOVE, (int)Event.SWITCH_FOCUS_MOVE }, (int)State.FREE_MOVE);

        _transitions.Add(new int[] { (int)State.FREE_MOVE, (int)Event.PUSH }, (int)State.FREE_PUSH);
        _transitions.Add(new int[] { (int)State.FOCUS_MOVE, (int)Event.PUSH }, (int)State.FOCUS_PUSH);

        _transitions.Add(new int[] { (int)State.FREE_PUSH, (int)Event.PUSH_FINISHED }, (int)State.FREE_MOVE);
        _transitions.Add(new int[] { (int)State.FOCUS_PUSH, (int)Event.PUSH_FINISHED }, (int)State.FOCUS_MOVE);

        _transitions.Add(new int[] { (int)State.FREE_MOVE, (int)Event.BEING_PUSHED }, (int)State.PUSHED);
        _transitions.Add(new int[] { (int)State.FOCUS_MOVE, (int)Event.BEING_PUSHED }, (int)State.PUSHED);
        _transitions.Add(new int[] { (int)State.FOCUS_PUSH, (int)Event.BEING_PUSHED }, (int)State.PUSHED);
        _transitions.Add(new int[] { (int)State.FREE_PUSH, (int)Event.BEING_PUSHED }, (int)State.PUSHED);

        _transitions.Add(new int[] { (int)State.PUSHED, (int)Event.PUSH_FINISHED }, (int)State.REVERT_PREVIOUS_MOVE);
        _transitions.Add(new int[] { (int)State.FOCUS_PUSH, (int)Event.GRAB }, (int)State.GRABBING);
        _transitions.Add(new int[] { (int)State.FREE_PUSH, (int)Event.GRAB }, (int)State.GRABBING);

        _transitions.Add(new int[] { (int)State.GRABBING, (int)Event.WIN_GRAB }, (int)State.FOCUS_PUSH);
        _transitions.Add(new int[] { (int)State.GRABBING, (int)Event.LOSE_GRAB }, (int)State.PUSHED);
        _transitions.Add(new int[] { (int)State.GRABBING, (int)Event.TIED_GRAB }, (int)State.REVERT_PREVIOUS_MOVE);

        enabled = false;
    }

    public void Setup(Transform opponentTransform = null)
    {
        SetDefaultParams(opponentTransform);
        //currentState = (int) State.INVALID;
    }

    void FixedUpdate()
    {
        ProcessCurrentState();
    }

    protected override void EnterState()
    {
        switch (currentState)
        {
            case (int)State.FREE_MOVE:
                //lastMovInputDirection = GetDirection();
                Vector2 lastDir = GetOpponentDir();//GetArrowDirection();
                lastMovInputDirection = new Vector3(lastDir.x, 0, lastDir.y);
                lastMotionType = State.FREE_MOVE;
                break;
            case (int)State.FOCUS_MOVE:
                lastMotionType = State.FOCUS_MOVE;
                break;
            case (int)State.FREE_PUSH:
                pushForce = 1;
                pushDecreasementCount = 0;
                transform.DOMove(transform.position + lastMovInputDirection * pushDistanceScalar, pushTime).OnComplete(PushCompleted);


                break;
            case (int)State.FOCUS_PUSH:
                pushCount = 0;
                pushForce = 1;
                pushDecreasementCount = 0;
                Vector3 dir = opponentTransform.position - transform.position;

                transform.DOMove(transform.position + dir.normalized * pushDistanceScalar, pushTime).OnComplete(PushCompleted);

                break;
            case (int)State.PUSHED:
                pushCount = 0;
                transform.DOKill();
                transform.DOMove(transform.position + -GetOpponentDirection() * pushedDistanceScalar, pushedTime).OnComplete(PushedComplete);
                break;
            case (int)State.WIN:
                // TODO: Armar una animación para esto.
                Debug.Log(gameObject.name + " dice: Gané wacho");
                break;
            case (int)State.LOSE:
                // TODO: Armar una animación para esto.
                break;
            case (int)State.GRABBING:
                pushForce = 0;
                transform.DOKill();
                //characterController.Move(lastMovInputDirection * 0.01f);
                break;
            case (int)State.REVERT_PREVIOUS_MOVE:
                ForceChangeState((int)lastMotionType);
                pushForce = 0;
                break;
        }
    }

    void ProcessCurrentState()
    {
        switch (currentState)
        {
            case (int)State.FREE_MOVE:
                ProcessMove();
                //modelTransform.LookAt(modelTransform.position + lastMovInputDirection);
                transform.LookAt(transform.position + lastMovInputDirection);

                break;
            case (int)State.FOCUS_MOVE:
                ProcessMove();
                transform.LookAt(opponentTransform.position);

                break;
            case (int)State.FREE_PUSH:
                DecreasePushForce();
                //Debug.Log(pushForce);
                break;
            case (int)State.FOCUS_PUSH:
                DecreasePushForce();
                break;
            case (int)State.PUSHED:
                pushForce = Mathf.Clamp(pushForce, 0.4f, 1);
                break;
        }
    }

    void ProcessMove()
    {

        if (movInputDirection == Vector3.zero)
        {
            currentSpeed = currentSpeed > maxSpeed / 10f ? Mathf.Lerp(currentSpeed, 0, deceleration) : 0;
        }
        else
        {
            lastMovInputDirection = movInputDirection;
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration);
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }

        characterController.Move(lastMovInputDirection * currentSpeed);

        // Parche para que no se desfase en el eje Y.
        Vector3 t = transform.position;
        t.y = Mathf.Clamp(t.y, 0, 0.05f);
        transform.position = t;
        //

    }

    void DecreasePushForce()
    {
        if (pushDecreasementCount < 1)
        {
            //push_force = (0 + pow(2, -10 * push_decreasement_count))
            pushForce = 0 + Mathf.Pow(1 - pushDecreasementCount, 3);
            pushDecreasementCount += Time.deltaTime;
            pushForce = Mathf.Clamp(pushForce, 0.1f, 1f);
        }
    }

    public float GetCurrentForce() => pushForce;

    Vector3 GetOpponentDirection() => (opponentTransform.position - transform.position).normalized;

    void PushCompleted()
    {
        ChangeState((int)Event.PUSH_FINISHED);
    }

    void PushedComplete()
    {
        ChangeState((int)Event.PUSH_FINISHED);
    }

    public void GoToOriginalPosition()
    {
        characterController.enabled = false;
        ForceChangeState((int)State.INVALID);
        transform.LookAt(originalPosition);
        transform.DOMove(originalPosition, 1.5f).OnComplete(GoToOriginalPositionCompleted);
    }

    void GoToOriginalPositionCompleted()
    {
        transform.DOLookAt(opponentTransform.position, 0.3f);
        characterController.enabled = true;
        ForceChangeState((int)State.FOCUS_MOVE);
        //Debug.Log(originalPosition);
    }

    public void AddPushCount() => pushCount++;
    public int GetPushCount() => pushCount;
    public void SetMovInputDirection(Vector3 input) => movInputDirection = new Vector3(input.x, 0, input.z);
    public void SetOriginalPosition(Vector3 pos) => originalPosition = pos;
    public float GetSize() => size;
    public State GetState() => (State)currentState;


}
