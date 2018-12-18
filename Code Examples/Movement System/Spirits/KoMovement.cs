using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*needs encapsulation.*/
public class KoMovement : MonoBehaviour
{
    // Object You want to Follow

    public SpiritController2D controller;
    public Animator animator;
    public Transform trailTarget;
    public Transform snapTarget;        // as yet not implemented.
    public Transform amayaTransform;
    public Vector2 amayaSnapOffset;
    // The Max Distance (in Unity units) before this Object follows the Target
    [Header("Framing Transposer")]
    public Vector2 frameTransposition;
    private Vector2 trailPosition;
    [Range(0f, 20f)]public float MaxXDistance = 5f;   // absolute values only
    [Range(0f, 20f)]public float MaxYDistance = 10f;  // absolute values only
    [Range(0f, 10f)]public float xDeadDistance = 3f;  // absolute values only
    [Range(0f, 10f)]public float yDeadDistance = 4f;  // absolute values only
    private ShieldEffect shield;

    // How fast you want to accelerate after the Target
    private Vector2 curPos;
    private Vector2 prevPos;
    private Vector2 velocity;
    private Vector2 targetCurPos;
    private Vector2 targetOffset;
    private Rect targetDeadFrame;
    private Rect targetSoftFrame;
    private Vector2 offsetToSoftRect;
    private Vector2 offsetToDeadRect;
    private bool insideSoftX;
    private bool insideSoftY;
    private bool insideDeadX;
    private bool insideDeadY;
    private bool snappedToPosition = false;
    private bool iceBlocked = false;
    private int angleQuad;
    private float AngleToDeadFrameR;
    private bool brakesSetExternally = false;

    public void SetTrailTarget(Transform transform) {
        trailTarget = transform;
    }

    public void Start()
    {
        shield = GetComponent<ShieldEffect>();
        SetTrailPosition();
        curPos = new Vector2(transform.position.x, transform.position.y);
        targetCurPos = new Vector2(trailPosition.x, trailPosition.y);
        velocity = new Vector2(0f, 0f);
        targetOffset = new Vector2(0f, 0f);
    }
    

    public Vector2 GetVelocity() { return velocity; }

    // set position of input transform to this NPC's transform.
    private Vector2 _SetPos(Vector2 pos)
    {
        pos.Set(transform.position.x, transform.position.y);
        return pos;
    }

    private Vector2 _SetVel(Vector2 curPosi, Vector2 prevPosi)
    {
        return ((curPosi - prevPosi) / Time.fixedDeltaTime);
    }

    // set position of input transform to the transform this NPC is following.
    private Vector2 _SetTargetPos(Vector2 target) {
        target.Set(trailPosition.x, trailPosition.y);
        return target;
    }

    // if target is at 10, 10, and I'm at 2, 2, targetOffset is 8, 8.
    // if target is at -5, -5, and I'm at 5, 5, targetOffset is -10, -10
    private void _SetTargetOffset() {
        targetOffset = targetCurPos - curPos;
    }

    private void SetTrailPosition() {
        trailPosition.Set(trailTarget.position.x + frameTransposition.x,
            trailTarget.position.y + frameTransposition.y);
    }
    // setup frames according to public thresholds.
    // max distance should be greater than dead distance.
    private void _SetTargetFrames()
    {
        if (MaxXDistance < xDeadDistance || MaxYDistance < yDeadDistance) {
            Debug.LogWarning("Warning: Soft Frame inside Dead Frame.");
        }
        targetDeadFrame.Set(
            targetCurPos.x - (xDeadDistance / 2), 
            targetCurPos.y - (yDeadDistance / 2),
            xDeadDistance, 
            yDeadDistance);

        targetSoftFrame.Set(
            targetCurPos.x - (MaxXDistance / 2),
            targetCurPos.y - (MaxYDistance / 2),
            MaxXDistance,
            MaxYDistance);

    }

