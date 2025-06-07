using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDisplayer : MonoBehaviour
{
    public List<Sprite> numbers;

    public SpriteRenderer dizaines;
    public SpriteRenderer unites;

    public SpriteRenderer logoMoney;
    public SpriteRenderer logoCandy;

    public void DisplayRewards(bool state, int nb, RewardData.RewardType rewardType)
    {
        gameObject.SetActive(state);
        if (!state) return;
        
        if (nb >= 10)
        {
            int dizaine = nb / 10;
            dizaines.sprite = numbers[dizaine];
        }
        else
        {
            dizaines.sprite = null;
        }
        
        int unite = nb % 10;
        unites.sprite = numbers[unite];

        switch (rewardType)
        {
            case RewardData.RewardType.Money:
                logoMoney.gameObject.SetActive(true);
                logoCandy.gameObject.SetActive(false);
                break;
            case RewardData.RewardType.Candy:
                logoMoney.gameObject.SetActive(false);
                logoCandy.gameObject.SetActive(true);
                break;
        }
    }
}
