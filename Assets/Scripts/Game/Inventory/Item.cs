using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemObject item;
    public int amount = 1;

    public TextMeshPro amountText;

    private void Awake()
    {
        amountText = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        amountText.text = amount == 1 ? "" : amount.ToString();
    }
}
