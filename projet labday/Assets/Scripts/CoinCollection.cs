using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    private int coin = 0;
    public TextMeshProUGUI coinText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Coin")
        {
            int randomAmount = Random.Range(50, 151);

            coin += randomAmount;

            coinText.text = "Coins: " + coin.ToString();

            Debug.Log("Gagné: " + randomAmount + " | Total: " + coin);

            Destroy(other.gameObject);
        }
    }
}