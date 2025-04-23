using System.Collections;
using UnityEngine;

public class EntityDisplayer : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer baseRenderer;
    [SerializeField] private Animator animator;

    private Material _instanceMaterial;

    private static readonly int UseGrayscale = Shader.PropertyToID("_UseGrayscale");
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineSize = Shader.PropertyToID("_OutlineSize");

    private static readonly int TriggerIdle = Animator.StringToHash("Idle");
    private static readonly int TriggerSpawn = Animator.StringToHash("Spawn");
    private static readonly int TriggerAttack = Animator.StringToHash("Attack");
    private static readonly int TriggerHit = Animator.StringToHash("Hit");
    private static readonly int TriggerSleep = Animator.StringToHash("Sleep");

    private void Awake()
    {
        // Clone material instance to avoid global changes
        _instanceMaterial = new Material(baseRenderer.sharedMaterial);
        baseRenderer.material = _instanceMaterial;
        ClearHighlight();
        _instanceMaterial.SetFloat(OutlineSize, 6f);
        gameObject.SetActive(false);
    }
    
    public void InitVisual(EntityDataInstance data)
    {
        baseRenderer.sprite = data.sprite;
        animator.runtimeAnimatorController = data.animator;
    }
    

    public void SetGrayscale(bool active)
    {
        _instanceMaterial.SetFloat(UseGrayscale, active ? 1f : 0f);
    }

    public void SetHighlight(Color color)
    {
        _instanceMaterial.SetColor(OutlineColor, color);
    }

    public void ClearHighlight()
    {
        _instanceMaterial.SetColor(OutlineColor, Color.clear);
    }

    public void PlayIdleAnim() => animator.SetTrigger(TriggerIdle);
    public void PlaySleepAnim() => animator.SetTrigger(TriggerSleep);
    public void PlaySpawnAnim() => animator.SetTrigger(TriggerSpawn);
    public void PlayAttackAnim() => animator.SetTrigger(TriggerAttack);
    public void PlayHitAnim() => animator.SetTrigger(TriggerHit);
    
    
    private bool canContinue = false;
    
    public void CanContinueEvent()
    {
        canContinue = true;
    }
    
    public IEnumerator WaitForAnimation()
    {
        canContinue = false;
        yield return new WaitUntil(() => canContinue);
    }
    
}