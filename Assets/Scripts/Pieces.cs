using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pieces : MonoBehaviour
{
    public static int UIHovering { get; set; }

    public VRDragAngleHandler dragAngleHandler;
    public VRInputHandler inputHandler;
    public Piece[] pieces;
    public float radius;
    public float selectRadius;
    public float height;
    public float selectHeight;
    public float pieceScale;
    public float selectPieceScale;
    public float initialRotation;

    private int selection;

    private void Start()
    {
        selection = PlayerPrefs.GetInt("selection");

        var interval = 360 / pieces.Length;
        var baseRot = initialRotation - interval * selection;
        for (var i = 0; i < pieces.Length; i++)
        {
            if (i == selection) pieces[i].onSelect?.Invoke();
            else pieces[i].onDeselect?.Invoke();

            var piece = pieces[i].transform;
            piece.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
            var yRot = Quaternion.Euler(0, (baseRot + interval * i) % 360, 0);

            var pos = yRot * new Vector3(i == selection ? selectRadius : radius, i == selection ? selectHeight : height,
                0);
            piece.position = pos;
            piece.rotation = yRot;
            var scale = i == selection ? selectPieceScale : pieceScale;
            piece.localScale = new Vector3(scale, scale, scale);
        }

        dragAngleHandler.onDragAngle += OnDragAngle;
    }

    private void OnDragAngle(Vector2 value)
    {
        if (!dragAngleHandler.swipeMode || UIHovering > 0) return;
        var prev = selection;
        selection = (selection - (int)value.y + pieces.Length) % pieces.Length;
        StartCoroutine(RotateCoroutine(prev, selection));
    }

    private IEnumerator RotateCoroutine(int prev, int next)
    {
        var time = Time.time;
        var interval = 360f / pieces.Length;
        var prevAngle = -interval * prev;
        var nextAngle = -interval * next;
        var first = true;
        var elapsed = 0f;
        var threshold = dragAngleHandler.swipeCooldown * 0.9f;
        while (elapsed <= threshold)
        {
            yield return null;
            elapsed = Time.time - time;
            var progress = elapsed / threshold;
            var cur = initialRotation + Mathf.LerpAngle(prevAngle, nextAngle, Math.Min(1, progress));
            for (var i = 0; i < pieces.Length; i++)
            {
                float x, y, s;
                if (i == prev)
                {
                    if (first) pieces[i].onDeselect.Invoke();
                    x = Mathf.Lerp(selectRadius, radius, progress);
                    y = Mathf.Lerp(selectHeight, height, progress);
                    s = Mathf.Lerp(selectPieceScale, pieceScale, progress);
                }
                else if (i == next)
                {
                    if (elapsed > threshold) pieces[i].onSelect.Invoke();
                    x = Mathf.Lerp(radius, selectRadius, progress);
                    y = Mathf.Lerp(height, selectHeight, progress);
                    s = Mathf.Lerp(pieceScale, selectPieceScale, progress);
                }
                else
                {
                    x = radius;
                    y = height;
                    s = pieceScale;
                }

                var piece = pieces[i].transform;
                var yRot = Quaternion.Euler(0, (cur + interval * i) % 360, 0);
                var pos = yRot * new Vector3(x, y, 0);
                piece.position = pos;
                piece.rotation = yRot;
                piece.localScale = new Vector3(s, s, s);
            }

            first = false;
        }
    }

    private void OnApplicationQuit()
    {
        selection = 0;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("selection", selection);
    }
}