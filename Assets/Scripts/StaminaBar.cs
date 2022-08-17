using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private const int MAX_STAMINA = 50;

    public int stamina = 25;
    [SerializeField] TextMeshProUGUI staminaText;
    [SerializeField] TextMeshProUGUI fatigueText;
    private float timeOfFatigueMessage;

    private Image staminaBar;

    // Start is called before the first frame update
    void Start()
    {
        staminaBar = GetComponent<Image>();
        staminaBar.fillAmount = (float) stamina / MAX_STAMINA;
        staminaText.text = stamina.ToString() + " / " + MAX_STAMINA.ToString();
        timeOfFatigueMessage = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.fillAmount = (float) stamina / MAX_STAMINA;
        if ((Time.time - timeOfFatigueMessage) > 1f)
        {
            fatigueText.text = "";
        }
    }

    public int getStamina()
    {
        return stamina;
    }

    public void setStamina(int value)
    {
        stamina = value;
        if (stamina > MAX_STAMINA)
        {
            stamina = MAX_STAMINA;
        }
        if (stamina < 0)
        {
            stamina = 0;
        }
        staminaBar.fillAmount = (float) stamina / MAX_STAMINA;
        staminaText.text = stamina.ToString() + " / " + MAX_STAMINA.ToString();
        if (stamina == 0)
        {
            fatigueText.text = "Empty stamina!";
            timeOfFatigueMessage = Time.time;
        }
    }
}
