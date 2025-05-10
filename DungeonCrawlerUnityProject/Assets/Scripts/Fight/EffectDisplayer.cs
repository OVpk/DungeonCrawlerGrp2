using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplayer : MonoBehaviour
{
    public Animation anim;
    public SpriteRenderer explosivePowder;

    public Animator glueAnim;
    
    private static readonly int Glue = Animator.StringToHash("Glue");

    public void PlayGlueAnim() => glueAnim.SetTrigger(Glue);
}
