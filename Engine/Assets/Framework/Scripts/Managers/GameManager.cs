using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager s_instance;

    public static GameManager GetInstance()
    {
        return s_instance;
    }

    void Awake()
    {
        s_instance = this;
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
