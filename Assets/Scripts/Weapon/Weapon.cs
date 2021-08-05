using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Weapon : MonoBehaviour
{
    public Transform ShotPoint = null;
    public ParticleSystem ActivateParticles = null;
    public int ClipSize = 12;
    public float ReloadTime = 0.65f;
    public float ShotDelay = 0.4f;
}
