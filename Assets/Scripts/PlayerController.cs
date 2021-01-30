using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Current movement settings")]
    public float currentMoveSpeed;
    public float currentHeight;
    public float currentOffset;
    public bool hasJump;

    [Header("Base movement settings")]
    public float baseMoveSpeed = 0.5f;
    public float baseHeight = 0.5f;
    public float baseOffset = 0.0f;

    public float maxSlopeAngle;

    public GameObject bodyView;

    [Header("Legs movement settings")]
    public float legsMoveSpeed = 2.0f;
    public float legsHeight = 1.0f;
    public float legsOffset = 0.25f;
    public GameObject legsView;

    [Header("Jump movement settings")]
    public float jumpForce = 2.0f;
    public GameObject jumpView;

    [Header("Components")]
    public Rigidbody2D rigid;
    public CapsuleCollider2D capsule;

    public MovementPart partA;
    public MovementPart partB;

    [SerializeField]
    private bool isGrounded;

    private Vector2 groundNormal;

    private bool _partsDirty;

    // Start is called before the first frame update
    void Start()
    {
        processParts();
    }

    private void OnValidate()
    {
        _partsDirty = true;
    }

    private void swapPart(MovementPart part, bool isA)
    {
        if (isA)
        {
            partA = part;
        } else
        {
            partB = part;
        }

        processParts();
    }

    private void processParts()
    {
        currentMoveSpeed = baseMoveSpeed;
        currentHeight = baseHeight;
        currentOffset = baseHeight;
        hasJump = false;

        legsView.SetActive(false);
        jumpView.SetActive(false);

        processPart(partA);
        processPart(partB);

        capsule.size = new Vector2(capsule.size.x, currentHeight);
        capsule.offset = new Vector2(capsule.offset.x, currentOffset);

        _partsDirty = false;
    }

    private void processPart(MovementPart part)
    {
        switch (part)
        {
            case MovementPart.Legs:
                currentMoveSpeed = legsMoveSpeed;
                currentHeight = legsHeight;
                currentOffset = legsOffset;
                legsView.SetActive(true);
                break;
            case MovementPart.Arms:
                break;
            case MovementPart.Jump:
                hasJump = true;
                jumpView.SetActive(hasJump);
                break;
            case MovementPart.NONE:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_partsDirty)
        {
            processParts();
        }

        var sign = Mathf.Sign(Input.GetAxis("Horizontal"));

        legsView.transform.localScale = new Vector3(sign, 1.0f);
        jumpView.transform.localScale = new Vector3(sign, 1.0f);
        bodyView.transform.localScale = new Vector3(sign, 1.0f);
        // TODO WT: Wall Grab and climbing
        // TODO WT: Pick up and put down of legs, arms and jump.
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

        var horiz = Input.GetAxis("Horizontal");
        if (isGrounded)
        {
            var dir = -new Vector2(-groundNormal.y, groundNormal.x);

            rigid.velocity = dir * horiz * currentMoveSpeed;
        }
        else
        {
            horiz *= 0.5f;

            var vel = rigid.velocity;
            vel.x += horiz * currentMoveSpeed * Time.fixedDeltaTime;
            rigid.velocity = vel;
        }

        if (hasJump)
        {
            if (isGrounded)
            {
                if (Input.GetButton("Jump"))
                {
                    rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isGrounded = false;
                }
            }
        }
    }
}