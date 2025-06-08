using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance;
    
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

    public enum PauseMenuOptions
    {
        Reprendre,
        MainMenu,
        Quitter
    }

    public PauseMenuOptions currentOption = PauseMenuOptions.Reprendre;

    public Image reprendre;
    public Image mainMenu;
    public Image quitter;
    public Sprite reprendreUnselected;
    public Sprite reprendreSelected;
    public Sprite mainMenuUnselected;
    public Sprite mainMenuSelected;
    public Sprite quitterUnselected;
    public Sprite quitterSelected;

    public void MoveSelector(int direction) // 1 ou -1
    {
        if (direction == 0) return;
        currentOption = (PauseMenuOptions)(((int)currentOption + direction + 3) % 3);
        UpdateSelectorDisplay();
    }

    public void ApplyOption()
    {
        switch (currentOption)
        {
            case PauseMenuOptions.Reprendre :
                GameManager.Instance.ChangeGameState(GameManager.GameState.InFightArea);
                break;
            case PauseMenuOptions.MainMenu :
                GameManager.Instance.ChangeGameState(GameManager.GameState.InMainMenu);
                break;
            case PauseMenuOptions.Quitter :
                Application.Quit();
                break;
        }
    }

    public void UpdateSelectorDisplay()
    {
        switch (currentOption)
        {
            case PauseMenuOptions.Reprendre :
                reprendre.sprite = reprendreSelected;
                mainMenu.sprite = mainMenuUnselected;
                quitter.sprite = quitterUnselected;
                break;
            case PauseMenuOptions.MainMenu :
                reprendre.sprite = reprendreUnselected;
                mainMenu.sprite = mainMenuSelected;
                quitter.sprite = quitterUnselected;
                break;
            case PauseMenuOptions.Quitter :
                reprendre.sprite = reprendreUnselected;
                mainMenu.sprite = mainMenuUnselected;
                quitter.sprite = quitterSelected;
                break;
        }
    }
    
    
}
