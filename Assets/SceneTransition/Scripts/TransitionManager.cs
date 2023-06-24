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
    #region SINGLETON

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
    private int rendererListLength = 2;
    private int currentRendererIndex = 0;
    [Space]
    
    [Header("Transition Settings")]
    [SerializeField] private SceneField lobbyScene;
    [SerializeField] private SceneField sceneToLoad;
    [SerializeField] private float transitionDuration = 1f;
    private SceneField currentScene;
    private float timeElapsed = 0f;
    
    private Light[] mainLights = new Light[2];
    private PortalBall selectedBall;

    void Start()
    {
        // disable player rig before initial setup
        playerRig.SetActive(false);
        
        DontDestroyOnLoad(gameObject);

        // load lobby scene
        currentScene = lobbyScene;
        SceneManager.LoadSceneAsync(lobbyScene.Name, LoadSceneMode.Additive);
        
        // enable rig after loading lobby scene
        playerRig.SetActive(true);

        // mainLights[0] = RenderSettings.sun;
        
        // cache graphics data
        portalRenderer = playerCam.transform.GetComponent<UniversalAdditionalCameraData>();

        // (REMOVE AFTER SETUP) test portal ball
        selectedBall = FindObjectOfType<PortalBall>();
        SelectBall(selectedBall);
    }

    // set currently selected portal ball
    public void SelectBall(PortalBall sphere)
    {
        selectedBall = sphere;
        sceneToLoad = sphere.Scene;
    }

    // Get all lights from main & additive scene
    void FetchLights()
    {
        // get main lights
        Light[] sceneLights = FindObjectsOfType<Light>();
        foreach (Light light in sceneLights)
        {
            if (light.type == LightType.Directional && light != RenderSettings.sun)
            {
                mainLights[1] = light;
                break;
            }
        }
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
        selectedBall.gameObject.SetActive(false);
    } 

    public void InvokeTransition()
    {
        if (selectedBall == null)
        {
            Debug.LogWarning("There is no Portal Ball selected. Cannot invoke transition");
            return;
        }
        
        SwitchRenderer();
        LoadSceneAdditive();
        StartCoroutine(SwitchScene());
    }
}
