using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script "esclavo" de OnGameFightLogic encargado de comunicarlo con los jugadores. 

public class PlayersEventHandler : MonoBehaviour
{
    protected Transform playerOne, playerTwo;
    protected PlayerStateMachine playerOneSM, playerTwoSM;

    protected float minDistCollision;
    protected float tieRange;
    protected Vector3 arenaCenter;
    protected float arenaRadius;

    void Awake() => enabled = false;

    public virtual void Setup(Transform playerOne, Transform playerTwo, Vector3 arenaCenter = new Vector3())
    {
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;

        playerOneSM = this.playerOne.gameObject.GetComponent<PlayerStateMachine>();
        playerTwoSM = this.playerTwo.gameObject.GetComponent<PlayerStateMachine>();

        minDistCollision = (playerOneSM.GetSize() + playerTwoSM.GetSize()) / 2;
        this.arenaCenter = arenaCenter;
        arenaRadius = 12;
        tieRange = 0.2f;
        enabled = true;
    }


    // Temporal code for test grab battle
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            playerTwoSM.ChangeState((int)PlayerStateMachine.Event.PUSH);
        }
    }
    //

    public virtual void PositionPlayersToOrigin()
    {
        playerOneSM.GoToOriginalPosition();
        playerTwoSM.GoToOriginalPosition();
    }

    public virtual void TellPlayersGrabBattleIsStarting()
    {
        playerOneSM.ChangeState((int)PlayerStateMachine.Event.GRAB);
        playerTwoSM.ChangeState((int)PlayerStateMachine.Event.GRAB);
    }

    public virtual void TellPlayersWhoWinGrabBattle(OnGameFightLogic.Player p)
    {

        switch (p)
        {
            case OnGameFightLogic.Player.ONE:
                playerOneSM.ChangeState((int)PlayerStateMachine.Event.WIN_GRAB);
                playerTwoSM.ChangeState((int)PlayerStateMachine.Event.LOSE_GRAB);
                break;
            case OnGameFightLogic.Player.TWO:
                playerOneSM.ChangeState((int)PlayerStateMachine.Event.LOSE_GRAB);
                playerTwoSM.ChangeState((int)PlayerStateMachine.Event.WIN_GRAB);
                break;
            case OnGameFightLogic.Player.BOTH:
                playerOneSM.ChangeState((int)PlayerStateMachine.Event.LOSE_GRAB);
                playerTwoSM.ChangeState((int)PlayerStateMachine.Event.LOSE_GRAB);
                break;
        }

    }

    public virtual void TellPlayersWhoWin(OnGameFightLogic.Player p)
    {
        switch (p)
        {
            case OnGameFightLogic.Player.ONE:
                playerOneSM.ForceChangeState((int)PlayerStateMachine.State.WIN);
                playerTwoSM.ForceChangeState((int)PlayerStateMachine.State.LOSE);
                break;
            case OnGameFightLogic.Player.TWO:
                playerOneSM.ForceChangeState((int)PlayerStateMachine.State.LOSE);
                playerTwoSM.ForceChangeState((int)PlayerStateMachine.State.WIN);
                break;
            case OnGameFightLogic.Player.BOTH:
                playerOneSM.ForceChangeState((int)PlayerStateMachine.State.LOSE);
                playerTwoSM.ForceChangeState((int)PlayerStateMachine.State.LOSE);
                break;
        }
    }

    public virtual int WhoHasGotStrongestPush()
    {
        float p1Force = playerOneSM.GetCurrentForce();
        float p2Force = playerTwoSM.GetCurrentForce();

        int winner = 0;

        if (p1Force > p2Force + tieRange)
        {
            winner = 1;
        }
        else if (p2Force > p1Force + tieRange)
        {
            winner = 2;
        }
        return winner;
    }

    public void PushPlayer1() => playerOneSM.ChangeState((int)PlayerStateMachine.Event.BEING_PUSHED);
    public virtual void PushPlayer2() => playerTwoSM.ChangeState((int)PlayerStateMachine.Event.BEING_PUSHED);
    public Transform GetP1Transform() => playerOne;
    public Transform GetP2Transform() => playerTwo;
    public Vector3 GetDirectionVector() => (GetP1Transform().position - GetP2Transform().position).normalized;
    public int GetPushCountP1() => playerOneSM.GetPushCount();
    public virtual int GetPushCountP2() => playerTwoSM.GetPushCount();
    public virtual bool PlayersAreColliding() => Vector3.Distance(playerOne.position, playerTwo.position) <= minDistCollision;
    public bool OutOfArenaP1() => Vector3.Distance(playerOne.position, arenaCenter) > arenaRadius;
    public bool OutOfArenaP2() => Vector3.Distance(playerTwo.position, arenaCenter) > arenaRadius;
    public bool IsAttackingP1() => playerOneSM.GetState() == PlayerStateMachine.State.FREE_PUSH || playerOneSM.GetState() == PlayerStateMachine.State.FOCUS_PUSH;
    public virtual bool IsAttackingP2() => playerTwoSM.GetState() == PlayerStateMachine.State.FREE_PUSH || playerTwoSM.GetState() == PlayerStateMachine.State.FOCUS_PUSH;
    public bool BothPlayersAttackAndCollide() => PlayersAreColliding() && IsAttackingP1() && IsAttackingP2();
}
