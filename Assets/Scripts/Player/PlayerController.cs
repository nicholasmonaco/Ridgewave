using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using MLAPI.NetworkVariable;

public class PlayerController : NetworkBehaviour
{
    private CharacterController _charController;
    public Weapon Weapon { get; private set; }

    [SerializeField] private Transform _playerModel;
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _weaponContainer;
    [SerializeField] private Transform _bobbingContainer;

    NetworkVariableInt CurHealth = new NetworkVariableInt();
    NetworkVariableInt MaxHealth = new NetworkVariableInt(100);
    NetworkVariableBool CanMove = new NetworkVariableBool(true);

    public float Speed = 5;
    public float JumpHeight = 3;
    public float MaxYSpeed = 2;
    public float MouseSensitivity = 3;
    public float LookSpeed = 3;

    private float _pitch = 0f;

    private Vector3 _origBobPos;
    private float _bobOffset = 0;

    private float _gravityValue = -0.75f;
    private Vector3 _playerVelocity = Vector3.zero;

    private int _clipRemaining; //to fix the ammo counter on respawn just make it a network variable
    private bool _reloading = false;
    private float _shotTimer = 0f;



    private void OnEnable() {
        CurHealth.OnValueChanged += UpdateUI;
        MaxHealth.OnValueChanged += UpdateUI;

        if (IsServer) {
            CurHealth.Value = MaxHealth.Value;
        }
    }

    private void OnDisable() {
        CurHealth.OnValueChanged -= UpdateUI;
        MaxHealth.OnValueChanged -= UpdateUI;
    }


    private void Start() {
        if (IsLocalPlayer) {
            _charController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;

            LoadWeapon();
            _origBobPos = _bobbingContainer.localPosition;

            Game.UI.AlterHealth(CurHealth.Value, MaxHealth.Value);

            int newLayer = LayerMask.NameToLayer("LocalPlayer");
            gameObject.layer = newLayer;
            _playerModel.gameObject.layer = newLayer;

        } else {
            int newLayer = LayerMask.NameToLayer("OtherPlayer");
            gameObject.layer = newLayer;
            _playerModel.gameObject.layer = newLayer;

            Destroy(_cameraParent.gameObject);
        }

        HitboxChild[] colliders = GetComponentsInChildren<HitboxChild>();
        foreach (HitboxChild hitbox in colliders) {
            hitbox.ParentObject = this;
        }
    }


    private void Update() {
        if(IsLocalPlayer) {
            GetMoveVars();
            Look();
            if(CanMove.Value) Attack();
        }
    }

    private void FixedUpdate() {
        if (IsLocalPlayer) {
            Move();
        }
    }


    private void LoadWeapon(/*WeaponID*/) {
        // Destroy old weapon
        // Load new weapon

        Weapon = _weaponContainer.GetComponentInChildren<Weapon>();

        _clipRemaining = Weapon.ClipSize;
        Game.UI.AlterAmmo(_clipRemaining, Weapon.ClipSize);

        _shotTimer = 0;
    }


    private Vector2 in_moveVec;
    private float in_jump;
    private float in_sprint;

    private void GetMoveVars() {
        in_moveVec = CanMove.Value == true ? Game.Input.Player.Move.ReadValue<Vector2>() : Vector2.zero;
        in_jump = CanMove.Value == true ? Game.Input.Player.Jump.ReadValue<float>() : 0;
        in_sprint = Game.Input.Player.Sprint.ReadValue<float>();
    }

    private void Move() {
        bool grounded = _charController.isGrounded;

        Vector2 inputVec = in_moveVec;
        Vector3 moveVec = new Vector3(inputVec.x, 0, inputVec.y).normalized;
        Vector3 flatForward = new Vector3(_cameraParent.forward.x, 0, _cameraParent.forward.z).normalized;
        moveVec = Quaternion.LookRotation(flatForward) * moveVec;

        float sprintMult = in_sprint > 0 ? 2 : 1;

        // Jumping
        if (grounded && _playerVelocity.y < 0) {
            _playerVelocity.y = 0;
        }

        if (grounded && in_jump > 0) {
            _playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * _gravityValue);
        }

