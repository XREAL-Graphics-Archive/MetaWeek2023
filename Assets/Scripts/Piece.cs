using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Piece : MonoBehaviour
{
    public UnityEvent onSelect;
    public UnityEvent onDeselect;

    public PortalBall portal;

    public MeshRenderer[] toPlayRenderers;

    private readonly List<Material> toPlayMaterials = new();
    private static int playProperty = Shader.PropertyToID("_Play");

    private void Start()
    {
        for (var i = 0; i < toPlayRenderers.Length; i++)
        {
            var materials = toPlayRenderers[i].sharedMaterials;
            for (var j = 0; j < materials.Length; j++) toPlayMaterials.Add(materials[j]);
        }
    }

    public void PlayMaterials()
    {
        for (var i = 0; i < toPlayMaterials.Count; i++) toPlayMaterials[i].SetInt(playProperty, 1);
    }

    public void StopMaterials()
    {
        for (var i = 0; i < toPlayMaterials.Count; i++) toPlayMaterials[i].SetInt(playProperty, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (portal == null) return;
        TransitionManager.Instance.SelectBall(portal);
        TransitionManager.Instance.InvokeTransition();
    }
}