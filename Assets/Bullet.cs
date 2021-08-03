using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class Bullet : NetworkBehaviour
{
    private LayerMask _hitMask;

    private void Start() {
        if (IsOwner) {
            GetComponent<Rigidbody>().velocity = transform.forward * 50;
            _hitMask = LayerMask.GetMask("POV", "LocalPlayer");
            StartCoroutine(DelayDeath(2f));
        } else {
            GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator DelayDeath(float lifetime) {
        float remaining = lifetime;

        while(remaining > 0) {
            yield return null;
            remaining -= Time.deltaTime;
        }

        DestroyBulletServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
    }

    private void OnTriggerEnter(Collider other) {
        if(!(_hitMask == (_hitMask | (1 << other.gameObject.layer)))) {
            StopAllCoroutines();
            DestroyBulletServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
        }
    }


    [ServerRpc]
    private void DestroyBulletServerRpc(ulong netId) {
        NetworkSpawnManager.SpawnedObjects[netId].Despawn(true);
    }
}
