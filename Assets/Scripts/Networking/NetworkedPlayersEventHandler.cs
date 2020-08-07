using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MLAPI;
using MLAPI.Messaging;

// Igual que la clase base excepto que sobrescribe las funciones del jugador 2 
// para que se comunique via red. El script "PlayerNetworkedVar" se encarga de
// gestionar esas funciones.

public class NetworkedPlayersEventHandler : PlayersEventHandler
{
    PlayerNetworkedVar playerTwoNET;

    public override void Setup(Transform playerOne, Transform playerTwo, Vector3 arenaCenter = new Vector3()){
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;

        playerOneSM = this.playerOne.gameObject.GetComponent<PlayerStateMachine>();
        playerTwoNET = this.playerTwo.gameObject.GetComponent<PlayerNetworkedVar>();

        minDistCollision = playerOneSM.GetSize() * 2;//(playerOneSM.GetSize() + playerTwoSM.GetSize()) / 2;
        this.arenaCenter = arenaCenter;
        arenaRadius = 12;
        tieRange = 0.2f;
        enabled = true;
    }

    public override void PositionPlayersToOrigin(){
        playerOneSM.GoToOriginalPosition();
        playerTwoNET.GoToOriginalPosition();
    }

    public override void TellPlayersWhoWin(OnGameFightLogic.Player p){
        switch(p){
            case OnGameFightLogic.Player.ONE:
                playerOneSM.ForceChangeState((int) PlayerStateMachine.State.WIN);
                playerTwoNET.ForceChangeState((int) PlayerStateMachine.State.LOSE);
            break;
            case OnGameFightLogic.Player.TWO:
                playerOneSM.ForceChangeState((int) PlayerStateMachine.State.LOSE);
                playerTwoNET.ForceChangeState((int) PlayerStateMachine.State.WIN);
            break;
            case OnGameFightLogic.Player.BOTH:
                playerOneSM.ForceChangeState((int) PlayerStateMachine.State.LOSE);
                playerTwoNET.ForceChangeState((int) PlayerStateMachine.State.LOSE);
            break;
        }
    }

    public override void PushPlayer2() => playerTwoNET.ChangeState((int) PlayerStateMachine.Event.BEING_PUSHED);

    public override int WhoHasGotStrongestPush(){
        float p1Force = playerOneSM.GetCurrentForce();
        float p2Force = playerTwoNET.GetCurrentForce();

        int winner = 0;

        if (p1Force > p2Force + tieRange){
            winner = 1;
        } else if (p2Force > p1Force + tieRange){
            winner = 2;
        }
        return winner;
    }
    
    public override bool IsAttackingP2() => (PlayerStateMachine.State) playerTwoNET.GetState() == PlayerStateMachine.State.FREE_PUSH || (PlayerStateMachine.State) playerTwoNET.GetState() == PlayerStateMachine.State.FOCUS_PUSH;
}
