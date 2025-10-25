using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    Vector3 m_startPosition;
    Quaternion m_startRotation;

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
    // PoolElements m_PoolParticles;

    // [Header("Amount")]
    // public float m_totalAmount = 50;
    // public float m_MaxAmount = 70;
    // public float m_ChargerAmmoCount = 24f;
    // public float m_costAmmoShot = 1f;
    // public float m_initialAmmo;
    // bool isReloading = false;

    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_ReloadKeyCode = KeyCode.R;
    public int m_BlueShootMouseButton = 0;
    public int m_OrangeShootMouseButton = 1;

    [Header("Ddebug Input")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;

    public float m_PortalDistance = 1.5f;
    Vector3 m_MovementDirection;
    public float m_MaxAnglesToTeleport = 75.0f;

    public Portal m_BluePortal;
    public Portal m_OrangePortal;


    // [Header("particles")]
    // public ParticleSystem m_ParticlesHealth;
    // public ParticleSystem m_ParticlesBuff;
    // public ParticleSystem m_ParticlesNextLevel;
    // public ParticleSystem m_ParticlesHit;

    [Header("Score")]
    public float m_score = 0;
    public float m_scoreNecessary = 20;
    public GameObject m_nextLevel;
    void Start()
    {
        // m_initialHealth = m_Health;
        // m_initialShield = m_Shield;

        // m_PoolParticles=new PoolElements();
        // m_PoolParticles.Init(25,m_ShootParticles);

        // m_initialAmmo = m_ChargerAmmoCount;
        // SetIdleAnimation();

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

        m_startPosition = transform.position;
        m_startRotation = transform.rotation;
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

        //------------------------------esto Importante if (CanShoot())
        // {
        //     if (Input.GetMouseButton(m_BlueShootMouseButton))


        // } 
        
        // if (CanReload() && Input.GetKeyDown(m_ReloadKeyCode))
        //     Reload();

        // UIManager.Instance.UiVariables(m_ChargerAmmoCount, m_totalAmount, m_Health, m_Shield);

        // NextLevel();
    }
    // bool CanReload()
    // {
    //     if (m_VerticalSpeed!=0)
    //         return false;

    //     if (Input.GetKey(m_RunKeyCode))
    //         return false;

    //     if (m_ChargerAmmoCount >= m_initialAmmo) //Cargador lleno
    //         return false;

    //     if (m_totalAmount <= 0)
    //         return false;

    //     return true;
    // }
    // void Reload()
    // {
    //     // SetReloadAnimation();

    //     float needed = m_initialAmmo - m_ChargerAmmoCount;

    //     if (needed <= 0)
    //         return; 

    //     if (m_totalAmount >= needed)
    //     {
    //         m_totalAmount -= needed;
    //         m_ChargerAmmoCount += needed;
    //     }
    //     else 
    //     {
    //         m_ChargerAmmoCount += m_totalAmount;
    //         m_totalAmount = 0;
    //     }
    // }
    // bool CanShoot()
    // {
    //     if (m_ChargerAmmoCount<=0 || isReloading)
    //     {
    //         return false;
    //     }
    //     return true;
    // }
    //------------------------------------importante void Shoot()
    // {
    //     // m_ChargerAmmoCount -= m_costAmmoShot;
    //     // SetShootAnimation();
    //     Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
    //     if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_ShootMaxDistance, m_ShootLayerMask.value))
    //     {
    //         if(l_RayCastHit.collider.CompareTag("DrawableWall"))
    //         {
    //             if (m_PortalDistance.IsValidPosition(l_RayCastHit.point, l_RayCastHit.normal))
    //             {
    //                 _Portal.gameObject.SetActiveTrue(true);
    //             }
    //             else
    //                 _Portal.gameObject.SetActive(false);

    //         }
            // if (l_RayCastHit.collider.CompareTag("HitCollider"))
            //     l_RayCastHit.collider.GetComponent<HitCollider>().Hit();
            // else if (l_RayCastHit.collider.CompareTag("Target"))
            // {
            //     m_score += l_RayCastHit.collider.GetComponent<TargetCollider>().Hit();
            //     UIManager.Instance.SumScore(m_score);
            // }
            // else
            //     CreateShootHitParticles(l_RayCastHit.point, l_RayCastHit.normal);
    //     }
    // }
    

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
        // if (other.CompareTag("ShootingGallery"))
        // {
        //     // UIManager.Instance.m_ScoreText.gameObject.SetActive(true);
        // }
        // if (other.CompareTag("NextLevel"))
        // {
        //     // GameManager.GetGameManager().LoadScene();
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
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShootingGallery"))
        {
            // UIManager.Instance.m_ScoreText.gameObject.SetActive(false);
        }
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
    

    // public void NextLevel()
    // {
    //     if (m_nextLevel!=null)
    //     {
    //         if (m_score >= m_scoreNecessary)
    //         {
    //             m_ParticlesNextLevel.Play();
    //             m_nextLevel.SetActive(true);
    //         }
    //         else
    //         {
    //             m_nextLevel.SetActive(false);
    //         }
    //     }
    // }

//     public void TakeDamage(float damage)
//     {
//         if (m_Shield>0)
//         {
//             float shieldDamage = damage * 0.75f;
//             float healthdamage = damage * 0.25f;

//             m_Shield -= Mathf.RoundToInt(shieldDamage);
//             m_Health -= Mathf.RoundToInt(healthdamage);

//             if (m_Shield < 0)  
//                 m_Shield = 0;
//         }
//         else
//         {
//             m_Health -= damage;
//         }

//         if (m_Health <= 0)
//         {
//             m_Health = 0;
//             Kill();
//         }
//         UIManager.Instance.UiVariables(m_ChargerAmmoCount, m_totalAmount, m_Health, m_Shield);

//     }
}
