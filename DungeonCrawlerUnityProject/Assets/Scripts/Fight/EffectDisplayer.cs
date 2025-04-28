using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplayer : MonoBehaviour
{
    public EntityDisplayer controller;
    public Animation anim;
    
    public void CanContinueEvent()
    {
        controller.CanContinueEvent();
    }
}
