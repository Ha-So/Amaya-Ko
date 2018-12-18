using UnityEngine;
using UnityEngine.Events;


public class SpiritController2D : MonoBehaviour
{
    public Rigidbody2D rb2d;
    protected bool facingRight = true;  // For determining which way the NPC is currently facing.
    protected bool braking = false;
    protected bool deadBraking = false;
    protected bool emergencyBraking = false;
    protected bool snapBraking = false;
    protected bool tiltForward = false;
    protected Vector2 lookTarget;
    protected Vector2 snapStartPos;

    protected bool moving = false;
    protected bool frazzled = false;
    protected float lastTick;     // timestamp
    protected float frazzleStart; // timestamp

    [Space]
    [Range(1f, 50f)] public float thrustPower = 37.8f;
    [Range(1f, 100f)] public float maxSpeed = 20f;
    public float naturalTorque = 3.78f;
    [Tooltip("Animation period should be same as the period of the Idle animation.")]
    public float animationPeriod;

    [Header("Balance and recovery:")]
    [Tooltip("Recovery of upright rotation per second, " +
        "where 1.0 is nearly immediate.")]
    [Range(0f, 1f)] public float naturalBalance = .95f;
    protected float balance;                   // should be private
    public float perceivedLevel = 0f;      // should be private.
    protected float destabilization = 0f;    // level of unbalancing -- should be private
    [Tooltip("Additive recovery of natural balance per second, " +
        "where 1.0 is nearly instant recovery after frazzling.")]
    [Range(0f, 100f)]
    public float recoveryRate = 25f;
    [Range(0f, 5f)] public float frazzleTimer = 1.5f;

    [Header("Coefficients of Drag -- higher is more controlled:")]
    [Tooltip("The drag when going top speed.")]
    [Range(0f, 1f)]
    public float standardDrag = .5f;
    [Tooltip("The drag when going partially in the wrong direction," +
        "or nearing the target.")]
    [Range(0f, 1f)]
    public float brakeDrag = .75f;
    [Tooltip("The drag when going entirely in the wrong direction," +
        "or right on top of the target.")]
    [Range(0f, 1f)]
    public float deadBrakeDrag = .9f;
    [Tooltip("The drag when right on top of his target.")]
    [Range(.98f, 1f)]
    public float emergencyBrakeDrag = 1f;
    [Tooltip("The drag when Amaya engages her handbrake. Should be very high.")]
    public float snapDrag = 1000f;
    public float snapStopTime = .5f;
    public float snapMoveTime = 1f;
    protected TimePortionTracker snapTPT;
    protected Timer timer;
    protected bool frozen = false;
    bool wasStarted;

    protected void Start() {
        snapTPT = new TimePortionTracker(snapStopTime);
    }
    public void SetMoving(bool moving)
    {
        this.moving = moving;
    }

    public void Frazzle(float debilitation) {
        destabilization = debilitation;
        frazzleStart = Time.time;
        frazzled = true;
        balance = naturalBalance * (1 - destabilization);
    }

    [Header("Events")]
    [Space]
    public UnityEvent OnShieldEvent;
    public UnityEvent OnShieldEventOver;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { /*empty for user*/ }

    protected void Awake() {

        lastTick = Time.time;
        balance = naturalBalance;
        rb2d.angularDrag = 1 - naturalBalance * destabilization;
        rb2d = GetComponent<Rigidbody2D>();

        if (OnShieldEvent == null) {
            OnShieldEvent = new UnityEvent();
        }
        if (OnShieldEventOver == null) {
            OnShieldEventOver = new UnityEvent();
        }
    }

    /*************************************************
     *  Check if frazzled.
     *  If they are, check if the time is up.
     *  If it is, turn off frazzled.
     ************************************************/
    protected void FrazzleCheck() {
        if (frazzled) {
            if (frazzleStart + frazzleTimer > Time.time) {
                frazzled = false;
            }
        }
    }

    /**********************************************************************
     *  NPCs recovers stability over time after being frazzled:
     *      Their balance is their natural balance * destabilization.
     *      Destabilization decrements according to recovery rate.
     *********************************************************************/
    protected void Restabilize() {
        balance = naturalBalance * (1 - destabilization);
        if (destabilization > 0) {
            rb2d.angularDrag = 1 - naturalBalance * destabilization;
            destabilization = destabilization - (recoveryRate * Time.fixedDeltaTime);
        } else {
            destabilization = 0;
        }
    }

