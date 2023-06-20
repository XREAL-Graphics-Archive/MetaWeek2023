using System;
using System.Collections;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public VRDragHandler dragHandler;
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
        var interval = 360 / pieces.Length;
        pieces[0].onSelect?.Invoke();
        for (var i = 0; i < pieces.Length; i++)
        {
            var piece = pieces[i].transform;
            piece.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
            var pos = Quaternion.Euler(0, (initialRotation + interval * i) % 360, 0) *
                      new Vector3(i == 0 ? selectRadius : radius, i == 0 ? selectHeight : height, 0);
            piece.position = pos;
            var scale = i == 0 ? selectPieceScale : pieceScale;
            piece.localScale = new Vector3(scale, scale, scale);
        }

        dragHandler.onDrag += OnDrag;
    }

    private void OnDrag(float value)
    {
        if (!dragHandler.swipeMode) return;
        var prev = selection;
        selection = (selection - (int)value + pieces.Length) % pieces.Length;
        StartCoroutine(RotateCoroutine(prev, selection));
    }

    private IEnumerator RotateCoroutine(int prev, int next)
    {
        var time = Time.time;
        var interval = 360f / pieces.Length;
        var prevAngle = -interval * prev;
        var nextAngle = -interval * next;
        while (Time.time - time <= dragHandler.swipeCooldown * 0.9f)
        {
            yield return null;
            var progress = (Time.time - time) / (dragHandler.swipeCooldown * 0.9f);
            var cur = initialRotation + Mathf.LerpAngle(prevAngle, nextAngle, Math.Min(1, progress));
            for (var i = 0; i < pieces.Length; i++)
            {
                float x, y, s;
                if (i == prev)
                {
                    pieces[i].onDeselect.Invoke();
                    x = Mathf.Lerp(selectRadius, radius, progress);
                    y = Mathf.Lerp(selectHeight, height, progress);
                    s = Mathf.Lerp(selectPieceScale, pieceScale, progress);
                }
                else if (i == next)
                {
                    pieces[i].onSelect.Invoke();
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

                var pos = Quaternion.Euler(0, (cur + interval * i) % 360, 0) * new Vector3(x, y, 0);
                pieces[i].transform.position = pos;
                pieces[i].transform.localScale = new Vector3(s, s, s);
            }
        }
    }
}