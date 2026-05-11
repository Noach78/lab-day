using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinCollection : MonoBehaviour
{
    private int coin = 0;
    public TextMeshProUGUI coinText;

    private const string COIN_SAVE_KEY = "SavedCoins";

    private void Start()
    {
        coin = PlayerPrefs.GetInt(COIN_SAVE_KEY, 0);
        UpdateCoinUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Coin")
        {
            int randomAmount = Random.Range(50, 151);
            coin += randomAmount;
            
            UpdateCoinUI();
            SaveCoins();

            Debug.Log("Gagné: " + randomAmount + " | Total: " + coin);
            Destroy(other.gameObject);
        }
    }

    public bool SpendCoins(int amount)
    {
        if (coin >= amount)
        {
            coin -= amount;
            UpdateCoinUI();
            SaveCoins();
            return true; 
        }
        return false; 
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_SAVE_KEY, coin);
        PlayerPrefs.Save(); 
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coin.ToString();
        }
    }
}