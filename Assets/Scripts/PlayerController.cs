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
    public float jumpForce = 2.0f;

    public bool hasLegs;
    public bool hasArms;
    public bool hasJump;

    public Rigidbody2D rigid;
    public CapsuleCollider2D capsule;

    public GameObject legsView;
    public GameObject jumpView;

    public float maxSlopeAngle;

    [SerializeField]
    private bool isGrounded;

    private Vector2 groundNormal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO WT: set these on hasLegs change.
        legsView.SetActive(hasLegs);
        jumpView.SetActive(hasJump);

        // TODO WT: Wall Grab and climbing
        // TODO WT: Pick up and put down of legs, arms and jump.

        capsule.size = new Vector2(
            capsule.size.x,
            (hasLegs) ? legsHeight : baseHeight
        );

        capsule.offset = new Vector2(
            capsule.offset.x,
            (hasLegs) ? legsOffset : baseOffset
        );
    }

    private void FixedUpdate()
    {
        var castDistance = 0.02f; // Should ideally be casting the next position after gravity is applied
        var hitInfo = Physics2D.CapsuleCast((Vector2)transform.position + capsule.offset, capsule.size, capsule.direction, 0.0f, Vector2.down, castDistance, ~(1 << 8));
        Debug.DrawRay(transform.position, Vector3.down * castDistance, Color.blue);

        if (hitInfo)
        {
            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.green);

            isGrounded = Vector2.Angle(Vector2.up, hitInfo.normal) <= maxSlopeAngle;

            groundNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
        }

        var moveSpeed = (hasLegs) ? legsMoveSpeed : baseMoveSpeed;

        var horiz = Input.GetAxis("Horizontal");
        if (isGrounded)
        {
            var dir = -new Vector2(-groundNormal.y, groundNormal.x);

            //var vel = rigid.velocity;
            //vel.x = horiz * moveSpeed;
            //rigid.velocity = vel;
            rigid.velocity = dir * horiz * moveSpeed;
        }
        else
        {
            horiz *= 0.5f;

            var vel = rigid.velocity;
            vel.x += horiz * moveSpeed * Time.fixedDeltaTime;
            rigid.velocity = vel;
        }

        if (hasJump)
        {
            if (isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    print("Jump");
                    rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isGrounded = false;
                }
            }
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    var hasGroundContact = isGrounded;
    //    foreach (var contact in collision.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal);
    //        if (Vector2.Angle(Vector2.up, contact.normal) < maxSlopeAngle)
    //        {
    //            hasGroundContact = true;
    //        }
    //    }

    //    isGrounded = hasGroundContact;
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    print("Collision exit");
    //    OnCollisionEnter2D(collision);
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    isGrounded = false;
    //}
}