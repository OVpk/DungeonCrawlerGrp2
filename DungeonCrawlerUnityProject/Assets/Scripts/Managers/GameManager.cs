using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ExplorationController overWorldController;
    [SerializeField] private FightAreaController fightAreaController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private RefillPackController refillPackController;

    private GameObject fightScene => FightManager.Instance.transform.root.gameObject;
    private GameObject explorationScene => ExplorationManager.Instance.transform.root.gameObject;
    private GameObject shopScene => ShopManager.Instance.transform.root.gameObject;

    private GameObject refillPackScene => RefillPackManager.Instance.transform.root.gameObject;
    
    public enum GameState
    {
        InOverWorld,
        InFightArea,
        InShop,
        InRefillPack
    }

    private GameState currentGameState;

    public List<CandyPack> candyPacks = new List<CandyPack>();

    public CandyPackData[] firstPacks;

    public int money;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        foreach (var pack in firstPacks)
        {
            candyPacks.Add(new CandyPack(pack));
        }
    }

    private void Start()
    {
        ChangeGameState(GameState.InFightArea);
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        ChangeDisplayedScene(currentGameState);
        ChangeController(currentGameState);
    }

    private void ChangeDisplayedScene(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.InOverWorld :
                explorationScene.SetActive(true);
                fightScene.SetActive(false);
                shopScene.SetActive(false);
                refillPackScene.SetActive(false);
                break;
            case GameState.InFightArea :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
                shopScene.SetActive(false);
                refillPackScene.SetActive(false);
                break;
            case GameState.InShop :
                explorationScene.SetActive(false);
                fightScene.SetActive(false);
                shopScene.SetActive(true);
                refillPackScene.SetActive(false);
                break;
            case GameState.InRefillPack :
                explorationScene.SetActive(false);
                fightScene.SetActive(false);
                shopScene.SetActive(false);
                refillPackScene.SetActive(true);
                break;
        }
    }

    private void ChangeController(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.InOverWorld :
                overWorldController.ChangeActiveState(true);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(false);
                break;
            case GameState.InFightArea :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(true);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(false);
                break;
            case GameState.InShop :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(true);
                refillPackController.ChangeActiveState(false);
                break;
            case GameState.InRefillPack :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(true);
                break;
        }
    }

}
