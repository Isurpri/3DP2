using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public float m_NearCameraOffset = 0.5f;
    public List<Transform> m_ValidPosition;

    [Header("Validation")]
    float m_ValidDistanceOffset = 0.15f;
    public LayerMask m_ValidLayerMask;
    public float m_MaxAnglePermitted = 5.0f;

    public Vector3 m_LastValidPos;
    public Quaternion m_LastvalidRot;

    public float m_Sizeportal = 1;

    [Header("Overlap prevention")]
    public float m_PortalOverlapMargin = 0.05f;
    public bool m_CheckOverlapWithOtherPortal = true;

    private void Start()
    {
        gameObject.transform.localScale = new Vector3(m_Sizeportal, m_Sizeportal, m_Sizeportal);
    }

    public void LateUpdate()
    {
        Vector3 l_WorldPosition = Camera.main.transform.position;
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        m_MirrorPortal.m_Camera.transform.position = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldForward = Camera.main.transform.forward;
        Vector3 l_LocalForward = m_OtherPortalTransform.InverseTransformDirection(l_WorldForward);
        m_MirrorPortal.m_Camera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_LocalForward);

        float l_DistanceToPortal = Vector3.Distance(m_MirrorPortal.transform.position, m_MirrorPortal.m_Camera.transform.position);
        m_MirrorPortal.m_Camera.nearClipPlane = l_DistanceToPortal + m_NearCameraOffset;
    }

    public bool IsValidPosition(Vector3 Position, Vector3 Normal)
    {
        gameObject.SetActive(false);
        transform.position = Position;
        transform.rotation = Quaternion.LookRotation(Normal);
        bool l_Valid = true;

        if (m_CheckOverlapWithOtherPortal && m_MirrorPortal != null && m_MirrorPortal.IsActive())
        {
            float myDiameter = m_Sizeportal;
            float otherDiameter = m_MirrorPortal.m_Sizeportal;
            float minDistance = (myDiameter * 0.5f) + (otherDiameter * 0.5f) + m_PortalOverlapMargin;
            float actualDistance = Vector3.Distance(transform.position, m_MirrorPortal.transform.position);
            if (actualDistance < minDistance)
                l_Valid = false;
        }

        if (!l_Valid)
        {
            gameObject.SetActive(false);
            return false;
        }

        Vector3 l_CameraPosition = Camera.main.transform.position;
        for (int i = 0; i < m_ValidPosition.Count; i++)
        {
            Vector3 l_ValidPosition = m_ValidPosition[i].position;
            Vector3 l_Direction = l_ValidPosition - l_CameraPosition;
            float l_Distance = Vector3.Distance(l_ValidPosition, l_CameraPosition);
            l_Direction /= l_Distance;
            Ray l_Ray = new Ray(l_CameraPosition, l_Direction);
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, l_Distance + m_ValidDistanceOffset, m_ValidLayerMask.value, QueryTriggerInteraction.Ignore))
            {
                if (l_RaycastHit.collider.CompareTag("DrawableWall"))
                {
                    if (Vector3.Distance(l_RaycastHit.point, l_ValidPosition) < m_ValidDistanceOffset)
                    {
                        float l_DotAngle = Vector3.Dot(l_RaycastHit.normal, m_ValidPosition[i].forward);
                        if (l_DotAngle < Mathf.Cos(m_MaxAnglePermitted * Mathf.Deg2Rad))
                            l_Valid = true;
                    }
                    else
                        l_Valid = false;
                }
                else
                    l_Valid = false;
            }
            else
                l_Valid = false;

            if (!l_Valid)
                break;
        }

        gameObject.SetActive(false);
        return l_Valid;
    }

    public bool IsActive()
    {
        return this.gameObject.activeSelf;
    }
}
