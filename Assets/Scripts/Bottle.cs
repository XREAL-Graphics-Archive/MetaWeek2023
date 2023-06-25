using System;
using Oculus.Interaction;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public new Collider collider;

    public float respawnY;
    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    private int selected;

    private void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (transform.position.y >= respawnY) return;
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        rigidbody.velocity = Vector3.zero;
    }
}