using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidWobble : MonoBehaviour
{
    public Renderer rend;

    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountTaAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        wobbleAmountTaAddX = Mathf.Lerp(wobbleAmountTaAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountTaAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
        
        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        wobbleAmountTaAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountTaAddX += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;

    }

}
