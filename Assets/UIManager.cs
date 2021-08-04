using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private void Awake() {
        Game.UI = this;
    }


    public RectTransform Crosshair;
    public Slider HealthBar;
    public TMP_Text HealthText;
    public TMP_Text WeaponAmmo;



    private void Start() {
        HealthBar.fillRect.gameObject.SetActive(true);
    }


    public void AlterHealth(int curHealth, int maxHealth) {
        float percent = (float)curHealth / maxHealth;

        if (curHealth <= 0) {
            HealthBar.fillRect.gameObject.SetActive(false);
        }else {
            HealthBar.fillRect.gameObject.SetActive(true);
        }

        HealthBar.value = percent;

        HealthText.text = $"{curHealth} / {maxHealth}";
    }

    public void AlterAmmo(int curAmmo, int maxAmmo) {
        WeaponAmmo.text = $"{curAmmo} / {maxAmmo}";
    }
}
