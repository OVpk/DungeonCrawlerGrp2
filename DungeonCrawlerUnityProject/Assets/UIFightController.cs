using UnityEngine;
using DG.Tweening;

public class TransformSwitcherSquishy : MonoBehaviour
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
    public KeyCode cToggleKey = KeyCode.C;

    // Positions d'origine
    private Vector3 posA;
    private Vector3 posB;
    private Vector3 posC;
    // États
    private bool isAActive = true;
    private bool isAnimatingAB = false;
    private bool isCVisible;
    private bool isAnimatingC = false;

    void Start()
    {
        posA = objA.localPosition;
        posB = objB.localPosition;
        posC = objC.transform.localPosition;

        // Initialisation des états
        objA.gameObject.SetActive(true);
        objB.gameObject.SetActive(false);
        // objC suit objA: actif seulement si A actif
        objC.SetActive(true);
        isCVisible = objC.activeSelf;

        objA.localScale = Vector3.one;
        objB.localScale = Vector3.one;
    }

    /// <summary>
    /// Alterne A/B avec squash & stretch vertical
    /// </summary>
    public void SwitchAB()
    {
        if (isAnimatingAB) return;
        isAnimatingAB = true;

        Transform inObj  = isAActive ? objB : objA;
        Transform outObj = isAActive ? objA : objB;
        Vector3    inPos  = isAActive ? posB : posA;
        Vector3    outPos = isAActive ? posA : posB;

        inObj.gameObject.SetActive(true);
        inObj.localPosition = inPos - Vector3.up * verticalOffset;
        inObj.localScale = Vector3.one;

        // Si on entre avec A, active objC
        if (inObj == objA)
            objC.SetActive(true);

        Sequence seqIn = DOTween.Sequence();
        seqIn.Append(inObj.DOLocalMove(inPos, animationDuration).SetEase(entryEaseAB));
        seqIn.Join(inObj.DOScale(new Vector3(1.3f, 0.7f, 1f), animationDuration * 0.3f).SetEase(Ease.OutQuad));
        seqIn.Append(inObj.DOScale(new Vector3(0.7f, 1.3f, 1f), animationDuration * 0.3f).SetEase(Ease.OutQuad));
        seqIn.Append(inObj.DOScale(Vector3.one, animationDuration * 0.4f).SetEase(Ease.OutQuad));
        seqIn.OnComplete(() => { isAnimatingAB = false; });

        Sequence seqOut = DOTween.Sequence();
        seqOut.Append(outObj.DOLocalMove(outPos - Vector3.up * verticalOffset, animationDuration).SetEase(exitEaseAB));
        seqOut.Join(outObj.DOScale(new Vector3(0.7f, 1.3f, 1f), animationDuration * 0.3f).SetEase(Ease.InQuad));
        seqOut.Append(outObj.DOScale(new Vector3(1.3f, 0.7f, 1f), animationDuration * 0.3f).SetEase(Ease.InQuad));
        seqOut.AppendCallback(() => {
            outObj.gameObject.SetActive(false);
            outObj.localScale = Vector3.one;

            // Si on sort avec A, désactive objC
            if (outObj == objA)
                objC.SetActive(false);
        });

        isAActive = !isAActive;
    }
    
    public void ToggleC()
    {
        // Si B actif, force C désactivé et bloque
        if (!isAActive)
        {
            if (objC.activeSelf)
                objC.SetActive(false);
            isCVisible = false;
            return;
        }

        if (isAnimatingC) return;
        isAnimatingC = true;

        if (!isCVisible)
        {
            // Entrée depuis la droite
            objC.SetActive(true);
            objC.transform.localPosition = posC + Vector3.right * horizontalOffset;
            objC.transform.DOLocalMoveX(posC.x, animationDuration).SetEase(entryEaseC)
                .OnComplete(() => { isAnimatingC = false; isCVisible = true; });
        }
        else
        {
            // Sortie vers la droite hors écran
            objC.transform.DOLocalMoveX(posC.x + horizontalOffset, animationDuration).SetEase(exitEaseC)
                .OnComplete(() => {
                    objC.SetActive(false);
                    isAnimatingC = false;
                    isCVisible = false;
                });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SwitchAB();

        if (Input.GetKeyDown(cToggleKey))
            ToggleC();
    }
}
