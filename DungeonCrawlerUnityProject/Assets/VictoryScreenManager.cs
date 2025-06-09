using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreenManager : MonoBehaviour
{
    public static VictoryScreenManager Instance;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
