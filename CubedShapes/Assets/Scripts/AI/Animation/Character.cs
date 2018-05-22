using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character: MonoBehaviour
{
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
    public bool isGrounded;
    public Transform lastWalkedOn;

    // Animation states
    public bool rifling;
    public bool shooting;
    public bool equipped;
    public bool crouching;

    //Head IK
    private float lookIKWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;
    private float clampWeight;

    //Foot IK
    private Transform leftFoot;
    private Transform rightFoot;
    //Hand IK
    //private Transform rightHand;
    //Head IK
    private GameUnit lookingAt;

    // Components
    public ItemEquiper itemEquiper;
    private Rigidbody rigid;
    private Animator anim;
    private CapsuleCollider capsule;
    public NavMeshAgent navAgent;
    private CharacterLinkMover linkMover;


    public void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        linkMover = this.gameObject.AddComponent<CharacterLinkMover>();
    }

    void Start()
    {
        
        rifling = false;
        shooting = false;

        capsuleHeight = capsule.height;
        capsuleCenter = capsule.center;

        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        origGroundCheckDistance = m_GroundCheckDistance;

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

    }

    public void UpdateWithEquippedItems()
    {
        foreach (Item j in itemEquiper.equipped.Values)
        {
            if (j is JetPack)
            {
                linkMover.characterJetpack = (JetPack)j;
                break;
            }
        }
    }

    public void LookAt(GameUnit lookTarget)
    {
        lookingAt = lookTarget;
        lookIKWeight = 1;
        bodyWeight = 0.1f;
        headWeight = 1;
        eyesWeight = 0;
        clampWeight = 1;
    }

    public void Move(Vector3 move, bool crouch, bool jump)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, groundNormal);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (isGrounded)
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    public void Bullet()
    {
        foreach (Item g in itemEquiper.equipped.Values)
        {
            if (g is Gun)
            {
                if (lookingAt != null)
                {
                    ((Gun)g).ShootAt(lookingAt);
                }
            }
        }
    }

    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (isGrounded && crouch)
        {
            if (crouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                crouching = true;
                return;
            }
            capsule.height = capsuleHeight;
            capsule.center = capsuleCenter;
            crouching = false;
        }
    }

    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!crouching)
        {
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = capsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                crouching = true;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (lookingAt != null)
        {
            anim.SetLookAtWeight(lookIKWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            anim.SetLookAtPosition(lookingAt.body.position);
            // Debug.Log(lookingAt.position);
        }
        if (forwardAmount == 0 && turnAmount == 0)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot.position);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot.position);
        }
        else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
        }
        /*
        if (rifling && equippedWeapon != null)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, equippedWeapon.position);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, equippedWeapon.position);
        }
        else
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }*/
    }

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", crouching);
        anim.SetBool("OnGround", isGrounded);
        anim.SetBool("Rifling", rifling);
        anim.SetBool("Shooting", shooting);
        if (!isGrounded)
        {

            anim.SetFloat("Jump", rigid.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * forwardAmount;
        if (isGrounded)
        {
            anim.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (isGrounded && move.magnitude > 0)
        {
            anim.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            anim.speed = 1;
        }
    }

    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        rigid.AddForce(extraGravityForce);

        m_GroundCheckDistance = rigid.velocity.y < 0 ? origGroundCheckDistance : 0.01f;
    }

    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            rigid.velocity = new Vector3(rigid.velocity.x, m_JumpPower, rigid.velocity.z);
            isGrounded = false;
            anim.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
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
        if (isGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (anim.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = rigid.velocity.y;
            rigid.velocity = v;
        }
    }
    public bool IsGrounded()
    {
        return !navAgent.isOnOffMeshLink;
    }
    private void CheckGroundStatus()
    {
        if (IsGrounded())
        {
            isGrounded = true;
            anim.applyRootMotion = true;
            RaycastHit hitInfo;
            int layer_mask = LayerMask.GetMask(Organizer.LAYER_GROUND);

            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, Mathf.Infinity, layer_mask))
            {
                groundNormal = hitInfo.normal;
                lastWalkedOn = hitInfo.collider.transform;
            }

        }else{
            
            isGrounded = false;
            groundNormal = Vector3.up;
            anim.applyRootMotion = false;
        }
    }
}

