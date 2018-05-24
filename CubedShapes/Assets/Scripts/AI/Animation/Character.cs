using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public enum ArmState
{
    Idle,
    Aiming
}
public enum BodyState
{
    Standing,
    Jumping,
    Crouching
}
public enum GunState
{
    Idle,
    Shooting
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character: MonoBehaviour
{
    //Variables
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_JumpPower = 6f;
    [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
    [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField] float m_MoveSpeedMultiplier = 1; //1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1; //1f;
    [SerializeField] float m_GroundCheckDistance = 0.2f;


    private float origGroundCheckDistance;
    private const float k_Half = 0.5f;
    private float turnAmount;
    private float forwardAmount;
    private Vector3 groundNormal;
    private float capsuleHeight;
    private Vector3 capsuleCenter;

    //Public
    public float stationaryTurnMultiplier = 3;
    //public bool isGrounded;
    public Transform lastWalkedOn;

    // Animation states
    /*public bool rifling;
    public bool shooting;
    public bool equipped;
    public bool crouching;*/

    //Head IK
    private float lookIKWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;
    private float clampWeight;

    //Foot IK
    private Transform leftFoot;
    private Transform rightFoot;


    private GameUnit lookingAt;

    //Owner
    public GameUnit owner;

    //State
    public ArmState armState;
    public BodyState bodyState;
    public GunState gunState;

    //Reactions
    public float reactionTime;
    public float currentReactionCycle;

    public void Update()
    {
        currentReactionCycle += Time.deltaTime;
    }
    public bool ShouldAct()
    {
        return currentReactionCycle >= reactionTime;
    }

    // Components
    /* public ItemEquiper itemEquiper;
     private Rigidbody rigid;
     private Animator anim;
     private CapsuleCollider capsule;
     public NavMeshAgent navAgent;
     private CharacterLinkMover linkMover;*/


    /*public void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        linkMover = this.gameObject.AddComponent<CharacterLinkMover>();
    }*/

    void Start()
    {
        armState = ArmState.Idle;
        bodyState = BodyState.Standing;
        gunState = GunState.Idle;

        /*rifling = false;
        shooting = false;*/

        capsuleHeight = this.owner.collider.height;
        capsuleCenter = this.owner.collider.center;

        this.owner.rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = m_GroundCheckDistance;

        leftFoot = this.owner.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = this.owner.animator.GetBoneTransform(HumanBodyBones.RightFoot);

    }

    /*public void UpdateWithEquippedItems()
    {
        foreach (Item j in itemEquiper.equipped.Values)
        {
            if (j is JetPack)
            {
                linkMover.characterJetpack = (JetPack)j;
                break;
            }
        }
    }*/

    public void LookAt(GameUnit lookTarget)
    {
        lookingAt = lookTarget;
        lookIKWeight = 1;
        bodyWeight = 0.1f;
        headWeight = 1;
        eyesWeight = 0;
        clampWeight = 1;
    }

    public void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (!IsGrounded())
        {
            HandleAirborneMovement();
        }
        /*if (IsGrounded())
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }*/

        ScaleCapsuleForCrouching(bodyState == BodyState.Crouching);
        PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    public void Bullet()
    {
        Gun g = (Gun)owner.itemEquiper.GetFirstItemOfType(typeof(Gun));// owner.itemEquiper.GetFirstGun();

        if (lookingAt != null && g != null)
        {
            
            g.ShootAt(lookingAt);
        }

    }

    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (IsGrounded() && crouch)
        {
            if (bodyState == BodyState.Crouching) return; //(crouching) return;
            this.owner.collider.height = this.owner.collider.height / 2f;
            this.owner.collider.center = this.owner.collider.center / 2f;
            bodyState = BodyState.Crouching;
            //crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(this.owner.rigid.position + Vector3.up * this.owner.collider.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - this.owner.collider.radius * k_Half;
            if (Physics.SphereCast(crouchRay, this.owner.collider.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                bodyState = BodyState.Crouching;
                //crouching = true;
                return;
            }
            this.owner.collider.height = capsuleHeight;
            this.owner.collider.center = capsuleCenter;
            bodyState = BodyState.Standing;
            //crouching = false;
        }
    }

    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (bodyState != BodyState.Crouching)//!crouching)
        {
            Ray crouchRay = new Ray(this.owner.rigid.position + Vector3.up * this.owner.collider.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - this.owner.collider.radius * k_Half;
            if (Physics.SphereCast(crouchRay, this.owner.collider.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                bodyState = BodyState.Crouching;
                //crouching = true;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (lookingAt != null)
        {
            this.owner.animator.SetLookAtWeight(lookIKWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            this.owner.animator.SetLookAtPosition(lookingAt.body.position);
        }
        if (forwardAmount == 0 && turnAmount == 0)
        {
            this.owner.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            this.owner.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            this.owner.animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot.position);
            this.owner.animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot.position);
        }
        else
        {
            this.owner.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            this.owner.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
    public void UpdateAnimatorState()
    {
        this.owner.animator.SetBool("Crouch", bodyState == BodyState.Crouching); //crouching);
        this.owner.animator.SetBool("OnGround", IsGrounded());
        this.owner.animator.SetBool("Rifling", armState == ArmState.Aiming); //rifling);
        this.owner.animator.SetBool("Shooting", gunState == GunState.Shooting); //shooting);

        if (!IsGrounded())
        {

            this.owner.animator.SetFloat("Jump", this.owner.rigid.velocity.y);
        }

    }

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        this.owner.animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        this.owner.animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        UpdateAnimatorState();



        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                this.owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
        if (IsGrounded())
        {
            this.owner.animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (IsGrounded() && move.magnitude > 0)
        {
            this.owner.animator.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            this.owner.animator.speed = 1;
        }
    }

    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        this.owner.rigid.AddForce(extraGravityForce);

        m_GroundCheckDistance = this.owner.rigid.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
    }

    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        /*if (jump && !crouch && this.owner.animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            this.owner.rigid.velocity = new Vector3(this.owner.rigid.velocity.x, m_JumpPower, this.owner.rigid.velocity.z);
            isGrounded = false;
            this.owner.animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }*/
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }
    public void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, stationaryTurnMultiplier * Time.deltaTime);
    }
    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (IsGrounded() && Time.deltaTime > 0)
        {
            Vector3 v = (this.owner.animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = this.owner.rigid.velocity.y;
            this.owner.rigid.velocity = v;
        }
    }
    public bool IsGrounded()
    {
        return !this.owner.navMeshAgent.isOnOffMeshLink;
    }
    private void CheckGroundStatus()
    {
        if (IsGrounded())
        {
            //isGrounded = true;
            this.owner.animator.applyRootMotion = true;
            RaycastHit hitInfo;
            int layer_mask = LayerMask.GetMask(Organizer.LAYER_GROUND);

            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, Mathf.Infinity, layer_mask))
            {
                groundNormal = hitInfo.normal;
                lastWalkedOn = hitInfo.collider.transform;
            }

        }else{
            
            //isGrounded = false;
            groundNormal = Vector3.up;
            this.owner.animator.applyRootMotion = false;
        }
    }
}

