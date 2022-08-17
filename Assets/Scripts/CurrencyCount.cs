using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyCount : MonoBehaviour
{
    private int count = 0;
    private TextMeshProUGUI countText;

    // Start is called before the first frame update
    void Start()
    {
        // We start at 0 currency
        count = 0;
        // We update the text shown on screen
        countText = GetComponent<TextMeshProUGUI>();
        countText.text = "¢" + count.ToString();
    }

    public int getCount()
    {
        return count;
    }

    public void setCount(int value)
    {
        count = value;
        // Currency cannot be negative
        if (count < 0)
        {
            count = 0;
        }
        countText.text = "¢" + count.ToString();
    }
}
