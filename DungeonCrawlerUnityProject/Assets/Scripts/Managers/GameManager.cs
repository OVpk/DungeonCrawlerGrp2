using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private OverWorldController overWorldController;
    [SerializeField] private AreaController areaController;
    [SerializeField] private FightAreaController fightAreaController;
    
    public enum ControllerTypes
    {
        InOverWorld,
        InArea,
        InFightArea
    }

    private ControllerTypes currentControllerType;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        ChangeController(ControllerTypes.InFightArea);
    }

    public void ChangeController(ControllerTypes newControllerType)
    {
        currentControllerType = newControllerType;
        switch (currentControllerType)
        {
            case ControllerTypes.InOverWorld :
                overWorldController.ChangeActiveState(true);
                areaController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(false);
                break;
            case ControllerTypes.InArea :
                overWorldController.ChangeActiveState(false);
                areaController.ChangeActiveState(true);
                fightAreaController.ChangeActiveState(false);
                break;
            case ControllerTypes.InFightArea :
                overWorldController.ChangeActiveState(false);
                areaController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(true);
                break;
        }
    }

}
