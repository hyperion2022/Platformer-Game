using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private const float MAX_STAMINA = 50f;

    public float stamina = MAX_STAMINA;

    private Image staminaBar;

    // Start is called before the first frame update
    void Start()
    {
        staminaBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.fillAmount = stamina / MAX_STAMINA;
    }
}
