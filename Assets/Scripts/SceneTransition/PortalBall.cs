using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PortalBall : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;
    public SceneField Scene => sceneToLoad;
}