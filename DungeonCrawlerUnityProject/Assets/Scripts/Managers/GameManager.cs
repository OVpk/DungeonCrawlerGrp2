using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private OverWorldController overWorldController;
    [SerializeField] private FightAreaController fightAreaController;
    
    public enum ControllerTypes
    {
        InOverWorld,
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
        }
    }

    private void Start()
    {
        ChangeController(ControllerTypes.InOverWorld);
    }
    
    

    public void ChangeController(ControllerTypes newControllerType)
    {
        currentControllerType = newControllerType;
        switch (currentControllerType)
        {
            case ControllerTypes.InOverWorld :
                overWorldController.ChangeActiveState(true);
                fightAreaController.ChangeActiveState(false);
                break;
            case ControllerTypes.InFightArea :
                overWorldController.ChangeActiveState(false);
                fightAreaController.ChangeActiveState(true);
                break;
        }
    }

}
