/*
 *  This class deals with Amaya's collisions and the disposition of her velocity,
 *      as that velocity is received from a child controller class.
 * */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CharacterObject2D : MonoBehaviour
{
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private LayerMask m_WhatIsMovableObject;
    [SerializeField] private LayerMask m_WhatIsWater;
    [SerializeField] private LayerMask m_WhatIsRain;
    [SerializeField] private float raindropSpeedModifier;
    public float pushForce;

    public CapsuleCollider2D amayaCapsule;
    private Vector2 amayaCapsuleOriginalSize;
    private Vector2 amayaCapsuleOriginalOffset;
    public CircleCollider2D amayaBoots;

    protected Vector2 targetVelocity;
    public Vector2 velocity;

    protected Rigidbody2D rb2d;
    protected const float minMoveDistance = 0.001f;
    protected ContactFilter2D groundContactFilter;
    protected ContactFilter2D runningWaterContactFilter;
    protected ContactFilter2D raindropContactFilter;
    protected ContactFilter2D movableObjContactFilter;
    protected RaycastHit2D[] landBuffer = new RaycastHit2D[16];
    protected RaycastHit2D[] splashBuffer = new RaycastHit2D[16];
    protected RaycastHit2D[] raindropBuffer = new RaycastHit2D[64];
    protected RaycastHit2D[] movableBuffer = new RaycastHit2D[16];
    protected RaycastHit2D[] standBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> landBufferList = new List<RaycastHit2D>(16);
    protected List<RaycastHit2D> movableBufferList = new List<RaycastHit2D>(16);
    protected const float shellRadius = 0.01f;
    protected bool onMovableObject = false;

    protected bool grounded;            // Whether or not the player is grounded.
    protected bool wasGrounded;
    protected Vector2 groundNormal;
    public float gravityModifier = 1f;
    public float minGroundNormalY = .9f;
    private bool update = true;


    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    [Header("Events")]
    [Space]

    [Tooltip("Unity will trigger the following script when the character lands on a ground collider.")]
        public UnityEvent OnLandEvent;
    [Tooltip("Unity will trigger the following script when the 'GetWet' public method is called.")]
        public UnityEvent OnGetWetEvent;
    [Tooltip("Unity will trigger the following script when the character lands on a water collider.")]
        public UnityEvent OnDrownEvent;
    [Tooltip("Unity will trigger the following script whenever she is hit by a raindrop.")]
        public UnityEvent OnRaindropEvent;
    [Tooltip("Unity will trigger the following script if she dries off from the rain.")]
        public UnityEvent OnDryOffEvent;


    private void Start()
    {
        amayaCapsuleOriginalSize = new Vector2(amayaCapsule.size.x, amayaCapsule.size.y);
        amayaCapsuleOriginalOffset = new Vector2(amayaCapsule.offset.x, amayaCapsule.offset.y);
        groundContactFilter = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true };
        groundContactFilter.SetLayerMask(m_WhatIsGround);

        runningWaterContactFilter = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true };
        runningWaterContactFilter.SetLayerMask(m_WhatIsWater);

        raindropContactFilter = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true };
        raindropContactFilter.SetLayerMask(m_WhatIsRain);

        movableObjContactFilter = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true };
        movableObjContactFilter.SetLayerMask(m_WhatIsMovableObject);
    }

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
        if (OnGetWetEvent == null)
            OnGetWetEvent = new UnityEvent();
        if (OnDrownEvent == null)
            OnDrownEvent = new UnityEvent();
        if (OnRaindropEvent == null)
            OnRaindropEvent = new UnityEvent();
        if (OnDryOffEvent == null)
            OnDryOffEvent = new UnityEvent();
    }

    protected virtual void ComputeVelocity()
    {
        // here for child to fill.
    }

    private void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();      // perhaps this being in update is the culprit for the jerky movement?

    }

    // consider moving some things from fixedupdate to update
    private void FixedUpdate()
    {
        if (update) {
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            
            //Debug.Log("Called target velocity at " + Time.fixedTime);
            velocity.x = targetVelocity.x;
            grounded = false;
            Vector2 deltaPosition = velocity * Time.deltaTime;

            // change x input to move parallel to ground normal, not exactly horizontal.
            Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
            Vector2 move = moveAlongGround * deltaPosition.x;

            Movement(move, false); // x movement call.

            move = Vector2.up * deltaPosition.y;

            Movement(move, true);
            wasGrounded = grounded;
        }
    }

    protected void SetFallingGroundFilter(bool falling) {
        if ((onMovableObject) || (falling)) {
            groundContactFilter.SetLayerMask(m_WhatIsGround + m_WhatIsMovableObject);
        }
    }

    protected void SetNormalGroundFilter() {
        onMovableObject = false;
        groundContactFilter.SetLayerMask(m_WhatIsGround);
    }

    public void Object_GetWet() {
        OnGetWetEvent.Invoke();
    }
    public void Object_DryOff() {
        OnDryOffEvent.Invoke();
    }

    public void ArrestMotion() {
        update = false;
    }

    private void ClearBuffersLists() {
        landBufferList.Clear();
        movableBufferList.Clear();
    }

    void Movement(Vector2 move, bool yMovement) {
        ClearBuffersLists();
        float distance = move.magnitude;
        if (distance > minMoveDistance) {

            // take collision counts of land, splash, raindrop, and movables.
            int landCount = rb2d.Cast(move, groundContactFilter, landBuffer, 
                distance + shellRadius);
            int splashCount = rb2d.Cast(move, runningWaterContactFilter,
                splashBuffer, distance + shellRadius);
            int raindropCount = rb2d.Cast(Vector2.up, raindropContactFilter,
                raindropBuffer, shellRadius * raindropSpeedModifier);
            int movableCount = rb2d.Cast(move, movableObjContactFilter, 
                movableBuffer, (distance + shellRadius));
            // invoke relevant states
            if (splashCount > 0) { OnDrownEvent.Invoke(); }
            for (int i = 0; i < landCount; i++) { landBufferList.Add(landBuffer[i]); }
            for (int i = 0; i < raindropCount; i++) { OnRaindropEvent.Invoke(); }
            for (int i = 0; i < movableCount; i++) { movableBufferList.Add(movableBuffer[i]); }
            for (int i = 0; i < movableCount; i++) {
                var obj = movableBufferList[i].rigidbody;
                movableBufferList[i].rigidbody.AddForceAtPosition(
                    new Vector2(velocity.x, velocity.y) * pushForce,
                    rb2d.position,
                    ForceMode2D.Force);
            }

            //determine slope angle of thing we're colliding with.
            for (int i = 0; i < landBufferList.Count; i++) {

                if (m_WhatIsMovableObject == (m_WhatIsMovableObject |
                    (1 << landBufferList[i].collider.gameObject.layer))) { // bitmask math
                    onMovableObject = true;
                }

                Vector2 surfaceNormal = landBufferList[i].normal;
                if (surfaceNormal.y > minGroundNormalY) {
                    if (yMovement) {
                        groundNormal = surfaceNormal;
                        surfaceNormal.x = 0f;
                        grounded = true;
                        if (grounded && !wasGrounded) {
                            //Debug.Log("Grounded.");
                            OnLandEvent.Invoke();
                        } // end new event invocation
                    } // end landing checks
                }   // end surface normal check
                float modifiedDistance = landBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            } // end for-loop through each hit
        } // end minimum distance check.
        rb2d.position = rb2d.position + move.normalized * distance;
    } // end method

    public void CrouchCollider()
    {
        Debug.Log("Crouching the capsule.");
        Vector2 crouchHeightAndOffset = new Vector2(0,
            amayaCapsuleOriginalOffset.y - (.3f));
        //amayaCapsule.size.Set(amayaCapsule.size.x, crouchHeightAndOffset.x);
        amayaCapsule.size = new Vector2(amayaCapsule.size.x, crouchHeightAndOffset.x);
        //amayaCapsule.offset.Set(amayaCapsule.offset.x, crouchHeightAndOffset.y);
        amayaCapsule.offset = new Vector2(amayaCapsule.offset.x, crouchHeightAndOffset.y);
    }

    private bool CeilingCheck() {
        bool clear = true;
        int headBumpCount = rb2d.Cast(Vector2.up, groundContactFilter, standBuffer, .5f);
        if (headBumpCount > 0) { clear = false; }
        return clear;
    }

    public void UncrouchCollider() {
        if (CeilingCheck()) {
            Debug.Log("Uncrouching the capsule.");
            Vector2 standingHeightAndOffset = new Vector2(amayaCapsuleOriginalSize.y, amayaCapsuleOriginalOffset.y);
            //amayaCapsule.size.Set(amayaCapsule.size.x, standingHeightAndOffset.x);
            amayaCapsule.size = new Vector2(amayaCapsule.size.x, standingHeightAndOffset.x);
            //amayaCapsule.offset.Set(amayaCapsule.offset.x, standingHeightAndOffset.y);
            amayaCapsule.offset = new Vector2(amayaCapsule.offset.x, standingHeightAndOffset.y);
        }
    }
}

