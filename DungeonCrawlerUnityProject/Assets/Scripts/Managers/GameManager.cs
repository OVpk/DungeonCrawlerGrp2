using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private ExplorationController overWorldController;
    [SerializeField] private FightAreaController fightAreaController;

    private GameObject fightScene => FightManager.Instance.transform.root.gameObject;
    private GameObject explorationScene => ExplorationManager.Instance.transform.root.gameObject;
    
    public enum GameState
    {
        InOverWorld,
        InFightArea
    }

    private GameState currentGameState;

    public CandyPackDataInstance[] candyPacks;

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
    }

    private void Start()
    {
        ChangeController(GameState.InOverWorld);
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
                break;
            case GameState.InFightArea :
                explorationScene.SetActive(false);
                fightScene.SetActive(true);
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
                break;
            case GameState.InFightArea :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(true);
                break;
        }
    }

}
