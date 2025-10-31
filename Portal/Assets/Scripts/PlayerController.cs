using System;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;
    public Transform m_PitchController;
    public bool m_UseInvertedYaw;
    public bool m_UseInvertedPitch;
    public CharacterController m_CharacterController;
    private float m_VerticalSpeed = 0.0f;

    public bool m_AngleLocked = false;
    public float m_Speed;
    public float m_Jumpspeed;
    public float m_SpeedMultiplier;

    public Camera m_Camera;

    [Header("Shoot")]
    public float m_ShootMaxDistance = 50.0f;
    public LayerMask m_ShootLayerMask;
    // public GameObject m_ShootParticles;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public KeyCode m_GrabKeyCode = KeyCode.E;
    public int m_BlueShootMouseButton = 0;
    public int m_OrangeShootMouseButton =1;

    [Header("Ddebug Input")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    [Header("Teleport")]
    public float m_PortalDistance = 1.5f;
    Vector3 m_MovementDirection;
    public float m_MaxAnglesToTeleport = 75.0f;

    [Header("Portal")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;


    [Header("AttachObject")]
    public ForceMode m_ForceMode;
    public float m_ThrowForce = 10.0f;
    public Transform m_GripTrans;
    Rigidbody m_AttachedObjectRb;
    bool m_AttachingObject;
    Vector3 m_StartAttachObjectPos;
    float m_AttachingCurrentTime;
    public float m_AttachingTime = 1.5f;
    public float m_AttachObjectRotationDistLerp = 2.0f;
    bool m_attachedObject;
    public LayerMask m_ValidAttachObjectsLayerMask;

    // [Header("particles")]
    // public ParticleSystem m_ParticlesHealth;
    // public ParticleSystem m_ParticlesBuff;
    // public ParticleSystem m_ParticlesNextLevel;
    // public ParticleSystem m_ParticlesHit;

   
    void Start()
    {

        // // Espera a que el GameManager exista
        // if (GameManager.GetGameManager() == null)
        // {
        //     StartCoroutine(WaitForGameManager());
        //     return;
        // }

        // var l_Player = GameManager.GetGameManager().GetPlayer();
        // if (l_Player != null)
        // {
        //     l_Player.m_CharacterController.enabled = false;
        //     l_Player.transform.position = transform.position;
        //     l_Player.transform.rotation = transform.rotation;
        //     l_Player.m_CharacterController.enabled = true;
        //     l_Player.m_startPosition = transform.position;
        //     l_Player.m_startRotation = transform.rotation;
        //     Destroy(gameObject);
        //     return;
        // }

        DontDestroyOnLoad(gameObject);
        // GameManager.GetGameManager().SetPlayer(this);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // IEnumerator WaitForGameManager()
    // {
    //     yield return new WaitUntil(() => GameManager.GetGameManager() != null);
    //     Start(); 
    // }
    void Update()
    {
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime * (m_UseInvertedYaw ? -1.0f : 1.0f);
            m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime * (m_UseInvertedPitch ? -1.0f : 1.0f);

            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        }

        Vector3 l_Movement = Vector3.zero;
        float l_YawPiRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90PiRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_ForwardDirection = new Vector3(Mathf.Sin(l_YawPiRadians), 0.0f, Mathf.Cos(l_YawPiRadians));
        Vector3 l_RightDirection = new Vector3(Mathf.Sin(l_Yaw90PiRadians), 0.0f, Mathf.Cos(l_Yaw90PiRadians));


        if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_RightDirection;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Movement = -l_RightDirection;

        if (Input.GetKey(m_UpKeyCode))
            l_Movement = l_ForwardDirection;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement = -l_ForwardDirection;

        float l_SpeedMultiplier = 1.0f;

        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_SpeedMultiplier;

        l_Movement.Normalize();
        m_MovementDirection = l_Movement;
        l_Movement *= m_Speed * l_SpeedMultiplier * Time.deltaTime;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;


        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if (m_VerticalSpeed < 0.0f && (l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            if (Input.GetKeyDown(m_JumpKeyCode))
                m_VerticalSpeed = m_Jumpspeed;
        }
        else if (m_VerticalSpeed > 0.0f && (l_CollisionFlags & CollisionFlags.Above) != 0)
            m_VerticalSpeed = 0.0f;

        if (CanShoot())
        {
            if (Input.GetMouseButtonDown(m_BlueShootMouseButton))
                Shoot(m_BluePortal);
            else if (Input.GetMouseButtonDown(m_OrangeShootMouseButton))
                Shoot(m_OrangePortal);
        }
        if (CanAttachObject())
        {
            AttachObject();
        }

        if (m_AttachedObjectRb!=null)
        {
            UpdateAttachedObject();
        }
        // if (CanReload() && Input.GetKeyDown(m_ReloadKeyCode))
        //     Reload();

        // UIManager.Instance.UiVariables(m_ChargerAmmoCount, m_totalAmount, m_Health, m_Shield);

        // NextLevel();
    }

    bool CanAttachObject()
    {
        return true;
    }

    bool CanShoot()
    {
        return m_AttachedObjectRb == null;
    }
    void Shoot(Portal _Portal)
    {
        //SetShootAnimation();
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_ShootMaxDistance,_Portal.m_ValidLayerMask.value,QueryTriggerInteraction.Ignore))
        {
            if (l_RayCastHit.collider.CompareTag("DrawableWall"))
            {
                if (_Portal.IsValidPosition(l_RayCastHit.point, l_RayCastHit.normal))
                {
                    _Portal.gameObject.SetActive(true);
                }
                else
                    _Portal.gameObject.SetActive(false);

            }
                
            // if (l_RayCastHit.collider.CompareTag("HitCollider"))
            //     l_RayCastHit.collider.GetComponent<HitCollider>().Hit();
            // else if (l_RayCastHit.collider.CompareTag("Target"))
            // {
            //     m_score += l_RayCastHit.collider.GetComponent<TargetCollider>().Hit();
            //     UIManager.Instance.SumScore(m_score);
            // }
            // else
            //     CreateShootHitParticles(l_RayCastHit.point, l_RayCastHit.normal);
        }
    }
    

    void SetIdleAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimationClip.name);
    }
    

    void SetShootAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimationClip.name, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (CanTeleport(l_Portal))
                Teleport(other.GetComponent<Portal>());
        }
        // if (other.CompareTag("Button"))
        // {
        //     m_score = 0;
        //     // UIManager.Instance.SumScore(m_score);
        // }
        // else if (other.CompareTag("DeadZone"))
        // {
        //     // Kill();
        // }
       
    }
    bool CanTeleport(Portal _Portal)
    {
        float l_DotValue = Vector3.Dot(_Portal.transform.forward, -m_MovementDirection);
        return l_DotValue > MathF.Cos(m_MaxAnglesToTeleport * Mathf.Deg2Rad);
    }
    void Teleport(Portal _Portal)
    {
        Vector3 l_NextPosition = transform.position + m_MovementDirection * m_PortalDistance;
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_NextPosition);
        Vector3 l_WorldPosition = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldForward = transform.forward;
        Vector3 l_LocalForward = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);
        l_WorldForward = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);

        m_CharacterController.enabled = false;
        transform.position = l_WorldPosition;
        transform.rotation = Quaternion.LookRotation(l_WorldForward);
        m_Yaw = transform.rotation.eulerAngles.y;
        m_CharacterController.enabled = true;
    }
    


    void AttachObject()
    {
        if (Input.GetKeyDown(m_GrabKeyCode))
        {
            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_ShootMaxDistance, m_ValidAttachObjectsLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if (l_RayCastHit.collider.CompareTag("Cube"))
                {
                    AttachObject(l_RayCastHit.rigidbody);
                }
            }
        }
    }

    void AttachObject(Rigidbody _Rb)
    {
        m_AttachingObject = true;
        m_AttachedObjectRb = _Rb;
        m_AttachedObjectRb.GetComponent<CompanionCube>().SetAttachedObj(true);
        m_StartAttachObjectPos = _Rb.transform.position;
        m_AttachingCurrentTime = 0.0f;
        m_attachedObject = false;
    }
    void UpdateAttachedObject()
    {
        if (m_AttachingObject)
        {
            m_AttachingCurrentTime += Time.deltaTime;
            float l_Pct=MathF.Min(1.0f, m_AttachingCurrentTime/m_AttachingTime);
            Vector3 l_Position = Vector3.Lerp(m_StartAttachObjectPos, m_GripTrans.position, l_Pct);
            float l_Distance = Vector3.Distance(l_Position,m_GripTrans.position);
            float l_RotationPct =1.0f - MathF.Min(1.0f,l_Distance/m_AttachObjectRotationDistLerp);
            Quaternion l_Rotation = Quaternion.Lerp(transform.rotation, m_GripTrans.rotation, l_RotationPct);
            m_AttachedObjectRb.MovePosition(l_Position);
            m_AttachedObjectRb.MoveRotation(l_Rotation);
            if (l_Pct==1.0f)
            {
                m_AttachingObject = false;
                m_attachedObject = true;
                m_AttachedObjectRb.transform.SetParent(m_GripTrans);
                m_AttachedObjectRb.transform.localPosition = Vector3.zero;
                m_AttachedObjectRb.transform.localRotation = Quaternion.identity;
                m_AttachedObjectRb.isKinematic = true;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            ThrowObject(m_ThrowForce);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ThrowObject(0.0f);
        }
    }
    void ThrowObject(float force) 
    {
        m_AttachedObjectRb.isKinematic = false;
        m_AttachedObjectRb.AddForce(m_PitchController.forward * force, m_ForceMode);
        m_AttachedObjectRb.transform.SetParent(null);
        m_attachedObject=false;
        m_AttachingObject=false;
        m_AttachedObjectRb.GetComponent<CompanionCube>().SetAttachedObj(false);
        m_AttachedObjectRb = null;
    }
    // void Kill()
    // {
    //     GameManager.GetGameManager().m_fade.FadeIn(() =>
    //     {
    //         GameManager.GetGameManager().RestartLevel();
    //     });
    // }

    /*public void Restart() 
    { 
        m_CharacterController.enabled = false; 
        transform.position = m_startPosition; 
        transform.rotation = m_startRotation; 
        m_CharacterController.enabled = true; 
    }*/
    

 
}
