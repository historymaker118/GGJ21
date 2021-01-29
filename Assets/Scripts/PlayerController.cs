using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseMoveSpeed = 0.5f;
    public float baseHeight = 0.5f;
    public float baseOffset = 0.0f;

    public float legsMoveSpeed = 2.0f;

    public float legsHeight = 1.0f;
    public float legsOffset = 0.25f;

    public bool hasLegs;
    public bool hasArms;
    public bool hasJump;

    public Rigidbody2D rigid;
    public CapsuleCollider2D capsule;

    public GameObject legs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var moveSpeed = baseMoveSpeed;
        if (hasLegs)
        {
            moveSpeed = legsMoveSpeed;
        }

        capsule.size = new Vector2(
            capsule.size.x,
            (hasLegs) ? legsHeight : baseHeight
        );

        capsule.offset = new Vector2(
            capsule.offset.x,
            (hasLegs) ? legsOffset : baseOffset
        );

        var horiz = Input.GetAxis("Horizontal");

        var vel = rigid.velocity;
        vel.x = horiz * moveSpeed;
        rigid.velocity = vel;
    }
}