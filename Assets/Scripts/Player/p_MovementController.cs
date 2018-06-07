using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_MovementController : p_Base
{
    public enum e_MovementState
    {
        Standing = 1,
        WalkLeft = 2,
        WalkRight = 4,
        WalkForward = 8,
        WalkBackward = 16,
        RunLeft = 32,
        RunRight = 64,
        RunForward = 128,
        RunBackward = 256,
        SprintingLeft = 512,
        SprintingRight = 1024,
        SprintingForward = 2048,
        SprintingBackward = 4096,
        DashingLeft = 8192,
        DashingRight = 16384,
        DashingForward = 32768,
        DashingBackward = 65536,
        InAir = 131072,
        Climbing = 262144
    }

    //Standing Parameters
    [SerializeField] e_MovementState movementState;

    [Header("Walk parameters")]
    [SerializeField] float s_Acceleration;
    [SerializeField] float s_WalkMaxSpeed;
    [SerializeField] float s_RunMaxSpeed;
    [SerializeField] float s_SideBrake;
    [SerializeField] float s_AutoBrake;

    //Sprint Parameters
    [Header("Sprint parameters")]
    [SerializeField] float ss_Acceleration;
    [SerializeField] float ss_MaxSpeed;
    [SerializeField] float ss_SideBrake;

    //In AIr Parameters
    [Header("In Air parameters")]
    [SerializeField] float a_Acceleration;
    [SerializeField] float a_MaxSpeed;
    [SerializeField] float a_SideBrake;
    [SerializeField] float a_AutoBrake;
    
    [Header("Jump & Dash parameters")]
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpCooldown;
    [SerializeField] float dashMinSpeed;
    [SerializeField] float dashPeakSpeed;
    [SerializeField] float dashFirstTransitionLinearity = 2f;
    [SerializeField] float dashLastTransitionLinearity = 2f;
    [SerializeField] float dashTransitionLength = 0.35f;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;

    [Header("Ladder Parameters")]
    [SerializeField] float ladderClimbSpeed = 0.02f;
    [SerializeField] float upLadderMaxHeightOffset = 0.25f;
    [SerializeField] float bottomLadderMinHeightOffset = 0.25f;
    [SerializeField] float upClimbTime = 0.5f;
    [SerializeField] float upClimbStartLinearity = 1.25f;
    [SerializeField] float upClimbEndLinearity = 1.25f;

    [Header("Misc")]
    [SerializeField] float normalYGrounded = 0.8f;

    //Calculation Getters
    public bool JumpUp { get { return Time.time > lastJumpTime + jumpCooldown; } }
    public bool DashUp { get { return Time.time > lastDashTime + dashCooldown; } }

    public new bool Dashing { get { return Time.time < lastDashTime + dashDuration; } }
    float DashSpeedProgress
    {
        get
        {
            float dashProgress = (dashDuration - ((lastDashTime + dashDuration) - Time.time)) / dashDuration;//Will change from 0 to 1 during dash progress

            if (dashProgress < dashTransitionLength)
            {
                return Mathf.Pow(dashProgress/ dashTransitionLength, dashFirstTransitionLinearity);
            }
            else if (dashProgress > 1 - dashTransitionLength)
            {
                return Mathf.Pow(1 - ((dashProgress - (1 - dashTransitionLength)) / dashTransitionLength), dashLastTransitionLinearity);
            }
            else
            {
                return 1;
            }
        }
    }

    //Members
    //Public
    new public e_MovementState MovementState { get { return movementState; } }
    public Vector3 MoveDirection { get; protected set; }
    public Vector3 MoveInput { get; protected set; }
    public new bool Grounded { get; private set; }
    public new bool Sprinting { get; private set; }

    public float SprintMaxSpeed { get { return ss_MaxSpeed; } }

    public Ladder CurrentLadder { get { return currentLadder; } }

    //private
    Transform sbCalculator;
    float lastJumpTime, lastDashTime = -Mathf.Infinity;
    bool oldSprinting = false, oldDashing = false;
    Ladder currentLadder = null;

    protected override void Awake ()
    {
        base.Awake();

        sbCalculator = Body.Find("SideBrakeCalculator");
        if (sbCalculator == null)
        {
            sbCalculator = new GameObject().transform;
            sbCalculator.SetParent(Body);
            sbCalculator.localPosition = Vector3.zero;
        }
    }

    protected void Start()
    {
        StartCoroutine("GroundedHandler");
    }

    Vector2 moveInput;
    Vector3 moveVector;

    IEnumerator GroundedHandler()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!JumpUp || movementState == e_MovementState.Climbing)
                Grounded = false;
            else
            {
                RaycastHit sphereHit;

                Vector3 startPos = (Body.position + Vector3.up * 0.59f);
                Physics.SphereCast(startPos, BodyColl.radius / 1.1f, Vector3.down, out sphereHit, Grounded ? 0.8f : 0.6f, Game_NG.GroundLayerMask);

                Grounded = false;

                if (sphereHit.collider != null)
                {
                    //So the normal isnt based on the sphere shape
                    Physics.Raycast(sphereHit.point + Vector3.up, Vector3.down, out sphereHit, 1.1f, Game_NG.GroundLayerMask);

                    //Cant climb walls lol
                    if (sphereHit.normal.y > normalYGrounded)
                    {
                        Grounded = true;
                        if (oldSprinting)
                        {
                            Sprinting = true;
                            oldSprinting = false;
                        }

                        V.TellStepOffset(sphereHit.point.y - Body.position.y);

                        Body.position = sphereHit.point.SetX(Body.position.x).SetZ(Body.position.z);
                        R.velocity = R.velocity.SetY(0);
                    }
                }

                if (!Grounded)
                {
                    R.velocity += Physics.gravity * Time.fixedDeltaTime;

                    if (Sprinting || (oldSprinting && C.MoveInput.sqrMagnitude > 0.25f))
                        oldSprinting = true;
                    else
                        oldSprinting = false;
                }

                ActualizeBodycollShape();
            }
        }
    }
    void ActualizeBodycollShape()
    {
        if (Grounded)
        {
            BodyColl.transform.localPosition = Vector3.up * 0.95f;
            BodyColl.height = 1.5f;
        }
        else
        {
            BodyColl.transform.localPosition = Vector3.up * 0.9f;
            BodyColl.height = 1.6f;
        }
    }

    protected void Update ()
    {
        //Operations like "Is it sprinting ?", "Force min walk force", "Transform moveDir", etc.
        DirectionAndStatesOperationsHandler();

        //if cannot move (dashing, stunned, dead)
        if ((PB.CurLivingState & (LivingBeing.LivingState.Stunned | LivingBeing.LivingState.Dead)) != 0 || InteractionState == p_InteractionController.e_InteractionState.Interacting)
        {
            StunMovementHandler();
            return;
        }

        //If there is no ladder
        if (movementState != e_MovementState.Climbing)
        {
            //Actualize state so we know everywhere what the movements of the character are
            ActualizeState();

            //Actually moves for real.
            if (Dashing)
                DashingMovementHandler();
            else
                MovementHandler();
        }
        //If on Ladder
        else
            OnLadderHandler();
    }


    void DirectionAndStatesOperationsHandler()
    {
        moveInput = C.MoveInput;

        if (Grounded && AttackState == p_AttackController.e_AttackState.None && movementState != e_MovementState.Climbing)
        {
            if (EnemyLocked)
            {
                if ((movementState & (e_MovementState.WalkForward | e_MovementState.SprintingForward | e_MovementState.RunForward)) != 0 && (C.Sprint || Sprinting))
                    Sprinting = true;
                else
                    Sprinting = false;
            }
            else
                Sprinting = C.Sprint || Sprinting;
        }
        else
            Sprinting = false;

        if (Sprinting)
        {
            if (moveInput.sqrMagnitude > 0.25f)//magnitude > 0.5f
                moveInput.Normalize();
            else
                Sprinting = false;
        }

        if (Dashing)
            return;

        if (moveInput.sqrMagnitude != 0)
        {
            if (moveInput.sqrMagnitude < 0.25f)
                moveInput = moveInput.normalized * 0.5f;
            else
                moveInput.Normalize();

            moveVector = CamPivot.TransformVector(moveInput.ToXZ());

            MoveDirection = moveVector.normalized;
        }
        else
            MoveDirection = Visual.forward;

        MoveInput = moveInput;

    }

    void ActualizeState()
    {
        if (Dashing && (movementState & (e_MovementState.DashingBackward | e_MovementState.DashingForward | e_MovementState.DashingRight | e_MovementState.DashingLeft)) == 0)
        {
            Vector3 vector = CamPivot.InverseTransformDirection(MoveDirection);

            if (vector.z >= Mathf.Abs(vector.x))
                movementState = e_MovementState.DashingForward;
            else if (vector.z <= -Mathf.Abs(vector.x))
                movementState = e_MovementState.DashingBackward;
            else if (vector.x > Mathf.Abs(vector.z))
                movementState = e_MovementState.DashingRight;
            else
                movementState = e_MovementState.DashingLeft;
        }
        else if (Grounded)
        {
            if (moveVector.sqrMagnitude == 0)
                movementState = e_MovementState.Standing;
            else if (moveInput.y >= Mathf.Abs(moveInput.x))
                movementState = Sprinting ? e_MovementState.SprintingForward : moveInput.sqrMagnitude < 1f ? e_MovementState.WalkForward : e_MovementState.RunForward;
            else if (moveInput.y <= -Mathf.Abs(moveInput.x))
                movementState = Sprinting ? e_MovementState.SprintingBackward : moveInput.sqrMagnitude < 1f ? e_MovementState.WalkBackward : e_MovementState.RunBackward;
            else if (moveInput.x > Mathf.Abs(moveInput.y))
                movementState = Sprinting ? e_MovementState.SprintingRight : moveInput.sqrMagnitude < 1f ? e_MovementState.WalkRight : e_MovementState.RunRight;
            else
                movementState = Sprinting ? e_MovementState.SprintingLeft : moveInput.sqrMagnitude < 1f ? e_MovementState.WalkLeft : e_MovementState.RunLeft;
        }
        else
            movementState = e_MovementState.InAir;
    }

    void StunMovementHandler()
    {
        R.velocity = Utilities.SubtractMagnitude(R.velocity, s_AutoBrake * Utilities.Delta);
    }
    void DashingMovementHandler()
    {
        if (Dashing)
        {
            oldDashing = true;
            R.velocity = MoveDirection * (DashSpeedProgress * (dashPeakSpeed - dashMinSpeed) + dashMinSpeed);
            return;
        }
        else if (oldDashing)
        {
            oldDashing = false;
            R.velocity = MoveDirection * (oldSprinting ? ss_MaxSpeed : (moveInput.magnitude <= 0.5f ? s_WalkMaxSpeed : s_RunMaxSpeed));
        }
    }
    void MovementHandler()
    {
        if (Grounded)
        {
            if (AttackState != p_AttackController.e_AttackState.None)
                R.velocity = Utilities.SubtractMagnitude(R.velocity, s_AutoBrake * Utilities.Delta);
            else
                GroundedMovementHandler();
        }
        else
        {
            if (AttackState != p_AttackController.e_AttackState.None)
                R.velocity = Utilities.SubtractMagnitude(R.velocity, a_AutoBrake * Utilities.Delta);
            else
                InAirMovementHandler();
        }
    }
    void GroundedMovementHandler()
    {
        //Get acceleration vector
        moveVector *= (Sprinting ? ss_Acceleration : s_Acceleration) * Utilities.Delta;

        if (moveVector.magnitude > 0)
        {
            //Side brake
            sbCalculator.LookAt(sbCalculator.position + moveVector);
            R.velocity = sbCalculator.InverseTransformVector(R.velocity);
            R.velocity = R.velocity.SetX(Utilities.SubtractFloat(R.velocity.x, (Sprinting ? ss_SideBrake : s_SideBrake) * Utilities.Delta, 0, true));
            R.velocity = sbCalculator.TransformVector(R.velocity);

            //Apply it
            R.velocity = Utilities.AddWithLimit(R.velocity, moveVector, (Sprinting ? ss_MaxSpeed : moveInput.magnitude <= 0.5f ? s_WalkMaxSpeed : s_RunMaxSpeed), Utilities.CalculMode.NoYAxis, Utilities.LimitMode.Smooth, (Sprinting ? ss_Acceleration : s_Acceleration) * Utilities.Delta);
        }
        else
            R.velocity = Utilities.SubtractMagnitude(R.velocity, s_AutoBrake * Utilities.Delta);
        
        //Jump
        if (C.Jump && JumpUp)
        {
            R.velocity = R.velocity.SetY(jumpSpeed);
            lastJumpTime = Time.time;

            Grounded = false;

            if (Sprinting)
                oldSprinting = true;

            SendMessage("OnJump");
        }
        //Dash
        if (C.Dash && DashUp)
        {
            R.velocity = MoveDirection * dashMinSpeed;
            lastDashTime = Time.time;

            if (Sprinting)
                oldSprinting = true;

            SendMessage("OnDash");
        }
    }
    void InAirMovementHandler()
    {
        //Get acceleration vector
        moveVector *= a_Acceleration * Utilities.Delta;

        if (moveVector.magnitude > 0)
        {
            //Apply it
            R.velocity = Utilities.AddWithLimit(R.velocity, moveVector, a_MaxSpeed, Utilities.CalculMode.NoYAxis, Utilities.LimitMode.Smooth, 0);
        }
        else
            R.velocity = Utilities.SubtractMagnitude(R.velocity, a_AutoBrake * Utilities.Delta);
    }

    bool waitForwardRelease = false;
    void OnLadderEnter(Ladder ladder)
    {
        movementState = e_MovementState.Climbing;
        R.isKinematic = true;
        R.useGravity = false;
        R.velocity = Vector3.zero;

        currentLadder = ladder;

        //If we are in the precise case where you walk straight a ladder from above it, you wont go up where you came from automatically.
        if (CurrentLadder.TestHeight(BodyColl.transform.position.y - upLadderMaxHeightOffset) == true)
        {
            waitForwardRelease = true;
        }
        Debug.Log(CurrentLadder.TestHeight(BodyColl.transform.position.y - upLadderMaxHeightOffset));
    }
    
    //True if up the ladder, false if bottom, null if middle
    void OnLadderExit(bool? ladderHeightPos)
    {
        switch (ladderHeightPos)
        {
            case null:
                GiveUpLadder();
                break;
            case true:
                StartCoroutine(ClimbLadder());
                break;
            case false:
                GiveUpLadder();
                break;
        }
    }

    void GiveUpLadder()
    {
        movementState = e_MovementState.Standing;
        R.isKinematic = false;
        R.useGravity = true;
        currentLadder = null;
    }

    IEnumerator ClimbLadder()
    {
        PB.StartCoroutine(PB.StunCoroutine(upClimbTime));
        Vector3 oPos = Body.position;
        Vector3 targetMidPos = Vector3.Lerp(oPos, currentLadder.ClimbPosition, 0.5f);
        Vector3 targetPos = currentLadder.ClimbPosition;

        float count = 0f;
        while (count < upClimbTime * 0.5f)
        {
            Body.position = Vector3.Lerp(oPos, targetMidPos, Mathf.Pow(count / (upClimbTime * 0.5f), upClimbStartLinearity));
            Body.position = Body.position.SetY(Utilities.Lerp(oPos.y, targetPos.y, Mathf.Pow(count / upClimbTime, 0.5f)));

            yield return null;
            count += Time.deltaTime;
        }
        count -= upClimbTime * 0.5f;
        while (count < upClimbTime * 0.5f)
        {
            Body.position = Vector3.Lerp(targetMidPos, targetPos, Mathf.Pow(count / (upClimbTime * 0.5f), upClimbEndLinearity));
            Body.position = Body.position.SetY(Utilities.Lerp(oPos.y, targetPos.y, Mathf.Pow((count + upClimbTime * 0.5f) / upClimbTime, 0.5f)));

            yield return null;
            count += Time.deltaTime;
        }

        Body.position = targetPos;
        GiveUpLadder();
    }

    void OnLadderHandler()
    {
        //Same as the comment into "OnladderEnter"
        if (moveInput.y <= 0)
        {
            waitForwardRelease = false;
        }

        bool? testBuffer;
        if (moveInput.y > 0)
            testBuffer = currentLadder.TestHeight(BodyColl.transform.position.y - upLadderMaxHeightOffset);
        else
            testBuffer = currentLadder.TestHeight(Body.position.y + bottomLadderMinHeightOffset);

        //Coming from above or extreme underneath on the ladder will make the character give up the ladder instantly.
        //This code make it so the character can only leave if his input ask him to do it, also, it will force it to
        //be at a TestHeight() == null height for the ladder
        if (testBuffer != null)
        {
            //This comment only concerns the || waitForwardRelease condition.
            //In the case where the guy hold forward, we force the code to pass through "null" which handles waitForForward
            //Also, we dont have to rewrite the code to make the character's body stick to ladder center if we do this.
            if ((bool)testBuffer && moveInput.y <= 0 || waitForwardRelease)
            {
                testBuffer = null;
                Body.position -= Vector3.up * ladderClimbSpeed * Time.deltaTime;
            }
            else if (!((bool)testBuffer) && moveInput.y >= 0)
            {
                testBuffer = null;
                Body.position += Vector3.up * ladderClimbSpeed * Time.deltaTime;
            }
        }


        switch (testBuffer)
        {
            case null:
                //Always make the body glue to center
                Body.position = Vector3.Lerp(Body.position, currentLadder.Center.SetY(Body.position.y), 0.1f);

                //Preventing climbing ladder when entering it from above
                if (moveInput.y > 0 && waitForwardRelease)
                    break;

                //Actually climbing
                Body.position += Vector3.up * moveInput.y * ladderClimbSpeed * Time.deltaTime;
                break;
            case true:
                OnLadderExit(true);
                break;
            case false:
                OnLadderExit(false);
                break;
        }
    }
}