using UnityEngine;

public class RefractionCube : MonoBehaviour
{
    GameObject m_LineRenderer;
    bool m_CreateRefraction;
    public float m_MaxDistance = 1.5f;
    LayerMask m_CollisionLayerMask;

    void Update()
    {
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);
        m_CreateRefraction=false;
    }
    public void CreateRefraction()
    {
        m_CreateRefraction=true;
        Vector3 l_EndRaycastPosition=Vector3.forward*m_MaxDistance;
        RaycastHit l_RaycastHit;
        if(Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out
        l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
        l_EndRaycastPosition=Vector3.forward*l_RaycastHit.distance;
        if(l_RaycastHit.collider.tag=="RefractionCube")
        {
        //Reflect ray
        l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
        }
        //Other collisions
        }
        // m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
    }    
}
