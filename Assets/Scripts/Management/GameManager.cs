using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private void Awake() {
        Game.Manager = this;
    }

    public GameObject BulletPrefab;
}
