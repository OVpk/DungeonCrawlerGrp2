using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHelpController : MonoBehaviour
{
    [SerializeField] private GameObject OnPlayerGridHelp;
    [SerializeField] private GameObject SelectAttackHelp;
    [SerializeField] private GameObject SelectAttackPositionHelp;
    [SerializeField] private GameObject SelectPackHelp;
    [SerializeField] private GameObject PlaceCharacterHelp;

    public void DisplayHelp(FightAreaController.SelectorState state)
    {
        switch (state)
        {
            case FightAreaController.SelectorState.OnPlayerGrid : 
                OnPlayerGridHelp.SetActive(true);
                SelectAttackHelp.SetActive(false);
                SelectAttackPositionHelp.SetActive(false);
                SelectPackHelp.SetActive(false);
                PlaceCharacterHelp.SetActive(false); break;
            case FightAreaController.SelectorState.SelectAttack : 
                OnPlayerGridHelp.SetActive(false);
                SelectAttackHelp.SetActive(true);
                SelectAttackPositionHelp.SetActive(false);
                SelectPackHelp.SetActive(false);
                PlaceCharacterHelp.SetActive(false); break;
            case FightAreaController.SelectorState.SelectAttackPosition : 
                OnPlayerGridHelp.SetActive(false);
                SelectAttackHelp.SetActive(false);
                SelectAttackPositionHelp.SetActive(true);
                SelectPackHelp.SetActive(false);
                PlaceCharacterHelp.SetActive(false); break;
            case FightAreaController.SelectorState.SelectPack : 
                OnPlayerGridHelp.SetActive(false);
                SelectAttackHelp.SetActive(false);
                SelectAttackPositionHelp.SetActive(false);
                SelectPackHelp.SetActive(true);
                PlaceCharacterHelp.SetActive(false); break;
            case FightAreaController.SelectorState.PlaceCharacter : 
                OnPlayerGridHelp.SetActive(false);
                SelectAttackHelp.SetActive(false);
                SelectAttackPositionHelp.SetActive(false);
                SelectPackHelp.SetActive(false);
                PlaceCharacterHelp.SetActive(true); break;
        }
    }
}
