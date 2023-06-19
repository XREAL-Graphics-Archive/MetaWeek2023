using UnityEngine;

public class Pieces : MonoBehaviour
{
    public VRDragHandler dragHandler;
    public GameObject[] pieces;
    public float radius;
    public float height;
    public float pieceScale;

    private void Start()
    {
        var interval = 360 / pieces.Length;
        for (var i = 0; i < pieces.Length; i++)
        {
            pieces[i].transform.localScale = new Vector3(pieceScale, pieceScale, pieceScale);
            var pos = Quaternion.Euler(0, interval * i % 360, 0) * new Vector3(radius, height, 0);
            pieces[i].transform.position = pos;
        }

        dragHandler.onDrag += OnDrag;
    }

    private void OnDrag(float value)
    {
        var interval = 360 / pieces.Length;
        for (var i = 0; i < pieces.Length; i++)
        {
            var pos = Quaternion.Euler(0, (value + interval * i) % 360, 0) * new Vector3(radius, height, 0);
            pieces[i].transform.position = pos;
        }
    }
}