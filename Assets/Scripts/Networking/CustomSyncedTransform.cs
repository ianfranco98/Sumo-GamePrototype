using System.Collections.Generic;
using System.IO;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Serialization.Pooled;
using UnityEngine;

public class CustomSyncedTransform : NetworkedBehaviour
{
    [Range(0, 120)]
    public float FixedSendsPerSecond = 20f;
    public float MinMeters = 0.15f;
    public float MinDegrees = 1.5f;
    public string Channel = null;

    private float lerpT;
    private Vector3 lerpStartPos;
    private Quaternion lerpStartRot;
    private Vector3 lerpEndPos;
    private Quaternion lerpEndRot;
    private float lastSendTime;
    private Vector3 lastSentPos;
    private Quaternion lastSentRot;
    private float lastRecieveTime;
    
    public override void NetworkStart()
    {
        lastSentRot = transform.rotation;
        lastSentPos = transform.position;

        lerpStartPos = transform.position;
        lerpStartRot = transform.rotation;

        lerpEndPos = transform.position;
        lerpEndRot = transform.rotation;
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (NetworkingManager.Singleton.NetworkTime - lastSendTime >= (1f / FixedSendsPerSecond) && (Vector3.Distance(transform.position, lastSentPos) > MinMeters || Quaternion.Angle(transform.rotation, lastSentRot) > MinDegrees))
            {
                lastSendTime = NetworkingManager.Singleton.NetworkTime;
                lastSentPos = transform.position;
                lastSentRot = transform.rotation;
                using (PooledBitStream stream = PooledBitStream.Get())
                {
                    using (PooledBitWriter writer = PooledBitWriter.Get(stream))
                    {
                        writer.WriteSinglePacked(transform.position.x);
                        writer.WriteSinglePacked(transform.position.z);

                        writer.WriteSinglePacked(transform.rotation.eulerAngles.y);

                        if (IsServer)
                            InvokeClientRpcOnEveryoneExceptPerformance(ApplyTransform, OwnerClientId, stream);
                        else{
                            InvokeServerRpcPerformance(SubmitTransform, stream);
                        }
                    }
                }
            }
        }
        else
        {
            //Toda este bloque se encarga de mover al dummy

            float sendDelay = 1f / FixedSendsPerSecond;
            lerpT += Time.unscaledDeltaTime / sendDelay;

            transform.position = Vector3.Lerp(lerpStartPos, lerpEndPos, lerpT);
            transform.rotation = Quaternion.Slerp(lerpStartRot, lerpEndRot, lerpT);
            //transform.position = lerpEndPos;
            //transform.rotation = lerpEndRot;
        }
    }

    [ClientRPC]
    private void ApplyTransform(ulong clientId, Stream stream)
    {
        if (!enabled) {
            Debug.Log("idk");
            return;
        }
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            float xPos = reader.ReadSinglePacked();
            float zPos = reader.ReadSinglePacked();

            float yRot = reader.ReadSinglePacked();

            lastRecieveTime = Time.unscaledTime;
            lerpStartPos = transform.position;
            lerpStartRot = transform.rotation;
            lerpEndPos = new Vector3(xPos, transform.position.y, zPos);
            lerpEndRot = Quaternion.Euler(0, yRot, 0);
            lerpT = 0;

            //transform.position = new Vector3(xPos, transform.position.y, zPos);
            //transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));
        }
    }

    /*[ServerRPC]
    private void SubmitTransform(ulong clientId, Stream stream)
    {
        if (!enabled) return;
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            float xPos = reader.ReadSinglePacked();
            float zPos = reader.ReadSinglePacked();

            float yRot = reader.ReadSinglePacked();

            using (PooledBitStream writeStream = PooledBitStream.Get())
            {
                using (PooledBitWriter writer = PooledBitWriter.Get(writeStream))
                {
                    writer.WriteSinglePacked(xPos);
                    writer.WriteSinglePacked(zPos);

                    writer.WriteSinglePacked(yRot);

                    InvokeClientRpcOnEveryoneExceptPerformance(ApplyTransform, OwnerClientId, writeStream, string.IsNullOrEmpty(Channel) ? "MLAPI_DEFAULT_MESSAGE" : Channel);
                }
            }
        }
    }*/
    [ServerRPC(RequireOwnership = false)]
    private void SubmitTransform(ulong clientId, Stream stream)
    {
        if (!enabled) return;
        using (PooledBitReader reader = PooledBitReader.Get(stream))
        {
            float xPos = reader.ReadSinglePacked();
            float zPos = reader.ReadSinglePacked();

            float yRot = reader.ReadSinglePacked();

            lastRecieveTime = Time.unscaledTime;
            lerpStartPos = transform.position;
            lerpStartRot = transform.rotation;
            lerpEndPos = new Vector3(xPos, transform.position.y, zPos);
            lerpEndRot = Quaternion.Euler(0, yRot, 0);
            lerpT = 0;

            //transform.position = new Vector3(xPos, transform.position.y, zPos);
            //transform.rotation = Quaternion.Euler(new Vector3(0, yRot, 0));
        }
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {   
        lerpStartPos = position;
        lerpStartRot = rotation;
        lerpEndPos = position;
        lerpEndRot = rotation;
        lerpT = 0;
    }
}
