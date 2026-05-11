using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.QuantumInventory
{
    
    [AddComponentMenu("Quantum Tek/Quantum Inventory/Vendor (Achat Uniquement)")]
    [DisallowMultipleComponent]
    public class QI_Vendor : MonoBehaviour
    {
        [Tooltip("L'inventaire qui recevra les objets achetés.")]
        public QI_Inventory Inventory;
        
        public Dictionary<string, QI_CurrencyStash> Currencies = new Dictionary<string, QI_CurrencyStash>();

       void Start()
        {
        int savedCoins = PlayerPrefs.GetInt("SavedCoins", 0);
        AddCurrency("Coins", savedCoins);
        }
        public bool Buy(QI_ItemData item, string currencyName, int amount = 1)
        {
            if (!CanBuy(currencyName, item.Price, amount))
            {
                Debug.LogWarning($"Achat refusé : Pas assez de {currencyName} pour acheter {amount}x {item.Name} !");
                return false;
            }

            QI_CurrencyStash currency = Currencies[currencyName];
            currency.Amount -= item.Price * amount;
            Currencies[currencyName] = currency;

            PlayerPrefs.SetInt("SavedCoins", (int)currency.Amount);
            PlayerPrefs.Save();

            Inventory.AddItem(item, amount);
            Debug.Log("Achat réussi !");
            return true;
        }

      
        public bool CanBuy(string currencyName, float itemCost, int amount)
        {
            float totalCost = itemCost * amount;
            
            if (!Currencies.ContainsKey(currencyName))
                return false; 
                
            if (Currencies[currencyName].Amount < totalCost && !Mathf.Approximately(Currencies[currencyName].Amount, totalCost))
                return false; 

            return true;
        }

       
        public float GetCurrency(string name)
        {
            if (Currencies.ContainsKey(name))
                return Currencies[name].Amount;
            return 0;
        }

     
        public void AddCurrency(QI_Currency currency, float amount = 0)
        {
            if (!Currencies.ContainsKey(currency.Name))
            {
                Currencies.Add(currency.Name, new QI_CurrencyStash { Currency = currency, Amount = amount });
            }
            else
            {
                QI_CurrencyStash stash = Currencies[currency.Name];
                stash.Amount += amount;
                Currencies[currency.Name] = stash;
            }
        }

       
        public void AddCurrency(string name, float amount)
        {
            if (Currencies.ContainsKey(name))
            {
                QI_CurrencyStash stash = Currencies[name];
                stash.Amount += amount;
                Currencies[name] = stash;
            }
        }

       
        public void RemoveCurrency(string name, float amount)
        {
            if (Currencies.ContainsKey(name))
            {
                QI_CurrencyStash stash = Currencies[name];
                stash.Amount = Mathf.Clamp(stash.Amount - amount, 0, stash.Amount + amount);
                Currencies[name] = stash;
            }
        }
    }
}