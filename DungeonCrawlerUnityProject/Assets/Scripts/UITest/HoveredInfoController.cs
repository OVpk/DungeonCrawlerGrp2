using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoveredInfoController : MonoBehaviour
{
    public TMP_Text characterName;
    public TMP_Text currentLayerInfo;
    public TMP_Text nextLayerInfo;
    public Image characterSprite;
    public TMP_Text description;

    public void UpdateInformations(CharacterDataInstance character)
    {
        if (character == null) {UpdateInformationWithEmpty(); return;}
        
        characterName.text = character.name;
        string currentType = character.type == EntityData.EntityTypes.Mou ? "<sprite index=3>" : "<sprite index=0>";
        currentLayerInfo.text = character.durability + "<sprite index=4> / " + currentType;
        if (character.nextLayer != null)
        {
            string nextType = character.nextLayer.type == EntityData.EntityTypes.Mou ? "<sprite index=3>" : "<sprite index=0>";
            nextLayerInfo.text = character.nextLayer.durability + "<sprite index=4> / " + nextType;
        }
        else
        {
            nextLayerInfo.text = "MORT";
        }
        
        characterSprite.gameObject.SetActive(true);
        characterSprite.sprite = character.descriptionVisual;
        description.text = character.description;
    }

    public void UpdateInformationWithEmpty()
    {
        characterName.text = "VIDE";
        currentLayerInfo.text = "???";
        nextLayerInfo.text = "???";
        characterSprite.gameObject.SetActive(false);
        description.text = "EMPLACEMENT VIDE";
    }
}
