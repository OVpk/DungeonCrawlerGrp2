using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class CheatConsoleToggle : MonoBehaviour
{
    public GameObject cheatConsoleCanvas;
    public TMP_InputField inputField;
    private bool isConsoleOpen = false;

    private void Start()
    {
        cheatConsoleCanvas.SetActive(false);
        inputField.onEndEdit.AddListener(OnCommandEntered);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }
    }

    void ToggleConsole()
    {
        isConsoleOpen = !isConsoleOpen;
        cheatConsoleCanvas.SetActive(isConsoleOpen);

        if (isConsoleOpen)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void OnCommandEntered(string input)
    {
        if (!isConsoleOpen) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ExecuteCommand(input);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void ExecuteCommand(string input)
    {
        input = input.Trim().ToLower();
        
        switch (input)
        {
            case "test":
                Debug.Log("test");
                break;
            
            // Commandes pour Changer de salle :
            
            case "scenenext":
                // Passer à la salle suivante
                Debug.Log("Salle Suivante");
                break;
            
            case "sceneprevious":
                // Passer à la salle précédente
                Debug.Log("Salle Précédente");
                break;
            
            case "scene1":
                // Passer à la salle 1
                break;
            
            case "scene2a":
                // Passer à la salle 2A
                break;
            
            case "scene2b":
                // Passer à la salle 2B
                break;
            
            case "scene3a":
                // Passer à la salle 3A
                break;
            
            case "scene3b":
                // Passer à la salle 3B
                break;
            
            case "scene4a":
                // Passer à la salle 4A
                break;
            
            case "scene4b":
                // Passer à la salle 4B
                break;
            
            case "scene5a":
                // Passer à la salle 5A
                break;
            
            case "scene5b":
                // Passer à la salle 5B
                break;
            
            case "scene6a":
                // Passer à la salle 6A
                break;
            
            case "scene6b":
                // Passer à la salle 6B
                break;
            
            case "scene7a":
                // Passer à la salle 7A
                break;
            
            case "scene7b":
                // Passer à la salle 7B
                break;
            
            case "scene8a":
                // Passer à la salle 8A
                break;
            
            case "scene8b":
                // Passer à la salle 8B
                break;
            
            case "scene9a":
                // Passer à la salle 9A
                break;
            
            case "scene9b":
                // Passer à la salle 9B
                break;
            
            case "scene10a":
                // Passer à la salle 10A
                break;
            
            case "scene10b":
                // Passer à la salle 10B
                break;
            
            case "scene11a":
                // Passer à la salle 11A
                break;
            
            case "scene11b":
                // Passer à la salle 11B
                break;
            
            case "scene12a":
                // Passer à la salle 12A
                break;
            
            case "scene12b":
                // Passer à la salle 12B
                break;
            
            case "scene13a":
                // Passer à la salle 13A
                break;
            
            case "scene13b":
                // Passer à la salle 13B
                break;
            
            case "scene14a":
                // Passer à la salle 14A
                break;
            
            case "scene14b":
                // Passer à la salle 14B
                break;
            
            // Commandes pour ajouter des bonbons ou de l'argent :
            
            case "givetagada":
                // Ajouter 5 tagada
                break;
            
            case "givecarambar":
                // Ajouter 5 carambar
                break;
            
            case "givemalabar":
                // Ajouter 5 malabar
                break;
            
            case "givemoney":
                // Ajouter 50 argent
                break;
            
            // Commandes pour éliminer des entités :
            
            case "killenemy":
                // Tuer tous les ennemis
                break;
            
            case "killally":
                // Tuer tous les alliés
                break;
            
            case "killall":
                // Tuer toutes les entités
                break;
            
            // Commandes pour débloquer les paquets :
            
            case "unlockcarambar":
                // Débloque le paquet de carambar
                break;
            
            case "unlockmalabar":
                // Débloque le paquet de malabar
                break;
                
            // Commandes inconnues :
            
            default:
                Debug.Log("Commande inconnue : " + input);
                break;
        }
    }
}