    // get distance of least magnitude from the side of a frame
    private float _getOffsetToFrame(Rect frame, bool getX) {
        float maxOffset;
        float minOffset;
        if (getX) {
            maxOffset = frame.xMax - curPos.x;
            minOffset = frame.xMin - curPos.x;
        } else {
            maxOffset = frame.yMax - curPos.y;
            minOffset = frame.yMin - curPos.y;
        }

        bool minOffsetIsLesserAbs = Mathf.Abs(minOffset) < Mathf.Abs(maxOffset);

        float minAbsOffset = 0f;
        if (minOffsetIsLesserAbs) {
            minAbsOffset = minOffset;
        } else {
            minAbsOffset = maxOffset;
        }
        return minAbsOffset;
    }

    // get offset from nearest frame sides. Set to 0 if parallel to bounds.
    // if doubly parallel, you're inside the box and both floats are 0f.
    private void _SetNearestFrameOffsets() {
        _SetTargetOffset();
        if (Mathf.Abs(targetOffset.x) < MaxXDistance) { insideSoftX = true; }
        if (Mathf.Abs(targetOffset.y) < MaxYDistance) { insideSoftY = true; }
        if (Mathf.Abs(targetOffset.x) < xDeadDistance) { insideDeadX = true; }
        if (Mathf.Abs(targetOffset.y) < yDeadDistance) { insideDeadY = true; }
        float offsetToSoftX = 0f;
        float offsetToSoftY = 0f;
        float offsetToDeadX = 0f;
        float offsetToDeadY = 0f;
        if (!insideSoftX) {
            offsetToSoftX = _getOffsetToFrame(targetSoftFrame, true);
        }
        if (!insideSoftY) {
            offsetToSoftY = _getOffsetToFrame(targetSoftFrame, false);
        }
        if (!insideDeadX) {
            offsetToDeadX = _getOffsetToFrame(targetDeadFrame, true);
        }
        if (!insideDeadY) {
            offsetToDeadY = _getOffsetToFrame(targetDeadFrame, false);
        }
        offsetToSoftRect.Set(offsetToSoftX, offsetToSoftY);
        offsetToDeadRect.Set(offsetToDeadX, offsetToDeadY);
    }

    private void SetCurrentValues() {
        curPos = _SetPos(curPos);
        targetCurPos = _SetTargetPos(targetCurPos);
        velocity = _SetVel(curPos, prevPos);
    }

    private void SetCurrentOffsets() {
        _SetTargetFrames();
        _SetNearestFrameOffsets();
    }

    private void SetPrevPositions() {
        prevPos = _SetPos(prevPos);
    }

    public void EBrake() {
        brakesSetExternally = true;
    }

    private void SetBrakes() {
        int brakeLevel = 0;
        bool awayX = (velocity.x > 0f && offsetToSoftRect.x < 0f) || (velocity.x < 0f && offsetToSoftRect.x > 0f);
        bool awayY = (velocity.y > 0f && offsetToSoftRect.y < 0f) || (velocity.y < 0f && offsetToSoftRect.y > 0f);

        /*
            Reasons to increase brake intensity:
            Being inside an x or y boundary of a soft or dead zone.
                Thus, if directly parallel to a dead zone outside the soft zone, +2.
            Being outside a soft boundary but heading away from it.
            Maximum braking is 3rd level.
         */
        if (insideSoftX) { brakeLevel++; }
        if (insideSoftY) { brakeLevel++; }
        if (insideDeadX) { brakeLevel++; }
        if (insideDeadY) { brakeLevel++; }
        if (awayX) { brakeLevel++; }
        if (awayY) { brakeLevel++; }
        if (brakesSetExternally) { brakeLevel = 4; }
        if (snappedToPosition) { brakeLevel = 10; }
        switch (brakeLevel)
        {
            case 0:
                break;
            case 1:
                controller.Brake();
                break;
            case 2:
                controller.DeadBrake();
                break;
            case 3:
            case 4:
                controller.EmergencyBrake();
                break;
            case 10:
                controller.SnapBrake();
                break;
        }
        // if heading out of soft frame in x direction
    }

    private int GetQuadrant(Vector2 pointing) {
        Vector2 zero = Vector2.zero;
        int quadrant = 0;
        if (pointing.x < zero.x && pointing.y > zero.y) { quadrant = 1; }
        if (pointing.x < zero.x && pointing.y < zero.y) { quadrant = 2; }
        if (pointing.x > zero.x && pointing.y < zero.y) { quadrant = 3; }
        return quadrant;
    }

