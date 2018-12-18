using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterObject2D
{
    public float jumpTakeOffSpeed = 20;
    public float crouchModifier = .36f;
    public float maxSpeed = 40f;

    private SpriteRenderer spriteRenderer;
    public Animator animator;
    
    // Use this for initialization
    public bool aboutToJump;
    public bool justLanded;
    public bool crouching;
    public bool jumping;
    public bool falling;
    private bool wasFalling;
    public bool wet;
    private bool ZombieWalking;
    private float ZWStartTime;

    [Range(0f, 1f)] public float jumpDelay = .35f;
    [Range(0f, 1f)] public float landDelay = .3f;
    private float yVelocityAtLanding = 0f;
    private float jumpStartTime;
    private float landingStartTime;
    [Range(5f, 30f)]public float timeToDryOff = 10f;
    private bool facingLeft;
    private float stickTheLandingFallTime = .15f;
    private float startFallTime;
    private bool dialogue;

    //private Vector2 velocity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        aboutToJump = false;
        crouching = false;
        jumping = false;
        justLanded = false;
        facingLeft = false;
        dialogue = false;
    }
       
    // called by parent fixedUpdate
    protected override void ComputeVelocity() {
        wasFalling = falling;
        // calculated at update, applied at fixedupdate.
        if (justLanded) {
            float lerpTime = ((Time.fixedTime - landingStartTime)) / landDelay;
            velocity.y = Mathf.Lerp(yVelocityAtLanding, -gravityModifier, lerpTime);
        }
        else if (aboutToJump) {
            float lerpTime = ((Time.fixedTime - jumpStartTime)) / jumpDelay;
            velocity.y = Mathf.Lerp(-gravityModifier, -jumpTakeOffSpeed, lerpTime);
        }
        else if (!jumping && !wasFalling) { velocity.y = -gravityModifier; }
        Vector2 move = Vector2.zero;
        if (!dialogue) {
            move.x = Input.GetAxis("Horizontal");
        }
        if (aboutToJump || justLanded) {
            move.x = 0f;

        }

        CheckCrouch();

        // set crouch velocity.x
        if ((crouching && grounded) || (wet)) {
            move.x *= crouchModifier;
        }

        if (!aboutToJump) {
            ReadyJump();    // timer start
        } else {
            Jump();         // timer check
        }

        if ((facingLeft && move.x > 0.01f) || (!facingLeft && move.x < -0.01f)) {
            facingLeft = !facingLeft;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        if (justLanded && grounded) {
            if (Time.fixedTime > startFallTime + stickTheLandingFallTime) { // timer check
                StickTheLanding();     // timer check
            }
        }
        
        if (!grounded && (velocity.y < -0.001f)) { Fall(); }

        animator.SetFloat("Speed", Mathf.Abs(velocity.x) / maxSpeed);
        SetAnimator();
        targetVelocity = move * maxSpeed;
        SetFallingGroundFilter(falling || wasFalling);

    }

    private void ZWStart() {
        if (!ZombieWalking) {
            ZombieWalking = true;
            ZWStartTime = Time.time;
        }
    }
    
    public void ZWStop() {
        ZombieWalking = false;
    }

    public void ZombieWalk(bool leftToRight) {
        ZWStart();
        Vector2 forward;
        if (leftToRight) { forward = Vector2.right; }
        else { forward = Vector2.left; }

        animator.SetFloat("Speed", Mathf.Abs(velocity.x) * maxSpeed);
        SetAnimator();

        if (Time.time < ZWStartTime + 2f) { targetVelocity = new Vector2(Mathf.Lerp(targetVelocity.x,
            forward.x * maxSpeed * crouchModifier, ((Time.time - ZWStartTime) / 2)), 0);
        } else {
            targetVelocity = forward * maxSpeed * crouchModifier;
        }
    }

    protected void SetAnimator() {
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsJumping", aboutToJump);
        animator.SetBool("IsCrouching", crouching);
        animator.SetBool("IsLanding", justLanded);
        animator.SetBool("IsFalling", falling);
        animator.SetBool("IsWet", wet);
    }


    protected void ReadyJump() {
        if (!dialogue && Input.GetButtonDown("Jump") && grounded) {
            if (!wet) { // can't jump while wet.
                aboutToJump = true;
                jumpStartTime = Time.fixedTime;
            }
        }
        if (Input.GetButtonUp("Jump") || wet) {
        // getting wet mid-jump stunts the velocity, like letting go of the button early.
            if (velocity.y > 0f) {
                velocity.y *= .5f;
            }
        }
    }

    protected void Jump() {
        // getting wet while preparing a jump cancels it.
        if (Input.GetButtonUp("Jump") || (wet)) {
            aboutToJump = false;
        }

        if (Time.fixedTime > jumpDelay + jumpStartTime) {
            if ((Input.GetButton("Jump")) && (grounded))
            {
                velocity.y = jumpTakeOffSpeed;
                aboutToJump = false;
                jumping = true;
                SetNormalGroundFilter();
            }

        }
    }

    protected void Fall() {
        aboutToJump = false;
        jumping = false;
        falling = true;
        justLanded = false;
        startFallTime = Time.fixedTime;

    }

    protected void StickTheLanding() {
        if (Time.fixedTime > (landDelay + landingStartTime)) {
            justLanded = false;
        }
    }


    // put this into the event handler.
    public void Land() {
        justLanded = true;
        yVelocityAtLanding = velocity.y;
        landingStartTime = Time.fixedTime;
        falling = false;
        jumping = false;
    }

    // put this into the event handler.
    public void Movement_GetWet() {
        wet = true;
    }

    public void Movement_DryingOff() {
        wet = false;
    }

    protected void CheckCrouch() {
        if (Input.GetButtonDown("Crouch") && !aboutToJump) {
            CrouchCollider();
            crouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            UncrouchCollider();
            crouching = false;
        }
        if (wet && velocity.x < 0.001f) {
            CrouchCollider();
        }
    }

     public void Drown() {
        animator.SetBool("IsDrowning", true);
    }
    public void DialogueOn() {
        dialogue = true;
    }
    public void DialogueOff() {
        dialogue = false;
    }

}

