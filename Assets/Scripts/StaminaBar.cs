using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private const float MAX_STAMINA = 50f;

    public float stamina = 25f;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI fatigueText;
    private float timeOfFatigueMessage;

    private Image staminaBar;

    // Start is called before the first frame update
    void Start()
    {
        staminaBar = GetComponent<Image>();
        // We show the stamina value on screen
        staminaBar.fillAmount = stamina / MAX_STAMINA;
        staminaText.text = ((int) stamina).ToString() + " / " + ((int) MAX_STAMINA).ToString();
        // We start counting since the last time we showed the fatigue message on screen
        timeOfFatigueMessage = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.fillAmount = stamina / MAX_STAMINA;
        // If one second passed since the last fatigue message, we erase it
        if ((Time.time - timeOfFatigueMessage) > 1f)
        {
            fatigueText.text = "";
        }
    }

    public float getStamina()
    {
        return stamina;
    }

    public void setStamina(float value)
    {
        stamina = value;
        // Stamina cannot be negative or exceed 100%
        if (stamina > MAX_STAMINA)
        {
            stamina = MAX_STAMINA;
        }
        if (stamina < 0)
        {
            stamina = 0;
        }
        staminaBar.fillAmount = stamina / MAX_STAMINA;
        staminaText.text = ((int)stamina).ToString() + " / " + ((int)MAX_STAMINA).ToString();
        showFatigueMessage();
    }

    public void showFatigueMessage()
    {
        if (stamina < 1f)
        {
            // If stamina < 1, we lock it to 0 and show the fatigue text
            stamina = 0f;
            fatigueText.text = "Empty stamina!";
            timeOfFatigueMessage = Time.time;
        }
    }
}
