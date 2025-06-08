using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    
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
    
    public enum MainMenuOptions
    {
        Jouer,
        Credits,
        Quitter
    }

    public MainMenuOptions currentOption = MainMenuOptions.Jouer;

    public Image jouer;
    public Image credits;
    public Image quitter;
    public Sprite jouerUnselected;
    public Sprite jouerSelected;
    public Sprite creditsUnselected;
    public Sprite creditsSelected;
    public Sprite quitterUnselected;
    public Sprite quitterSelected;

    public void MoveSelector(int direction) // 1 ou -1
    {
        if (direction == 0) return;
        currentOption = (MainMenuOptions)(((int)currentOption + direction + 3) % 3);
        UpdateSelectorDisplay();
    }

    public void ApplyOption()
    {
        switch (currentOption)
        {
            case MainMenuOptions.Jouer :
                GameManager.Instance.ChangeGameState(GameManager.GameState.InFightArea);
                break;
            case MainMenuOptions.Credits :
                // afficher crédits
                break;
            case MainMenuOptions.Quitter :
                Application.Quit();
                break;
        }
    }

    public void UpdateSelectorDisplay()
    {
        switch (currentOption)
        {
            case MainMenuOptions.Jouer :
                jouer.sprite = jouerSelected;
                credits.sprite = creditsUnselected;
                quitter.sprite = quitterUnselected;
                break;
            case MainMenuOptions.Credits :
                jouer.sprite = jouerUnselected;
                credits.sprite = creditsSelected;
                quitter.sprite = quitterUnselected;
                break;
            case MainMenuOptions.Quitter :
                jouer.sprite = jouerUnselected;
                credits.sprite = creditsUnselected;
                quitter.sprite = quitterSelected;
                break;
        }
    }
}
