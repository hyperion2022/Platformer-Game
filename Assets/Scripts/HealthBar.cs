using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const float MAX_HEALTH = 100f;

    public float health = 50f;

    private Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI deathText;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        // We start at 100% health and show it on screen
        healthBar.fillAmount = health / MAX_HEALTH;
        healthText.text = ((int) health).ToString() + " / " + ((int) MAX_HEALTH).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = health / MAX_HEALTH;
    }

    public float getHealth()
    {
        return health;
    }

    public void setHealth(float value)
    {
        health = value;
        // Health cannot be negative or exceed 100%
        if (health > MAX_HEALTH)
        {
            health = MAX_HEALTH;
        }
        if (health < 0)
        {
            health = 0;
        }
        healthBar.fillAmount = health / MAX_HEALTH;
        healthText.text = ((int) health).ToString() + " / " + ((int) MAX_HEALTH).ToString();
        if (health == 0)
        {
            // If health is 0, we show a death screen and stop the game
            deathText.text = "You died!";
            Time.timeScale = 0.0001f;
        }
    }
}