        if (!grounded) {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }

        _playerVelocity.y = Mathf.Clamp(_playerVelocity.y, -MaxYSpeed, MaxYSpeed);

        moveVec *= Speed * sprintMult * Time.deltaTime;

        Vector3 finalMoveVec = new Vector3(moveVec.x, _playerVelocity.y, moveVec.z);
        
        _charController.Move(finalMoveVec);

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
        _pitch = Mathf.Clamp(_pitch, -82.5f, 82.5f);
        // _cameraParent.localRotation = Quaternion.Euler(_pitch, _cameraParent.localRotation.eulerAngles.y + lookVec.x, 0);

        _cameraParent.localRotation = Quaternion.Lerp(_cameraParent.localRotation,
                                                      Quaternion.Euler(_pitch, _cameraParent.localRotation.eulerAngles.y + lookVec.x, 0),
                                                      Time.deltaTime * LookSpeed);
    }



    public void Damage(int damage) {
        if (IsServer) {
            CurHealth.Value -= damage;

            if (CurHealth.Value <= 0) {
                CurHealth.Value = 0;
                //Die(); //this might go in the update ui
                StartCoroutine(Respawn());
            }
        }
    }

    private IEnumerator Respawn() {
        CanMove.Value = false;

        float timer = 4;
        while(timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        float y = 1;

        float r = 30;
        Vector3 newPos = new Vector3(Random.Range(-r, r), 50, Random.Range(-r, r));

        RaycastHit hit;
        if(Physics.Raycast(new Ray(newPos, Vector3.down), out hit, 2000)) {
            newPos.y = hit.point.y + 3;
        }else {
            newPos.y = 1;
        }

        _charController.Move(newPos - transform.position);

        //refresh values
        MaxHealth.Value = 100;
        CurHealth.Value = MaxHealth.Value;

        _clipRemaining = Weapon.ClipSize;
        _shotTimer = 0;
        _reloading = false;
        

        CanMove.Value = true;
    }

    public void UpdateUI(int oldValue, int newValue) {
        if (IsOwner && IsClient) {
            Game.UI.AlterHealth(CurHealth.Value, MaxHealth.Value);
            if(Weapon != null) Game.UI.AlterAmmo(_clipRemaining, Weapon.ClipSize);
        }
    }


    private void Attack() {
        Reload();
        BasicShoot();
    }

    private void Reload() {
        if (Game.Input.Player.Reload.ReadValue<float>() > 0 && !_reloading) {
            DoReload();
        }
    }

    private void DoReload() {
        _reloading = true;
        StartCoroutine(Reload_C(Weapon.ReloadTime));
    }

    private IEnumerator Reload_C(float reloadTime) {
        yield return new WaitForSeconds(reloadTime);
        _clipRemaining = Weapon.ClipSize;
        Game.UI.AlterAmmo(_clipRemaining, Weapon.ClipSize);

        yield return new WaitForEndOfFrame();

        _reloading = false;
    }


    private void BasicShoot() {
        if (_shotTimer <= 0) {
            if(Game.Input.Player.PrimaryAttack.ReadValue<float>() > 0 && !_reloading) {
                if (_clipRemaining > 0) {
                    _shotTimer += Weapon.ShotDelay;
                    PistolShot();
                    _clipRemaining--;
                    Game.UI.AlterAmmo(_clipRemaining, Weapon.ClipSize);
                }else if(_clipRemaining == 0) {
                    DoReload();
                }
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

        if (Physics.Raycast(new Ray(origin, _cameraParent.forward), out hit, maxDist, ~LayerMask.GetMask("POV", "LocalPlayer"))) {
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
        // do logic per client
        //NetworkObject netObj = NetworkSpawnManager.SpawnedObjects[itemNetId];

        //GameObject _spawnedProjectile = netObj.gameObject;
    }
}
