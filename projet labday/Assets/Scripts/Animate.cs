using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    Animator PlayerAnimator;

    void Awake() {
        PlayerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveAmount = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && moveAmount > 0)
        {
            moveAmount *= 2; 
        }

        PlayerAnimator.SetFloat("walk", moveAmount);
    }
}