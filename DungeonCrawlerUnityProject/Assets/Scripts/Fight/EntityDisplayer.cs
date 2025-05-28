using System;
using System.Collections;
using System.Collections.Generic;
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

    private static readonly int TriggerSpawn = Animator.StringToHash("Spawn");
    private static readonly int TriggerAttack = Animator.StringToHash("Attack");
    private static readonly int TriggerHit = Animator.StringToHash("Hit");
    private static readonly int TriggerSleep = Animator.StringToHash("Sleep");
    private static readonly int TriggerDeath = Animator.StringToHash("Death");

    private void Awake()
    {
        // Clone material instance to avoid global changes
        _instanceMaterial = new Material(baseRenderer.sharedMaterial);
        baseRenderer.material = _instanceMaterial;
        ClearHighlight();
        _instanceMaterial.SetFloat(OutlineSize, 6f);
        Register(FightManager.Instance);
        gameObject.SetActive(false);
    }

    public void InitVisual(EntityDataInstance data)
    {
        baseRenderer.sprite = data.sprite;
        animator.runtimeAnimatorController = data.animator;
    }

    public void SetHighlight(Color color)
    {
        _instanceMaterial.SetColor(OutlineColor, color); // Met à jour la couleur de l'outline
        _instanceMaterial.SetColor("_Color", color); // Met à jour la teinte
    }


    public void ClearHighlight()
    {
        _instanceMaterial.SetColor(OutlineColor, Color.clear);
        _instanceMaterial.SetColor("_Color", Color.white); // Met à jour la teinte
    }

    public void PlaySleepAnim() => animator.SetTrigger(TriggerSleep);
    public void PlaySpawnAnim() => animator.SetTrigger(TriggerSpawn);
    public void PlayAttackAnim() => animator.SetTrigger(TriggerAttack);
    public void PlayHitAnim() => animator.SetTrigger(TriggerHit);

    public void PlayDeathAnim() => animator.SetTrigger(TriggerDeath);
    
    public void CanContinueEvent()
    {
        SayInformationCanContinue();
    }

    public void SetSleepingState(bool state)
    {
        animator.SetBool(IsSleeping, state);
    }
    
    private List<IFightDisplayerListener> listeners = new List<IFightDisplayerListener>();
    private static readonly int IsSleeping = Animator.StringToHash("IsSleeping");

    public void Register(IFightDisplayerListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void Unregister(IFightDisplayerListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    private void SayInformationCanContinue()
    {
        foreach (var listener in listeners)
        {
            listener.OnDisplayerSaidCanContinue();
        }
    }
    
}