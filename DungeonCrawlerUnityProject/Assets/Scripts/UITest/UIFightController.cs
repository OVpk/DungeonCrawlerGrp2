// TransformSwitcherSquishy.cs
using UnityEngine;
using DG.Tweening;

public class UIFightController : MonoBehaviour
{
    [Header("Transforms à switcher")]
    public Transform objA;
    public Transform objB;

    [Header("Object C suiveur")]
    public GameObject objC;

    [Header("Paramètres d'animation A/B")]
    public float animationDuration = 0.6f;
    public float verticalOffset = 5f;
    public Ease entryEaseAB = Ease.OutBack;
    public Ease exitEaseAB = Ease.InBack;

    [Header("Paramètres d'animation C")]
    public float horizontalOffset = 5f;
    public Ease entryEaseC = Ease.OutQuad;
    public Ease exitEaseC = Ease.InQuad;

    // Positions d'origine
    private Vector3 posA;
    private Vector3 posB;
    private Vector3 posC;

    void Start()
    {
        // Sauvegarde des positions locales initiales
        posA = objA.localPosition;
        posB = objB.localPosition;
        posC = objC.transform.localPosition;

        // États initiaux
        objA.gameObject.SetActive(true);
        objB.gameObject.SetActive(false);

        objA.localScale = Vector3.one;
        objB.localScale = Vector3.one;
    }

    /// <summary>
    /// Alterne A/B avec squash & stretch vertical
    /// </summary>
    private int switchCounter = 0;

    public void SwitchAB(bool isForB)
    {
        switchCounter++;
        int currentSwitch = switchCounter;

        Transform inObj  = isForB ? objB : objA;
        Transform outObj = isForB ? objA : objB;
        Vector3    inPos  = isForB ? posB : posA;
        Vector3    outPos = isForB ? posA : posB;

        DOTween.Kill(inObj);
        DOTween.Kill(outObj);

        inObj.gameObject.SetActive(true);
        inObj.localScale = Vector3.one;
        outObj.localScale = Vector3.one;
        inObj.localPosition = inPos - Vector3.up * verticalOffset;

        Sequence seqIn = DOTween.Sequence();
        seqIn.Append(inObj.DOLocalMove(inPos, animationDuration).SetEase(entryEaseAB));
        seqIn.Join(inObj.DOScale(new Vector3(1.3f, 0.7f, 1f), animationDuration * 0.3f).SetEase(Ease.OutQuad));
        seqIn.Append(inObj.DOScale(new Vector3(0.7f, 1.3f, 1f), animationDuration * 0.3f).SetEase(Ease.OutQuad));
        seqIn.Append(inObj.DOScale(Vector3.one, animationDuration * 0.4f).SetEase(Ease.OutQuad));

        Sequence seqOut = DOTween.Sequence();
        seqOut.Append(outObj.DOLocalMove(outPos - Vector3.up * verticalOffset, animationDuration).SetEase(exitEaseAB));
        seqOut.Join(outObj.DOScale(new Vector3(0.7f, 1.3f, 1f), animationDuration * 0.3f).SetEase(Ease.InQuad));
        seqOut.Append(outObj.DOScale(new Vector3(1.3f, 0.7f, 1f), animationDuration * 0.3f).SetEase(Ease.InQuad));
        seqOut.AppendCallback(() => {
            if (currentSwitch != switchCounter) return;
            outObj.gameObject.SetActive(false);
            outObj.localScale = Vector3.one;
        });
    }
    
    /// <summary>
    /// Affiche ou cache C sans dépendance à A/B
    /// </summary>
    
    private int toggleCCounter = 0;
    
    public void ToggleC(bool state)
    {
        toggleCCounter++;
        int currentToggle = toggleCCounter;

        DOTween.Kill(objC.transform); // kill existing tweens

        if (state)
        {
            objC.SetActive(true);
            objC.transform.localPosition = posC + Vector3.right * horizontalOffset;
            objC.transform.DOLocalMoveX(posC.x, animationDuration)
                .SetEase(entryEaseC);
        }
        else
        {
            objC.transform.DOLocalMoveX(posC.x + horizontalOffset, animationDuration)
                .SetEase(exitEaseC)
                .OnComplete(() => {
                    if (currentToggle != toggleCCounter) return; // only latest
                    objC.SetActive(false);
                });
        }
    }
}
