using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class PortalBall : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneToLoad;
    public SceneAsset Scene => sceneToLoad;
}