using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const int MAX_HEALTH = 100;

    public int health = 50;

    private Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI deathText;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        healthBar.fillAmount = (float) health / MAX_HEALTH;
        healthText.text = health.ToString() + " / " + MAX_HEALTH.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float) health / MAX_HEALTH;
    }

    public int getHealth()
    {
        return health;
    }

    public void setHealth(int value)
    {
        health = value;
        if (health > MAX_HEALTH)
        {
            health = MAX_HEALTH;
        }
        if (health < 0)
        {
            health = 0;
        }
        healthBar.fillAmount = (float) health / MAX_HEALTH;
        healthText.text = health.ToString() + " / " + MAX_HEALTH.ToString();
        if (health == 0)
        {
            deathText.text = "You died!";
            Time.timeScale = 0.0001f;
        }
    }
}
