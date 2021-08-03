using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _charController;
    public Weapon Weapon { get; private set; }

    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _weaponContainer;
    [SerializeField] private Transform _bobbingContainer;

    public float Speed = 5;
    public float MouseSensitivity = 3;
    public float LookSpeed = 3;

    private float _pitch = 0f;

    private Vector3 _origBobPos;
    private float _bobOffset = 0;

    private int _clipRemaining;
    private float _shotTimer = 0f;



    private void Start() {
        if (IsLocalPlayer) {
            _charController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;

            LoadWeapon();
            _origBobPos = _bobbingContainer.localPosition;

        } else {
            Destroy(_cameraParent.gameObject);
            Destroy(this);
        }
         
    }


    private void Update() {
        if(IsLocalPlayer) {
            Move();
            Look();
            Attack();
        }
    }


    private void LoadWeapon(/*WeaponID*/) {
        // Destroy old weapon
        // Load new weapon

        Weapon = _weaponContainer.GetComponentInChildren<Weapon>();

        _clipRemaining = Weapon.ClipSize;
        _shotTimer = 0;
    }


    private void Move() {
        Vector2 inputVec = Game.Input.Player.Move.ReadValue<Vector2>();
        Vector3 moveVec = new Vector3(inputVec.x, 0, inputVec.y).normalized;
        moveVec = Quaternion.LookRotation(_cameraParent.forward) * moveVec;
        _charController.SimpleMove(moveVec * Speed);

        float zeroCheckedSpeed = moveVec.magnitude == 0 ? 0.15f : 1;
        float bobDiv = 50 / zeroCheckedSpeed;

        _bobOffset = Mathf.Sin(Time.time * 10 * zeroCheckedSpeed);
        _bobbingContainer.localPosition = Vector3.Lerp(
            _bobbingContainer.localPosition,
            _origBobPos + new Vector3(_bobOffset / bobDiv, _bobOffset / bobDiv, 0), Time.deltaTime * 20);
    }

    private void Look() {
        Vector2 lookVec = Game.Input.Player.Look.ReadValue<Vector2>() * MouseSensitivity;

        _pitch -= lookVec.y;
        _pitch = Mathf.Clamp(_pitch, -70f, 70f);
        // _cameraParent.localRotation = Quaternion.Euler(_pitch, _cameraParent.localRotation.eulerAngles.y + lookVec.x, 0);

        _cameraParent.localRotation = Quaternion.Lerp(_cameraParent.localRotation,
                                                      Quaternion.Euler(_pitch, _cameraParent.localRotation.eulerAngles.y + lookVec.x, 0),
                                                      Time.deltaTime * LookSpeed);
    }

    private void Attack() {
        BasicShoot();
    }

    private void BasicShoot() {
        if (_shotTimer <= 0) {
            if (Game.Input.Player.PrimaryAttack.ReadValue<float>() > 0) {
                _shotTimer += Weapon.ShotDelay;
                PistolShot();
            }
        } else {
            _shotTimer -= Time.deltaTime;
        }
    }

    private void PistolShot() {
        Vector3 shotPoint = Weapon.ShotPoint.position;
        Vector3 aimPoint;

        Vector2 center = Game.UI.Crosshair.anchoredPosition;
        Vector3 origin = _playerCamera.ScreenToWorldPoint(new Vector3(center.x, center.y, 0));
        float maxDist = 500; //idk
        RaycastHit hit;

        if (Physics.Raycast(new Ray(origin, _cameraParent.forward), out hit)) {
            //actually hit something
            aimPoint = hit.point;
        } else {
            aimPoint = origin + _cameraParent.forward * maxDist;
        }

        //calculate where we look at
        Vector3 shotDir = (aimPoint - shotPoint).normalized;

        Weapon.ActivateParticles.Play();

        SpawnProjectileServerRpc(NetworkManager.Singleton.LocalClientId,
                                 shotPoint, Quaternion.LookRotation(shotDir));
    }


    [ServerRpc]
    private void SpawnProjectileServerRpc(ulong netId, Vector3 position, Quaternion rotation) {
        NetworkObject no = Instantiate(Game.Manager.BulletPrefab, position, rotation).GetComponent<NetworkObject>();
        no.SpawnWithOwnership(netId);
        ulong itemNetId = no.NetworkObjectId;

        SpawnProjectileClientRpc(itemNetId);
    }

    [ClientRpc]
    private void SpawnProjectileClientRpc(ulong itemNetId) {
        NetworkObject netObj = NetworkSpawnManager.SpawnedObjects[itemNetId];

        GameObject _spawnedProjectile = netObj.gameObject;
    }
}
