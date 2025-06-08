using UnityEngine;
using DG.Tweening;

public class DOTweenAutoAnimator2D : MonoBehaviour
{
    [Header("Flottement Vertical")]
    public bool floatUpDown;
    public float floatStrength = 0.5f;
    public float floatSpeed = 1f;

    [Header("Effet de Vent")]
    public bool windEffect;
    public float windAngleStrength = 15f;
    public float windSpeed = 1f;
    public bool windToRight = true;

    [Header("Squishy Bounce")]
    public bool squishyBounce;
    public float squishStrength = 0.2f;
    public float squishSpeed = 2f;

    [Header("Rotation Continue")]
    public bool rotateEffect;
    public float rotateSpeed = 1f;
    public bool rotateClockwise = true;

    [Header("Déplacement Circulaire Continu")]
    public bool circleMove;
    public float circleRadius = 0.5f;
    public float circleSpeed = 1f;
    public bool circleClockwise = true;

    [Header("Vibration de Position")]
    public bool positionVibration;
    public float positionVibrationStrength = 0.1f;
    public float positionVibrationSpeed = 20f;

    [Header("Tremblement de Rotation")]
    public bool rotationShake;
    public float rotationShakeStrength = 5f;
    public float rotationShakeSpeed = 20f;

    [Header("Tremblement de Scale")]
    public bool scaleShake;
    public float scaleShakeStrength = 0.1f;
    public float scaleShakeSpeed = 20f;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Transform windContainer;

    void Start()
    {
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
        initialRotation = transform.localRotation;

        if (windEffect)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            float spriteHeight = sr.sprite.bounds.size.y * transform.localScale.y;
            Vector3 bottomOffset = new Vector3(0, -spriteHeight / 2f, 0);
            
            GameObject container = new GameObject("WindContainer");
            container.transform.SetParent(transform.parent);
            container.transform.localRotation = transform.localRotation;
            container.transform.localPosition = initialPosition + bottomOffset;
            
            transform.SetParent(container.transform);
            transform.localPosition = -bottomOffset;

            windContainer = container.transform;
            StartWindAnimation();
        }

        if (floatUpDown) StartFloatAnimation();
        if (squishyBounce) StartSquishyAnimation();
        if (rotateEffect) StartRotateAnimation();
        if (circleMove) StartCircleAnimation();
        if (positionVibration) StartPositionVibration();
        if (rotationShake) StartRotationShake();
        if (scaleShake) StartScaleShake();
    }

    void StartFloatAnimation()
    {
        transform.DOLocalMoveY(initialPosition.y + floatStrength, floatSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StartWindAnimation()
    {
        float direction = windToRight ? 1f : -1f;
        windContainer.DOLocalRotate(new Vector3(0, 0, direction * windAngleStrength), windSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StartSquishyAnimation()
    {
        transform.DOScale(new Vector3(initialScale.x + squishStrength, initialScale.y - squishStrength, initialScale.z), squishSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StartRotateAnimation()
    {
        float direction = rotateClockwise ? -1f : 1f;
        transform.DORotate(new Vector3(0, 0, 360f * direction), rotateSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    void StartCircleAnimation()
    {
        float angle = 0f;
        float direction = circleClockwise ? -1f : 1f;
        DOTween.To(() => angle, x =>
        {
            angle = x;
            float rad = Mathf.Deg2Rad * angle;
            transform.localPosition = initialPosition + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * circleRadius;
        }, 360f, circleSpeed)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    void StartPositionVibration()
    {
        transform.DOShakePosition(1f, positionVibrationStrength, (int)positionVibrationSpeed, 90, false, true)
            .SetLoops(-1, LoopType.Restart);
    }

    void StartRotationShake()
    {
        transform.DOShakeRotation(1f, new Vector3(0, 0, rotationShakeStrength), (int)rotationShakeSpeed, 90)
            .SetLoops(-1, LoopType.Restart);
    }

    void StartScaleShake()
    {
        transform.DOShakeScale(1f, new Vector3(scaleShakeStrength, scaleShakeStrength, 0), (int)scaleShakeSpeed, 90)
            .SetLoops(-1, LoopType.Restart);
    }
    
    void OnDestroy()
    {
        // Arrête toutes les animations DOTween associées à ce GameObject
        transform.DOKill();

        // Si un container de vent a été créé, arrête également ses animations
        if (windContainer != null)
        {
            windContainer.DOKill();
        }
    }
}
