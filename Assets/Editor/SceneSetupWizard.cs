using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SceneSetupWizard : EditorWindow
{
    private SceneAsset selectedSceneAsset;
    
    private List<GameObject> lightList = new List<GameObject>();
    private List<GameObject> objectsToRender = new List<GameObject>();
    private Vector2 lightsScrollPosition;
    private Vector2 objectsScrollPosition;
    private Vector2 windowScrollPosition;

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += OnSceneChange;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnSceneChange;
    }
    
    [MenuItem("Window/Scene Setup Wizard")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        SceneSetupWizard window = GetWindow<SceneSetupWizard>();
        window.minSize = new Vector2(625f, 640f);
        window.maxSize = new Vector2(625f, 640f);
    }
    
    void OnGUI()
    {
        windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);
        
        if (GUILayout.Button("Fetch Scene Data", GUILayout.Height(20f)))
        {
            GetCurrentScene();
            GetLights();
            GetObjectsToRender();
        }
        
        GUILayout.Space(10f);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Scene Asset", EditorStyles.boldLabel, EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField(selectedSceneAsset, typeof(SceneAsset), false);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);
        
        EditorGUILayout.BeginHorizontal();
        
        // Lights List
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Lights", EditorStyles.boldLabel);
        ShowLights();
        EditorGUILayout.EndVertical();

        // Objects to Render List
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Objects to Render", EditorStyles.boldLabel);
        ShowObjectsToRender();
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10f);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Configure as Lobby", GUILayout.Height(20f)))
        {
            Debug.Log("Configured Scene as Lobby");
            ConfigureLobby();
        }

        if (GUILayout.Button("Configure as Room", GUILayout.Height(20f)))
        {
            Debug.Log("Configured Scene as Room");
            ConfigureRoom();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndScrollView();
    }

    private void ConfigureLobby()
    {
        foreach (GameObject obj in lightList)
        {
            UniversalAdditionalLightData light = obj.GetComponent<Light>().GetUniversalAdditionalLightData();
            light.renderingLayers = 1 << 8;
        }
        
        foreach (GameObject obj in objectsToRender)
        {
            obj.layer = 6;
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            mr.renderingLayerMask = 1 << 8;
        }

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private void ConfigureRoom()
    {
        foreach (GameObject obj in lightList)
        {
            UniversalAdditionalLightData light = obj.GetComponent<Light>().GetUniversalAdditionalLightData();
            light.renderingLayers = 1 << 9;
        }
        
        foreach (GameObject obj in objectsToRender)
        {
            obj.layer = 7;
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            mr.renderingLayerMask = 1 << 9;
        }
        
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private void OnSceneChange(Scene scene, OpenSceneMode mode)
    {
        lightList.Clear();
        objectsToRender.Clear();
        GetCurrentScene();
        GetLights();
        GetObjectsToRender();
    }

    private void GetCurrentScene()
    {
        selectedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
    }

    private void GetLights()
    {
        lightList.Clear();
        foreach (Light light in FindObjectsOfType<Light>())
        {
            lightList.Add(light.gameObject);
        }
    }

    private void ShowLights()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        lightsScrollPosition = EditorGUILayout.BeginScrollView(
            lightsScrollPosition, 
            GUILayout.Width(300f),
            GUILayout.Height(500f));

        foreach (GameObject light in lightList)
        {
            EditorGUILayout.LabelField(light.gameObject.name);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void GetObjectsToRender()
    {
        objectsToRender.Clear();
        foreach (MeshRenderer mr in FindObjectsOfType<MeshRenderer>())
        {
            objectsToRender.Add(mr.gameObject);
        }
    }
    
    private void ShowObjectsToRender()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        objectsScrollPosition = EditorGUILayout.BeginScrollView(
            objectsScrollPosition,
            GUILayout.Width(300f),
            GUILayout.Height(500f));

        foreach (GameObject obj in objectsToRender)
        {
            EditorGUILayout.LabelField(obj.gameObject.name);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
