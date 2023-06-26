using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

public class TransitionManager : MonoBehaviour
{
    #region CAMERA EVENTS
    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering -= OnEndCameraRendering;
    }
    
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        if (playerCam != cam) return;
        
        // TODO SWITCH SCENE LIGHTING SETUP
    }
    #endregion

    [Header("XR Rig Data")]
    [SerializeField] private GameObject playerRig;
    [SerializeField] private Camera playerCam;
    private UniversalAdditionalCameraData portalRenderer;
    private int currentRendererIndex = 0;
    [Space]
    
    [Header("Transition Settings")]
    [SerializeField] private MeshRenderer globalMask;
    [SerializeField] private SceneField lobbyScene;
    [SerializeField] private float transitionDuration = 1f;
    private SceneField currentScene;
    private SceneField sceneToLoad;
    private PortalBall selectedBall;
    private float timeElapsed = 0f;

    public bool IsReady { get; private set; } = true;

    private static TransitionManager _instance;

    public static TransitionManager Instance { get; private set; }

    private void Awake()
    {
        // singleton instantiation
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        // disable player rig before initial setup
        playerRig.SetActive(false);

        // load lobby scene
        currentScene = lobbyScene;
        SceneManager.LoadSceneAsync(lobbyScene.Name, LoadSceneMode.Additive);
        
        // enable rig after loading lobby scene
        playerRig.SetActive(true);
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // cache graphics data
        portalRenderer = playerCam.transform.GetComponent<UniversalAdditionalCameraData>();

        // (REMOVE AFTER SETUP) test portal ball
        selectedBall = FindObjectOfType<PortalBall>();
        SelectBall(selectedBall);
    }

    private void Update()
    {
        // (REMOVE AFTER SETUP) convenient method to test transition
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InvokeTransition();
        }
    }

    void SwitchRenderer()
    {
        currentRendererIndex = (++currentRendererIndex) % 2;
        portalRenderer.SetRenderer(currentRendererIndex);
    }
    
    void LoadSceneAdditive()
    {
        // only load additive if player cam renderer is set to portal renderer
        if (currentRendererIndex == 1)
            SceneManager.LoadSceneAsync(sceneToLoad.Name, LoadSceneMode.Additive);
    }

    void UnloadSceneAdditive(SceneField scene)
    {
        // disable all objects to prevent rendering glitches
        Scene sceneToUnload = SceneManager.GetSceneByName(scene);

        if (!sceneToUnload.IsValid()) return;
        if (!sceneToUnload.isLoaded) return;

        foreach (GameObject rootObject in sceneToUnload.GetRootGameObjects())
        {
            rootObject.SetActive(false);
        }
        
        // unload scene
        SceneManager.UnloadSceneAsync(scene);
    }

    IEnumerator SwitchScene()
    {
        IsReady = false;
        timeElapsed = 0;
        Vector3 startPos = selectedBall.transform.position;
        
        // lerp portal ball position
        while (timeElapsed < transitionDuration)
        {
            selectedBall.transform.position = Vector3.Lerp(
                startPos, 
                playerCam.transform.position, 
                timeElapsed/transitionDuration
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // set portal ball position to player cam after lerp
        selectedBall.transform.position = playerCam.transform.position;
        
        // unload current scene and switch renderer
        UnloadSceneAdditive(currentScene);
        SwitchRenderer();
        
        // switch global mask
        globalMask.sharedMaterial = selectedBall.GlobalMask;
        IsReady = true;
    } 
    
    /// <summary>
    /// Set currently selected portal ball.
    /// </summary>
    /// <param name="sphere"></param>
    public void SelectBall(PortalBall sphere)
    {
        selectedBall = sphere;
        sceneToLoad = sphere.Scene;
    }

    /// <summary>
    /// Invoke transition after selecting ball. Must be called after SelectBall()
    /// </summary>
    public void InvokeTransition()
    {
        if (selectedBall == null)
        {
            Debug.LogWarning("There is no Portal Ball selected. Cannot invoke transition.");
            return;
        }
        
        SwitchRenderer();
        LoadSceneAdditive();
        StartCoroutine(SwitchScene());
    }
}
