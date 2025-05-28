using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaInWorldDisplayController : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image baseRenderer;

    private Material _instanceMaterial;
    
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineSize = Shader.PropertyToID("_OutlineSize");

    public TMP_Text rewardText;
    
    public TMP_Text areaName;
    
    private void Awake()
    {
        // Clone material instance to avoid global changes
        _instanceMaterial = new Material(baseRenderer.material);
        baseRenderer.material = _instanceMaterial;
        ClearHighlight();
        _instanceMaterial.SetFloat(OutlineSize, 20f);
    }

    public void Init(FightAreaData fightArea)
    {
        ClearHighlight();
        DisplayRewards(fightArea);
        areaName.text = fightArea.areaName;
    }

    private void DisplayRewards(FightAreaData fightArea)
    {
        if (fightArea.reward != null)
        {
            switch (fightArea.reward.rewardType)
            {
                case RewardData.RewardType.Candy:
                    rewardText.text = $"Candies: {fightArea.reward.nbOfCandy}";
                    break;
                case RewardData.RewardType.Money:
                    rewardText.text = $"Money: {fightArea.reward.money}$";
                    break;
                default:
                    rewardText.text = "Unknown reward.";
                    break;
            }
        }
        else
        {
            rewardText.text = "No rewards available.";
        }
    }

    
    public void ClearHighlight()
    {
        _instanceMaterial.SetColor(OutlineColor, Color.clear);
        _instanceMaterial.SetColor("_Color", Color.white); // Met à jour la teinte
    }
    
    public void SetHighlight(Color color)
    {
        _instanceMaterial.SetColor(OutlineColor, color); // Met à jour la couleur de l'outline
        _instanceMaterial.SetColor("_Color", color); // Met à jour la teinte
    }
}
