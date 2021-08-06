using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject BulletPrefab;
    public GameObject DeathPlanePrefab;

    public PauseMenu PauseMenu;


    [HideInInspector] public bool InGame = false;



    private void Awake() {
        Cursor.lockState = CursorLockMode.None;

        Game.Manager = this;
    }

    private void LateUpdate() {
        if (Options.DoingUpdate) Options.UpdateActions();
    }
}
