using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;
    
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
