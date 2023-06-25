using UnityEngine;

[System.Serializable]
public class SceneField
{
    [SerializeField] private Object _sceneAsset;
    [SerializeField] private string _sceneName;

    public string Name => _sceneName;

    public static implicit operator string(SceneField obj)
    {
        return obj.Name;
    }
}
