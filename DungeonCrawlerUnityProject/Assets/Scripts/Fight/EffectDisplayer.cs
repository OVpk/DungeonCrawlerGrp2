using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplayer : MonoBehaviour
{
    public Animation anim;
    public Animator explosivePowder;

    public Animator glueAnim;
    
    private static readonly int Glue = Animator.StringToHash("Glue");

    public void PlayGlueAnim() => glueAnim.SetTrigger(Glue);

    public Animator fogAnim;
    private static readonly int Fog = Animator.StringToHash("Fog");
    
    public void PlayFogAnim() => fogAnim.SetTrigger(Fog);
}
