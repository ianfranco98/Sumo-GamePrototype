using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using MLAPI;
using MLAPI.Transports;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.Serialization.Pooled;

// Script "puente" entre el objeto Player del cliente y el Dummy del Host.

// TODO: Cambiar force por una variable común que se actualice por funcionRPC

public class PlayerNetworkedVar : NetworkedBehaviour
{
    public string Channel = null;
    private PlayerStateMachine playerReference;
    private NetworkedVarFloat force = new NetworkedVarFloat(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone }, 0);


    int _state;
    int _previousState;

    List<ulong> listEnemyID;
    ulong enemyID;

    bool initialized;

    void Start()
    {
        listEnemyID = new List<ulong>();
        enemyID = (ulong)1;
        listEnemyID.Add(enemyID);
    }

    public void Setup()
    {

        playerReference = gameObject.GetComponent<PlayerStateMachine>();
        initialized = true;

    }

    void Update()
    {
        if (initialized)
        {
            _state = (int)playerReference.GetState();

            SendServerMessage(UpdateNetworkedState, _state);

            _previousState = (int)playerReference.GetState();

            //state.Value = (int)playerReference.GetState();
            //force.Value = (int)playerReference.GetCurrentForce();
            //Debug.Log("State: " + state.Value + "Force: " + force.Value + "playerReference: " + playerReference);
        }
    }

    public int GetState() => _state;
    public float GetCurrentForce() => force.Value;

    public void ChangeState(int newEvent) => SendMessage(ChangeNetworkedState, newEvent);
    public void ForceChangeState(int newState) => SendMessage(ForceNetworkedState, newState);
    public void GoToOriginalPosition() => InvokeClientRpcOnEveryoneExceptPerformance(CallGoToOriginalPosition, NetworkingManager.Singleton.LocalClientId, PooledBitStream.Get());

    //---------------------------------------------

    [ServerRPC]
    private void UpdateNetworkedState(ulong clientId, Stream stream)
    {
        if (!enabled) return;
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            _state = reader.ReadByte();
        }
    }

    [ClientRPC]
    private void ChangeNetworkedState(ulong clientId, Stream stream)
    {
        if (!enabled) return;
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            int newEvent = reader.ReadByte();
            playerReference.ChangeState(newEvent);
        }
    }

    [ClientRPC]
    private void ForceNetworkedState(ulong clientId, Stream stream)
    {
        if (!enabled) return;
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            int newState = reader.ReadByte();
            playerReference.ForceChangeState(newState);
        }
    }

    [ClientRPC]
    private void CallGoToOriginalPosition(ulong clientId, Stream stream)
    {
        playerReference.GoToOriginalPosition();
    }

    //---------------------------------------------


    void SendMessage(RpcDelegate rpc, int msj)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt16((short)msj);
            }
            InvokeClientRpcOnEveryoneExceptPerformance(rpc, NetworkingManager.Singleton.LocalClientId, stream);

        }
    }

    void SendServerMessage(RpcDelegate rpc, int msj)
    {
        using (PooledBitStream stream = PooledBitStream.Get())
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt16((short)msj);
            }
            InvokeServerRpcPerformance(rpc, stream);

        }
    }



}
