using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ExplorationController overWorldController;
    [SerializeField] private FightAreaController fightAreaController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private RefillPackController refillPackController;
    [SerializeField] private EncyclopedieController encyclopedieController;
    [SerializeField] private PauseMenuController pauseMenuController;

    private GameObject fightScene => FightManager.Instance.transform.root.gameObject;
    private GameObject explorationScene => ExplorationManager.Instance.transform.root.gameObject;
    private GameObject shopScene => ShopManager.Instance.transform.root.gameObject;

    private GameObject refillPackScene => RefillPackManager.Instance.transform.root.gameObject;

    private GameObject encyclopedieScene => EncyclopedieManager.Instance.transform.root.gameObject;

    private GameObject pauseMenuScene => PauseMenuManager.Instance.transform.root.gameObject;
    
    public enum GameState
    {
        InOverWorld,
        InFightArea,
        InShop,
        InRefillPack,
        InEncyclopedie,
        InMainMenu,
        InPauseMenu
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
                encyclopedieScene.SetActive(false);
                pauseMenuScene.SetActive(false);
                break;
            case GameState.InFightArea :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
                shopScene.SetActive(false);
                refillPackScene.SetActive(false);
                encyclopedieScene.SetActive(false);
                pauseMenuScene.SetActive(false);
                break;
            case GameState.InShop :
                explorationScene.SetActive(false);
                fightScene.SetActive(false);
                shopScene.SetActive(true);
                refillPackScene.SetActive(false);
                encyclopedieScene.SetActive(false);
                pauseMenuScene.SetActive(false);
                break;
            case GameState.InRefillPack :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
                shopScene.SetActive(false);
                refillPackScene.SetActive(true);
                encyclopedieScene.SetActive(false);
                pauseMenuScene.SetActive(false);
                break;
            case GameState.InEncyclopedie :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
                shopScene.SetActive(false);
                refillPackScene.SetActive(false);
                encyclopedieScene.SetActive(true);
                pauseMenuScene.SetActive(true);
                EncyclopedieManager.Instance.UpdateDescriptionSprite();
                break;
            case GameState.InPauseMenu :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
                shopScene.SetActive(false);
                refillPackScene.SetActive(false);
                encyclopedieScene.SetActive(false);
                pauseMenuScene.SetActive(true);
                PauseMenuManager.Instance.currentOption = PauseMenuManager.PauseMenuOptions.Reprendre;
                PauseMenuManager.Instance.UpdateSelectorDisplay();
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
                encyclopedieController.ChangeActiveState(false);
                pauseMenuController.ChangeActiveState(false);
                break;
            case GameState.InFightArea :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(true);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(false);
                encyclopedieController.ChangeActiveState(false);
                pauseMenuController.ChangeActiveState(false);
                break;
            case GameState.InShop :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(true);
                refillPackController.ChangeActiveState(false);
                encyclopedieController.ChangeActiveState(false);
                pauseMenuController.ChangeActiveState(false);
                break;
            case GameState.InRefillPack :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(true);
                encyclopedieController.ChangeActiveState(false);
                pauseMenuController.ChangeActiveState(false);
                break;
            case GameState.InEncyclopedie :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(false);
                encyclopedieController.ChangeActiveState(true);
                pauseMenuController.ChangeActiveState(false);
                break;
            case GameState.InPauseMenu :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                shopController.ChangeActiveState(false);
                refillPackController.ChangeActiveState(false);
                encyclopedieController.ChangeActiveState(false);
                pauseMenuController.ChangeActiveState(true);
                break;
        }
    }

}
