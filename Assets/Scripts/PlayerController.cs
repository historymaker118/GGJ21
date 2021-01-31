using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Current movement settings")]
    private float currentMoveSpeed;
    private float currentHeight;
    private float currentOffset;
    private float currentJumpForce;
    private bool isClimbEnabled;

    [Header("Base movement settings")]
    public float baseMoveSpeed = 0.5f;
    public float baseHeight = 0.5f;
    public float baseOffset = 0.0f;
    public float baseBodyHeight = 0.5f;
    public float baseJumpForce = 3.0f;
    public GameObject bodyView;

    public float maxSlopeAngle;

    [Header("Legs movement settings")]
    public float legsMoveSpeed = 2.0f;
    public float legsHeight = 1.0f;
    public float legsOffset = 0.25f;
    public float legsBodyHeight = 1.5f;
    public GameObject legsView;

    [Header("Wall movement settings")]
    public GameObject armsView;

    [Header("Jump movement settings")]
    public float jumpForce = 2.0f;
    public float extraGravityScale = 2.0f;
    public GameObject jumpView;

    [Header("Components")]
    public Rigidbody2D rigid;
    new public BoxCollider2D collider;

    public MovementPart partA;
    public MovementPart partB;

    [SerializeField]
    private bool isGrounded;

    [SerializeField]
    private bool canWallClimb;

    private bool wallOnLeft;
    private bool wallOnRight;

    private Vector2 groundNormal;

    private int isQPressed;
    private int isEPressed;

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
        currentOffset = baseOffset;
        currentJumpForce = baseJumpForce;
        isClimbEnabled = false;

        var pos = bodyView.transform.localPosition;
        pos.y = baseBodyHeight;
        bodyView.transform.localPosition = pos;

        legsView.SetActive(false);
        jumpView.SetActive(false);
        armsView.SetActive(false);

        processPart(partA);
        processPart(partB);

        collider.size = new Vector2(collider.size.x, currentHeight);
        collider.offset = new Vector2(collider.offset.x, currentOffset);

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

                var pos = bodyView.transform.localPosition;
                pos.y = legsBodyHeight;
                bodyView.transform.localPosition = pos;

                legsView.SetActive(true);
                break;
            case MovementPart.Arms:
                isClimbEnabled = true;
                armsView.SetActive(true);
                break;
            case MovementPart.Jump:
                currentJumpForce = jumpForce;
                jumpView.SetActive(true);
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

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            var sign = Mathf.Sign(Input.GetAxis("Horizontal"));

            legsView.transform.localScale = new Vector3(sign, 1.0f);
            jumpView.transform.localScale = new Vector3(sign, 1.0f);
            bodyView.transform.localScale = new Vector3(sign, 1.0f);
            armsView.transform.localScale = new Vector3(sign, 1.0f);
        }
        // TODO WT: Wall Grab and climbing
        // TODO WT: Pick up and put down of legs, arms and jump.


        isQPressed = Mathf.Max(0, (Input.GetKeyDown(KeyCode.Q)) ? 5 : isQPressed - 1);
        isEPressed = Mathf.Max(0, (Input.GetKeyDown(KeyCode.E)) ? 5 : isEPressed - 1);

        var currentVelocity = rigid.velocity;


        // TODO WT: Snap to wall we're climbing
        //canWallClimb = wallHitsInfo.Select(x => Vector2.Angle(Vector2.up, x.normal)).Contains(90.0f);
        if (isClimbEnabled && canWallClimb)
        {
            var verti = Input.GetAxis("Vertical") * currentMoveSpeed;
            currentVelocity.y = Mathf.Clamp(currentVelocity.y + verti, -currentMoveSpeed, currentMoveSpeed);
        }

        var horiz = Input.GetAxis("Horizontal");
        if (isGrounded)
        {
            var groundTangent = -new Vector2(-groundNormal.y, groundNormal.x);

            var normalComponent = Vector2.Dot(currentVelocity, groundNormal);
            var tangentComponent = Vector2.Dot(currentVelocity, groundTangent);
            var tangentMovement = horiz * currentMoveSpeed;

            currentVelocity += groundTangent * (tangentMovement - tangentComponent);

            //var feetPos = capsule.bounds.min + Vector3.right * capsule.size.x / 2.0f;
            //Debug.DrawRay(feetPos, groundTangent, Color.red);
            //Debug.DrawRay(transform.position, Vector2.right * horiz, Color.white);

            //var dotRight = Vector2.Dot(Vector3.right, groundTangent);
            //Debug.DrawRay(feetPos, groundTangent * dotRight, Color.yellow);

            //rigid.velocity = dir * horiz * currentMoveSpeed;
            //var vel = rigid.velocity;
            //vel.x = horiz * currentMoveSpeed;
            //rigid.velocity = vel;
        }
        else
        {
            if (Mathf.Abs(horiz) > 0.0f)
            {
                var moveAmount = horiz * currentMoveSpeed;

                moveAmount = (wallOnLeft) ? Mathf.Max(0.0f, moveAmount) : moveAmount;
                moveAmount = (wallOnRight) ? Mathf.Min(0.0f, moveAmount) : moveAmount;

                currentVelocity.x += moveAmount * Time.deltaTime * 10.0f;
                var sign = Mathf.Sign(currentVelocity.x);
                currentVelocity.x = Mathf.Min(Mathf.Abs(currentVelocity.x), currentMoveSpeed) * sign;
            }

            // Air control
            //horiz *= 0.5f;
            //currentVelocity.x += horiz * currentMoveSpeed * Time.fixedDeltaTime;
        }

        if (!canWallClimb)
        {
            rigid.gravityScale = (rigid.velocity.y < 0.1f) ? extraGravityScale : 1.0f;
        }

        rigid.velocity = currentVelocity;

        if (isGrounded || (isClimbEnabled && canWallClimb))
        {
            print("Can Jump " + rigid.velocity.y);
            if (Input.GetButtonDown("Jump") && rigid.velocity.y <= 0.001f)
            {
                rigid.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                canWallClimb = false;
            }
        }
    }

    private void FixedUpdate()
    {
        var notPlayerMask = ~(1 << 8);
        // Ground cast.
        var castDistance = 0.02f + collider.edgeRadius; // Should ideally be casting the next position after gravity is applied
        var groundHitsInfo = Physics2D.BoxCastAll((Vector2)transform.position + collider.offset, collider.size, 0.0f, Vector2.down, castDistance, notPlayerMask);
        Debug.DrawRay(transform.position, Vector3.down * castDistance, Color.blue);

        isGrounded = false;
        foreach (var hit in groundHitsInfo)
        {
            print("hit");
            if (hit.collider.tag == "Pickup")
            {
                continue;
            }

            print("was not pickup");

            if (Vector2.Angle(Vector2.up, hit.normal) <= maxSlopeAngle)
            {
                isGrounded = true;
                groundNormal = hit.normal;
            }
        }

        // Wall cast
        float dist = 0.1f;
        var wallHitsInfo = Physics2D.BoxCastAll(
            (Vector2)transform.position + collider.offset - Vector2.right * dist,
            new Vector2(collider.size.x, collider.size.y),
            0.0f,
            Vector2.right,
            dist * 2.0f,
            ~(1 << 8)
        );

        wallOnLeft = false;
        wallOnRight = false;

        canWallClimb = false;
        foreach (var hit in wallHitsInfo)
        {
            if (hit.collider.tag == "Pickup")
            {
                continue;
            }

            if (hit.point.x < rigid.position.x)
            {
                wallOnLeft = true;
            }

            if (hit.point.x > rigid.position.x)
            {
                wallOnRight = true;
            }

            if (Vector2.Angle(Vector2.up, hit.normal) == 90.0f)
            {
                canWallClimb = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Pickup")
        {
            if (isQPressed > 0 || isEPressed > 0) {
                var newPart = other.gameObject.GetComponent<Pickup>().part;
                other.gameObject.GetComponent<Pickup>().SetPart((isQPressed > 0) ? partA : partB);
                swapPart(newPart, isQPressed > 0);

                isQPressed = isEPressed = 0;
            }
        }
    }
}
