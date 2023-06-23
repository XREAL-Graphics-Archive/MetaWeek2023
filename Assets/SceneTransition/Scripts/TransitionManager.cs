using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    #region Singleton

    private static TransitionManager _instance;

    public static TransitionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #endregion
    
    [SerializeField] private GameObject globalMask;
    [SerializeField] private SceneAsset sceneToLoad;
    
    private GameObject selectedBall;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(globalMask != null)
            DontDestroyOnLoad(globalMask);
    }

    public void SelectSphere(GameObject sphere)
    {
        selectedBall = sphere;
    }

    public void LoadSceneAdditive()
    {
        
    }
}
