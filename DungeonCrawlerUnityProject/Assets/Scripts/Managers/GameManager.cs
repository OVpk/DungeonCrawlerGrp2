using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private OverWorldController overWorldController;
    [SerializeField] private AreaController areaController;
    
    public enum ControllerTypes
    {
        InOverWorld,
        InArea
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
        ChangeController(ControllerTypes.InOverWorld);
    }

    public void ChangeController(ControllerTypes newControllerType)
    {
        currentControllerType = newControllerType;
        switch (currentControllerType)
        {
            case ControllerTypes.InOverWorld :
                overWorldController.ChangeActiveState(true);
                areaController.ChangeActiveState(false);
                break;
            case ControllerTypes.InArea :
                overWorldController.ChangeActiveState(false);
                areaController.ChangeActiveState(true);
                break;
        }
    }

}
