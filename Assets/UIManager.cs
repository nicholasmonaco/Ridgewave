using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Awake() {
        Game.UI = this;
    }


    public RectTransform Crosshair;
    //more
}
