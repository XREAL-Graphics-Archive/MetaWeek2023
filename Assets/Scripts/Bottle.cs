using UnityEngine;

public class Bottle : MonoBehaviour
{
    public new Rigidbody rigidbody;

    public float respawnY;
    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    private int selected;

    private void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (transform.position.y >= respawnY) return;
        rigidbody.Move(respawnPosition, respawnRotation);
        rigidbody.velocity = Vector3.zero;
    }
}