    /**********************************************************************
     *  Fluctuates randomly once per animation period, 
     *      in a threshold of balance.
     *********************************************************************/
    protected void Rebalance() {
        tiltForward = !tiltForward;
        float balanceFactor = (1f - (Random.value * .1f));
        perceivedLevel = ((balanceFactor) * 360) % 360; // the direction Ko feels is up, per frame.
        if (tiltForward) { perceivedLevel = perceivedLevel - 360; }
        perceivedLevel /= balance;
    }

    protected void AttitudeControl() {
        if (!moving) {
            float perceivedRotation = (perceivedLevel - rb2d.rotation) % 360;
            if (perceivedRotation > 180) { perceivedRotation = perceivedRotation - 360; }

            float zVelocity = 0.0f;
            float x = gameObject.transform.rotation.x;
            float y = gameObject.transform.rotation.y;
            float w = gameObject.transform.rotation.w;
            gameObject.transform.rotation = new Quaternion(x, y, Mathf.SmoothDampAngle(gameObject.transform.rotation.z, perceivedRotation, ref zVelocity, animationPeriod), w);
            //m_Rigidbody2D.AddTorque(naturalTorque * -1f, ForceMode2D.Force);
        }
    }

    /**********************************************************************
     *  Add hover thrust at constant rate equal to just less than
     *      equilibrium with gravity.
     *  Maintain vertical attitude relative to perceived level angle.
     *********************************************************************/
    protected void Hover() {
        // add hover thrust
        //Vector2 hoverThrust = new Vector2(0f, naturalThrust);
        //m_Rigidbody2D.AddForce(hoverThrust * 10f, ForceMode2D.Force);

        // if ticked past increment, rebalance.
        if ((Time.time % animationPeriod) < (lastTick % animationPeriod)) {
            Rebalance();
        }
        AttitudeControl();
        lastTick = Time.time;
    }

    /**********************************************************************
     * UPDATE ALGORITHM:
     *  Spirits always try to float a certain distance away from their
     *      follow target.
     *  NPCs hover by applying constant upward thrust in
     *      semi-equilibrium with gravity.
     *  NPCs move by applying thrust toward their target.
     *      Their natural Thrust must be less than or equal to gravity,
     *      and their thrust power plus natural thrust must be 
     *      significantly greater than gravity.
     *  NPCs also try to stay upright by applying natural torque, when 
     *      they aren't applying thrust.
     *  Overarching all of this, NPCs try to maintain an awareness of
     *      which way is up based on their balance and destabilization.
     *  They may go in the completely wrong direction if they are frazzled
     *      enough, or just roll on the ground.
     *  
     *********************************************************************/
    protected void FixedUpdate() {


        BrakeCheck();   // set drag
        FrazzleCheck(); // check frazzle timer and set bool
        Restabilize();  // apply NPC's recovery rate from frazzling
        Hover();        // keep the NPC upright (or not if destabilized)

        Unfreeze();
    }

    //public void SetLookTarget(Vector2 target)
    //{
    //    Vector2 rb2d = new Vector2(m_Rigidbody2D.position.x, m_Rigidbody2D.position.y);
    //    lookTarget = target - rb2d;
    //    lookTarget.Normalize();
    //}

    protected void BrakeCheck() {
        rb2d.drag = standardDrag;
        if (snapBraking) {
            rb2d.drag = snapDrag;
        } else {
            if (emergencyBraking) {
                rb2d.drag = emergencyBrakeDrag;
            } else {
                if (deadBraking) {
                    rb2d.drag = deadBrakeDrag;
                } else {
                    if (braking) {
                        rb2d.drag = brakeDrag;
                    } // end brake
                } // end dead brake
            } // end emergency brake
        } // end snap brake

        // ensure braking ends when KoMovement stops sending brake signals.
        braking = false;
        deadBraking = false;
        emergencyBraking = false;
        snapBraking = false;
    }

