using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TransitionManager : MonoBehaviour
{
    #region CAMERA EVENTS

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

    #region SCENEMANAGER EVENTS

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        sceneLoads -= 1;
        Debug.Log("[TransitionManager] Loaded " + scene.name);
    }

    void OnSceneUnload(Scene scene)
    {
        sceneLoads -= 1;
        Debug.Log("[TransitionManager] Unloaded " + scene.name);
    }

    #endregion

    [Header("XR Rig Data")] [SerializeField]
    private GameObject playerRig;
    [SerializeField] private Camera playerCam;
    private UniversalAdditionalCameraData portalRenderer;
    private int currentRendererIndex = 0;

    [Space] [Header("Transition Settings")] 
    [SerializeField] private PortalBall.SceneConfiguration currentSceneType;
    [SerializeField] private MeshRenderer globalMask;
    [SerializeField] private SceneField lobbyScene;
    [SerializeField] private float transitionDuration = 1f;
    private SceneField currentScene;
    private SceneField sceneToLoad;
    private PortalBall selectedBall;
    private float timeElapsed = 0f;
    private int sceneLoads;
    private List<GameObject> stencilMasks;

    public static event Action sceneTransition;

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

    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering += OnEndCameraRendering;
        SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.sceneUnloaded += OnSceneUnload;
        sceneTransition += OnSceneTransition;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering -= OnEndCameraRendering;
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.sceneUnloaded -= OnSceneUnload;
        sceneTransition -= OnSceneTransition;
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedBall = FindObjectOfType<PortalBall>();
            SelectBall(selectedBall);
        }
    }

    // scene transition event
    void OnSceneTransition()
    {
        // disable any other stencil masks
        if (currentSceneType == PortalBall.SceneConfiguration.Lobby)
        {
            var balls = FindObjectsOfType<PortalBall>();
            foreach(PortalBall ball in balls)
                if(ball != selectedBall)
                    ball.gameObject.SetActive(false);
        }

        // update scene type
        currentSceneType = selectedBall.SceneType;
        
        // change skybox
        RenderSettings.skybox = selectedBall.Skybox;
        DynamicGI.UpdateEnvironment();
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
        {
            sceneLoads += 1;
            SceneManager.LoadSceneAsync(sceneToLoad.Name, LoadSceneMode.Additive);
        }
    }

    bool UnloadSceneAdditive(SceneField scene)
    {
        // disable all objects to prevent rendering glitches
        Scene sceneToUnload = SceneManager.GetSceneByName(scene);

        if (!sceneToUnload.IsValid()) return false;
        if (!sceneToUnload.isLoaded) return false;

        foreach (GameObject rootObject in sceneToUnload.GetRootGameObjects())
        {
            rootObject.SetActive(false);
        }

        // unload scene
        sceneLoads += 1;
        SceneManager.UnloadSceneAsync(scene);
        return true;
    }

    IEnumerator SwitchScene()
    {
        sceneLoads += 1;
        timeElapsed = 0;
        Vector3 startPos = selectedBall.transform.position;

        // lerp portal ball position
        while (timeElapsed < transitionDuration)
        {
            selectedBall.transform.position = Vector3.Lerp(
                startPos,
                playerCam.transform.position,
                timeElapsed / transitionDuration
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // set portal ball position to player cam after lerp
        selectedBall.transform.position = playerCam.transform.position;

        // unload current scene and switch renderer
        while (UnloadSceneAdditive(currentScene)) yield return null;
        SwitchRenderer();

        // switch global mask
        globalMask.sharedMaterial = selectedBall.GlobalMask;

        if (sceneToLoad != null && sceneToLoad.Name.Length != 0)
            currentScene = sceneToLoad;
        sceneLoads -= 1;
        yield return 0;
    }

    /// <summary>
    /// Set currently selected portal ball.
    /// </summary>
    /// <param name="ball"></param>
    public void SelectBall(PortalBall ball)
    {
        if (sceneLoads > 0) return;
        selectedBall = ball;
        sceneToLoad = ball.Scene;
        // Debug.Log($"{currentScene.Name} {ball.Scene.Name}");
    }

    /// <summary>
    /// Invoke transition after selecting ball. Must be called after SelectBall()
    /// </summary>
    public void InvokeTransition()
    {
        if (sceneLoads > 0) return;
        if (selectedBall == null)
        {
            Debug.LogWarning("There is no Portal Ball selected. Cannot invoke transition.");
            return;
        }

        sceneTransition?.Invoke();
        
        SwitchRenderer();
        LoadSceneAdditive();
        StartCoroutine(SwitchScene());
    }
}