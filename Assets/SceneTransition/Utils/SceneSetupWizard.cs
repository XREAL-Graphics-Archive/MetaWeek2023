using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetupWizard : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private SceneAsset oldSceneAsset;
    private SceneAsset selectedSceneAsset;
    private bool showLights;
    private bool showGameObjects;
    private Vector2 lightsScrollPosition;
    private Vector2 objectsScrollPosition;

    private List<GameObject> lightList = new List<GameObject>();
    private List<GameObject> objectsToRender = new List<GameObject>();

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += OnSceneChange;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnSceneChange;
    }

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Scene Setup Wizard")]
    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        SceneSetupWizard window = GetWindow<SceneSetupWizard>();
        window.minSize = new Vector2(625f, 700f);
        window.maxSize = new Vector2(800f, 1200f);
    }

    void OnGUI()
    {
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
        }

        if (GUILayout.Button("Configure as Room", GUILayout.Height(20f)))
        {
            Debug.Log("Configured Scene as Room");
        }
        EditorGUILayout.EndHorizontal();
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
