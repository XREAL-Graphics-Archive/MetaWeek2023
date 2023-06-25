using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation : MonoBehaviour
{

    Animator animator;

    [SerializeField] float speed = 1f;
    private float _speed;

    [SerializeField] float weight = 1f;
    private float _weight;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = speed;
        animator.SetLayerWeight(1, weight);
        _speed = speed;
        _weight = weight;
    }

    void Update()
    {
        if (_speed != speed)
        {
            animator.speed = speed;
            _speed = speed;
        }

        if(_weight != weight)
        {
            animator.SetLayerWeight(1, weight);
            _weight = weight;
        }
    }
}
