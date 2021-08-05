using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class DeathPlane : NetworkBehaviour
{
    private static LayerMask _hitMask;


    private void Start() {
        _hitMask = LayerMask.GetMask("LocalPlayer", "OtherPlayer");
    }


    private void OnTriggerEnter(Collider other) {
        if (!IsServer) return;

        if (_hitMask == (_hitMask | (1 << other.gameObject.layer))) {
            HitboxChild hitbox = other.gameObject.GetComponent<HitboxChild>();
            if (hitbox == null) return;

            NetworkBehaviour player = hitbox.ParentObject;
            HitPlayerServerRpc(player.OwnerClientId, player.NetworkObjectId, 9999);
        }
    }


    [ServerRpc]
    private void HitPlayerServerRpc(ulong clientId, ulong netId, int damage) {
        //ClientRpcParams clientRpcParams = new ClientRpcParams {
        //    Send = new ClientRpcSendParams {
        //        TargetClientIds = new ulong[] { clientId }
        //    }
        //};

        NetworkSpawnManager.SpawnedObjects[netId].gameObject.GetComponent<PlayerController>().Damage(damage);
        //HitPlayerClientRPC(netId, damage);
    }

    [ClientRpc]
    private void HitPlayerClientRPC(ulong netId, int damage) {
        //if (IsOwner) return; //why

        NetworkSpawnManager.SpawnedObjects[netId].gameObject.GetComponent<PlayerController>().Damage(damage);
    }
}
