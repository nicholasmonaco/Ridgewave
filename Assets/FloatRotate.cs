using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatRotate : MonoBehaviour
{
    public float YDist = 0.25f;
    public float FloatTime = 2.5f;
    public Vector3 RotationSpeed = Vector3.zero;

    private float _floatTimer = 0;

    private Vector3 _origPos;
    private Vector3 _minPos, _maxPos;



    private void Start() {
        _floatTimer = FloatTime / 2f;

        _origPos = transform.localPosition;
        _minPos = _origPos - new Vector3(0, YDist, 0);
        _maxPos = _origPos + new Vector3(0, YDist, 0);
    }

    private void Update() {
        if(_floatTimer >= 0) {
            _floatTimer -= Time.deltaTime;
        } else {
            _floatTimer += FloatTime;
        }

        float trigFrac = (Mathf.Sin(_floatTimer / FloatTime * Mathf.PI * 2) + 1) / 2f;

        transform.localPosition = Vector3.Lerp(_minPos, _maxPos, trigFrac);

        transform.localRotation = Quaternion.Euler(Time.time * RotationSpeed);
    }
}
