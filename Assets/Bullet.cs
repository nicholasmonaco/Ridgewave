using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class Bullet : NetworkBehaviour
{
    [HideInInspector] public int Damage = 8;

    private LayerMask _hitMask;
    private LayerMask _damageMask;



    private void Start() {
        if (IsOwner) {
            GetComponent<Rigidbody>().velocity = transform.forward * 50;
            _hitMask = LayerMask.GetMask("POV", "LocalPlayer");
            _damageMask = LayerMask.GetMask("OtherPlayer");
            StartCoroutine(DelayDeath(2f, () => { DestroyBulletServerRpc(GetComponent<NetworkObject>().NetworkObjectId); }));
        } else {
            GetComponent<Collider>().enabled = false;
            // Destroy(this);
        }
    }

    private IEnumerator DelayDeath(float lifetime, System.Action deathCallback) {
        float remaining = lifetime;

        while(remaining > 0) {
            yield return null;
            remaining -= Time.deltaTime;
        }

        deathCallback();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsOwner) return;

        if(!(_hitMask == (_hitMask | (1 << other.gameObject.layer)))) {
            StopAllCoroutines();

            if (_damageMask == (_damageMask | 1 << other.gameObject.layer)) {
                HitboxChild hitbox = other.gameObject.GetComponent<HitboxChild>();
                if (hitbox == null) return;

                NetworkBehaviour player = hitbox.ParentObject;
                HitPlayerServerRpc(player.OwnerClientId, player.NetworkObjectId, Damage);
            }

            DestroyBulletServerRpc(GetComponent<NetworkObject>().NetworkObjectId); //maybe bad?
        }
    }


    [ServerRpc]
    private void DestroyBulletServerRpc(ulong netId) {
        NetworkSpawnManager.SpawnedObjects[netId].Despawn(true);
    }


    [ServerRpc]
    private void HitPlayerServerRpc(ulong clientId, ulong netId, int damage) {
        NetworkSpawnManager.SpawnedObjects[netId].gameObject.GetComponent<PlayerController>().Damage(damage);
    }
}
