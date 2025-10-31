using System;
using UnityEngine;

public class CompanionCube : MonoBehaviour
{
    Rigidbody m_rb;
    public float m_PortalDis = 1.5f;
    public float m_MaxAngleToTeleport = 45.0f;
    bool m_AttachedObj = false;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            var l_portal = other.GetComponent<Portal>();
            if(CanTeleport(l_portal))
                Teleport(l_portal);
        }
    }

    bool CanTeleport(Portal _Portal)
    {
        float l_DotValue = Vector3.Dot(_Portal.transform.forward, -m_rb.linearVelocity.normalized);
        return !m_AttachedObj &&  l_DotValue > MathF.Cos(m_MaxAngleToTeleport * Mathf.Deg2Rad);
    }
    void Teleport(Portal _Portal)
    {
        Vector3 l_Direction = m_rb.linearVelocity.normalized;
        Vector3 l_WorldPos = transform.position + l_Direction*m_PortalDis;
        Vector3 l_localPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPos);
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_localPosition);

        Vector3 l_WorldDirection = transform.forward;
        Vector3 l_localDirection = _Portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldDirection);
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_localDirection);

        Vector3 l_localVelocity = _Portal.m_OtherPortalTransform.InverseTransformDirection(m_rb.linearVelocity);
        m_rb.linearVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_localVelocity);
        float l_Scale = _Portal.m_MirrorPortal.transform.localScale.x/_Portal.transform.localScale.x;
        m_rb.transform.localScale = Vector3.one * l_Scale * m_rb.transform.localScale.x;
    }

    public void SetAttachedObj(bool AttachedObj)
    {
        m_AttachedObj = AttachedObj;
    }
}