    /**********************************************************************
     *  Drag slows down movement through the air. It's how a flying
     *      spirit slows itself down, either naturally or voluntarily.
     *  A spirit cannot exceed a maximum speed.
     *      If it still does, increase braking drag.
     *      Dead brake drag should always be higher than brake drag.
     *********************************************************************/
    public void Brake() {
        braking = true;
        rb2d.drag = brakeDrag;
    }
    protected void AirBrake() {
        float linearVelocity = rb2d.velocity.magnitude;
        if (linearVelocity > maxSpeed) {
            Brake();
        }
    }
    public void DeadBrake() {
        deadBraking = true;
        rb2d.drag = deadBrakeDrag;
    }

    public void EmergencyBrake() {
        emergencyBraking = true;
        rb2d.drag = emergencyBrakeDrag;
    }
    // snaps to a halt smoothly 0 to .5s after snap starts.
    public void SnapBrake() {
        snapBraking = true;
    }

    public void SnapStart() { // should only fire once per seesKo:true, then false
        if (!snapBraking) {
            if (!wasStarted) {
                snapTPT.Start();
                wasStarted = true;
            }
            
            snapStartPos = new Vector2(rb2d.position.x, rb2d.position.y);
            snapBraking = true;
        }
    }

    public void StopSnapBraking(string caller) {
        snapBraking = false;
        rb2d.drag = .1f;
        wasStarted = false; // hopefully this works out as planned?
    }
    
    // freeze all movement
    public void Freeze(float t) {
        frozen = true;
        timer = new Timer(t);
        timer.Start();
    }

    // stop freezing when time is up.
    protected void Unfreeze() {
        if (frozen && timer.GetElapsed()) {
            frozen = false;
        }
    }

    // use only for kinematic maneuvers, like shielding or body slamming.
    // snaps to position smoothly between .5 and 1s after snap starts.
    public void Snap(Vector2 pos) {
        if (!frozen) { 
            SnapStart();
            float portionElapsed = snapTPT.GetPortion();
            if (portionElapsed != 42 && portionElapsed <= 1f) {
                float x = Mathf.Lerp(snapStartPos.x, pos.x, portionElapsed);
                float y = Mathf.Lerp(snapStartPos.y, pos.y, portionElapsed);
                rb2d.MovePosition(new Vector2(x, y));
            }

            if (gameObject.name == "Ko") {
                bool flip = GameObject.Find("Amaya").GetComponent<SpriteRenderer>().flipX;
                gameObject.GetComponent<SpriteRenderer>().flipX = !flip;
            }
        }
    }

    /**********************************************************************
     *  If not frazzled, try to move in the prescribed direction.
     *  Add force to thrust in that direction, and add rotation to match
     *      the angle of that velocity.
     *  Finally, flip the sprite per its direction.
     *********************************************************************/
    public void Move(float xThrust, float yThrust, float angle) {
        if (!frozen) {
            //only control the NPC if not frazzled
            if (!frazzled) {
                Vector3 targetThrust = new Vector2(xThrust * thrustPower, yThrust * thrustPower);

                /**************************************************************
                 * Mutate thrust based on perceivedLevel frame of reference.
                 *      Not yet implemented.
                 *************************************************************/

                // add no thrust in any direction if past max speed. Only brake.
                if (rb2d.velocity.magnitude < maxSpeed) {
                    rb2d.AddForce(targetThrust, ForceMode2D.Force);
                }
                //transform.eulerAngles.Set(transform.eulerAngles.x, transform.eulerAngles.y, angle);
                float deltaAngle = (transform.rotation.z - angle) / (.05f / Time.fixedDeltaTime);
                rb2d.MoveRotation(deltaAngle);

                // If the trail is moving the player right and the player is facing left...
                if (xThrust > 0 && !facingRight) {
                    // ... flip the NPC by scale.
                    Flip();
                }
                // Otherwise if the trail is moving the NPC left and the player is facing right...
                else if (xThrust < 0 && facingRight) {
                    // ... flip the NPC by scale.
                    Flip();
                }
            }
        }
    }

    protected virtual void Flip() {
        rb2d.AddRelativeForce(Vector2.up * thrustPower / 10, ForceMode2D.Impulse);
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ResetSnapLerpTimer() { snapTPT.Start(); }

}
