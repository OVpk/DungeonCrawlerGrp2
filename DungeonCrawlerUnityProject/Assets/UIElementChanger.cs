using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIElementChanger : MonoBehaviour
{
    [Header("Références UI")]    
    [SerializeField] private Image targetImage;
    [SerializeField] private TMP_Text textField1;
    [SerializeField] private TMP_Text textField2;
    [SerializeField] private TMP_Text textField3;
    [SerializeField] private TMP_Text textField4;

    public void ChangeElements(Sprite newSprite, string t1, string t2, string t3, string t4)
    {
        if (targetImage != null)
        {
            targetImage.sprite = newSprite;
        }
        if (textField1 != null) textField1.text = t1;
        if (textField2 != null) textField2.text = t2;
        if (textField3 != null) textField3.text = t3;
        if (textField4 != null) textField4.text = t4;
    }
}