    private void GetAngleToDeadFrame() {
        if (offsetToDeadRect.x == 0 || offsetToDeadRect.y == 0) {
            if (offsetToDeadRect.y < 0f) { AngleToDeadFrameR = 1.5f * Mathf.PI; }
            if (offsetToDeadRect.y > 0f) { AngleToDeadFrameR = .5f * Mathf.PI; }
            if (offsetToDeadRect.x < 0f) { AngleToDeadFrameR = 1f * Mathf.PI; }
            if (offsetToDeadRect.x > 0f) { AngleToDeadFrameR = 0f; }
        } else {
            float angleRad = Mathf.Atan2(Mathf.Abs(offsetToDeadRect.y), Mathf.Abs(offsetToDeadRect.x));
            angleQuad = GetQuadrant(offsetToDeadRect);
            switch (angleQuad) {
                default:
                case 0:
                    AngleToDeadFrameR = 0f + angleRad;
                    break;
                case 1:
                    AngleToDeadFrameR = Mathf.PI - angleRad;
                    break;
                case 2:
                    AngleToDeadFrameR = Mathf.PI + angleRad;
                    break;
                case 3:
                    AngleToDeadFrameR = 0f - angleRad;
                    break;
            }
        }
    }

    // call this sustained.
    public void SnapToSnapPosition(Vector2 pos, bool _snapping) {
        if (_snapping) {
            snappedToPosition = true;
            controller.Snap(pos);
        } else {
            controller.StopSnapBraking("SnapToSnapPosition");
        }
    }

    // snap Ko to overhead of Amaya, and change to Shielding animation.
    // Frazzle afterward?
    public void ShieldMe(bool keepSustaining) {
        Debug.Log("Shield me.");
        if (shield.abilityActivated) {
            if (keepSustaining) {
                Vector2 pos = new Vector2(amayaTransform.position.x + amayaSnapOffset.x,
                    amayaTransform.position.y + amayaSnapOffset.y);
                SnapToSnapPosition(pos, true);
                animator.SetBool("IsShielding", true);
            } else {
                animator.SetBool("IsShielding", false);
                SnapToSnapPosition(Vector2.zero, false);
            }
        }
    }

    private void ResetBools() {
        insideSoftX = false;
        insideSoftY = false;
        insideDeadX = false;
        insideDeadY = false;
}

    private float GetAngle() {
        float angle = Mathf.Atan2(targetOffset.y, Mathf.Abs(targetOffset.x)) * Mathf.Rad2Deg;
        return angle;
    }
    public void FixedUpdate() {
        SetTrailPosition();
        SetCurrentValues();     // velocity and position
        SetCurrentOffsets();    // x,y offsets to target, soft frame, and dead frame.
        SetBrakes();

        // check trig quadrants, and assign thrust to the right quadrant.
        if ((!insideDeadX || !insideDeadY) && !snappedToPosition && !iceBlocked) {
            GetAngleToDeadFrame();
            float xThrust = Mathf.Cos(AngleToDeadFrameR);
            float yThrust = Mathf.Sin(AngleToDeadFrameR);
            
            if (AngleToDeadFrameR == 1.5f * Mathf.PI) { xThrust = 0;  yThrust = -1;}
            if (AngleToDeadFrameR == .5f * Mathf.PI)  { xThrust = 0;  yThrust = 1; }
            if (AngleToDeadFrameR == 1f * Mathf.PI)   { xThrust = -1; yThrust = 0; }
            if (AngleToDeadFrameR == 0f * Mathf.PI)   { xThrust = 1;  yThrust = 0; }

            controller.Move(xThrust, yThrust, GetAngle());
        } 
        if ((prevPos - curPos).magnitude > 0.1f) {
            animator.SetBool("moving", true);
        } else { 
            animator.SetBool("moving", false);
        }
        SetPrevPositions();
        ResetBools();
        brakesSetExternally = false;
        snappedToPosition = false;
    }
}