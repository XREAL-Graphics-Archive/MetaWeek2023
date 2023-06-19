using System;
using System.Collections;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public VRDragHandler dragHandler;
    public GameObject[] pieces;
    public float radius;
    public float height;
    public float initialRotation;
    public float pieceScale;

    private int selection;

    private void Start()
    {
        var interval = 360 / pieces.Length;
        for (var i = 0; i < pieces.Length; i++)
        {
            pieces[i].transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
            var pos = Quaternion.Euler(0, (initialRotation + interval * i) % 360, 0) * new Vector3(radius, height, 0);
            pieces[i].transform.position = pos;
        }

        dragHandler.onDrag += OnDrag;
    }

    private void OnDrag(float value)
    {
        if (!dragHandler.swipeMode) return;
        var prev = selection;
        selection = (selection + (int)value + pieces.Length) % pieces.Length;
        StartCoroutine(RotateCoroutine(prev, selection));
    }

    private IEnumerator RotateCoroutine(int prev, int next)
    {
        var time = Time.time;
        var interval = 360f / pieces.Length;
        var prevAngle = interval * prev;
        var nextAngle = interval * next;
        while (Time.time - time <= dragHandler.swipeCooldown * 0.9f)
        {
            yield return null;
            var cur = initialRotation + Mathf.LerpAngle(prevAngle, nextAngle,
                Math.Min(1, (Time.time - time) / (dragHandler.swipeCooldown * 0.9f)));
            for (var i = 0; i < pieces.Length; i++)
            {
                var pos = Quaternion.Euler(0, (cur + interval * i) % 360, 0) * new Vector3(radius, height, 0);
                pieces[i].transform.position = pos;
            }
        }
    }
}