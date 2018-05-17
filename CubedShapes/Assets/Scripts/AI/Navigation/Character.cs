using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_GroundCheckDistance = 0.2f;


    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;

    bool m_Crouching;

    // Animation states
    public bool rifling;
    public bool shooting;
    public bool equipped;

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
    private Transform rightHand;
    //Head IK
    private Transform lookingAt;

    // Components
    private ItemEquiper itemEquiper;
    private Rigidbody rigid;
    private Animator anim;
    private CapsuleCollider capsule;
    private Organizer o;

    Transform muzzle;

    public void Awake()
    {
        itemEquiper = GetComponent<ItemEquiper>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        o = Organizer.instance;

        rifling = false;
        shooting = false;

        m_CapsuleHeight = capsule.height;
        m_CapsuleCenter = capsule.center;

        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        rightHand = anim.GetBoneTransform(HumanBodyBones.RightHand);


        Gun rifle = o.GUN_STANDARD_RIFLE.Clone();
        itemEquiper.EquipItem(rifle);
        rifle.Show(anim.GetBoneTransform(HumanBodyBones.RightHand));

        //muzzle = Instantiate(o.E_MUZZLE_FLASHES[0],anim.GetBoneTransform(HumanBodyBones.RightHand));
        //muzzle.parent = rifle.visualItem;
        //muzzle.Rotate(new Vector3(-30, 90, 0));
        //muzzle.position += new Vector3(0.5f,0,0.2f);
       

    }

    public void LookAt(Transform lookTarget)
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

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
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

        
        foreach (Gun g in itemEquiper.equipped.Values)
        {
            
           // muzzle.gameObject.SetActive(false);
           // muzzle.gameObject.SetActive(true);

            g.Shoot();
            
        }
    }


    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (m_IsGrounded && crouch)
        {
            if (m_Crouching) return;
            capsule.height = capsule.height / 2f;
            capsule.center = capsule.center / 2f;
            m_Crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
                return;
            }
            capsule.height = m_CapsuleHeight;
            capsule.center = m_CapsuleCenter;
            m_Crouching = false;
        }
    }

    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!m_Crouching)
        {
            Ray crouchRay = new Ray(rigid.position + Vector3.up * capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
            }
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (lookingAt != null)
        {
            anim.SetLookAtWeight(lookIKWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
            anim.SetLookAtPosition(lookingAt.position);
            // Debug.Log(lookingAt.position);
        }
        if (m_ForwardAmount == 0 && m_TurnAmount == 0)
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
        anim.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Crouch", m_Crouching);
        anim.SetBool("OnGround", m_IsGrounded);
        anim.SetBool("Rifling", rifling);
        anim.SetBool("Shooting", shooting);
        if (!m_IsGrounded)
        {

            anim.SetFloat("Jump", rigid.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        if (m_IsGrounded)
        {
            anim.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
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

        m_GroundCheckDistance = rigid.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }


    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            rigid.velocity = new Vector3(rigid.velocity.x, m_JumpPower, rigid.velocity.z);
            m_IsGrounded = false;
            anim.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }


    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = (anim.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = rigid.velocity.y;
            rigid.velocity = v;
        }
    }


    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            anim.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            anim.applyRootMotion = false;
        }
    }
}

