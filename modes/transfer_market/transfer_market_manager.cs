using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerMarketData
{
    public int playerId;
    public float basePriceCC;
    public float currentPriceCC;
    public int views;
    public int purchases;
    public float performanceScore; // e.g., 0-100
    public float rarityScore; // e.g., 0-1
}

public class TransferMarketManager : MonoBehaviour
{
    [Header("Market Settings")]
    public float updateInterval = 30f; // seconds
    public float minMultiplier = 0.5f;
    public float maxMultiplier = 2.0f;

    [Header("Factor Weights")]
    [Range(0,1)] public float demandWeight = 0.4f;
    [Range(0,1)] public float performanceWeight = 0.3f;
    [Range(0,1)] public float rarityWeight = 0.2f;
    [Range(0,1)] public float globalTrendWeight = 0.1f;

    [Header("UI Feedback")]
    public Color priceIncreaseColor = Color.green;
    public Color priceDecreaseColor = Color.red;
    public float feedbackDuration = 0.5f;

    [Header("Market Data")]
    public List<PlayerMarketData> players = new List<PlayerMarketData>();

    private float globalMarketTrend = 0.0f; // calculated externally or default 0

    void Start()
    {
        StartCoroutine(UpdateMarketPrices());
    }

    IEnumerator UpdateMarketPrices()
    {
        while(true)
        {
            foreach(var player in players)
            {
                float demandFactor = CalculateDemandFactor(player);
                float newPrice = player.basePriceCC * 
                                 (1 + demandFactor*demandWeight + 
                                     player.performanceScore/100f*performanceWeight + 
                                     player.rarityScore*rarityWeight + 
                                     globalMarketTrend*globalTrendWeight);

                // Apply min/max limits
                newPrice = Mathf.Clamp(newPrice, player.basePriceCC*minMultiplier, player.basePriceCC*maxMultiplier);

                // Trigger UI feedback if price changed
                if(newPrice > player.currentPriceCC)
                {
                    TriggerPriceUI(player.playerId, true); // true = increase
                }
                else if(newPrice < player.currentPriceCC)
                {
                    TriggerPriceUI(player.playerId, false); // false = decrease
                }

                player.currentPriceCC = newPrice;
            }

            // Wait for next interval
            yield return new WaitForSeconds(updateInterval);
        }
    }

    float CalculateDemandFactor(PlayerMarketData player)
    {
        // Simple demand formula: (views + purchases*2) normalized
        float factor = (player.views + player.purchases*2) / 100f;
        return Mathf.Clamp(factor, 0f, 1f);
    }

    void TriggerPriceUI(int playerId, bool increase)
    {
        // TODO: connect to your UI system to show flashing price
        Color flashColor = increase ? priceIncreaseColor : priceDecreaseColor;
        Debug.Log($"Player {playerId} price {(increase ? "increased" : "decreased")} - flash UI color {flashColor}");
    }

    // Optional: call this from network manager to sync prices with other clients
    public void SyncMarketPrices(List<PlayerMarketData> updatedData)
    {
        foreach(var updatedPlayer in updatedData)
        {
            var localPlayer = players.Find(p => p.playerId == updatedPlayer.playerId);
            if(localPlayer != null)
                localPlayer.currentPriceCC = updatedPlayer.currentPriceCC;
        }
    }
}
