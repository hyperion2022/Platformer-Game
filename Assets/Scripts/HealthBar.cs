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
        if (value > 0)
        {
            health = value;
        }
        if (health > MAX_HEALTH)
        {
            health = MAX_HEALTH;
        }
        healthBar.fillAmount = (float) health / MAX_HEALTH;
        healthText.text = health.ToString() + " / " + MAX_HEALTH.ToString();
    }
}
