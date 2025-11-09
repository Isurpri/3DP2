using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RefractionCube : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public float m_MaxDist = 50;
    public LayerMask m_Layer;
    bool m_IsReflectingLaser=false;

    private void Start()
    {
        m_LineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_IsReflectingLaser)
        {
            UpdateLaser();
            m_IsReflectingLaser = false;
        }
        else
            m_LineRenderer.gameObject.SetActive(false);

    }
    public void Reflect()
    {
        if (m_IsReflectingLaser) 
        { 
            return; 
        }

        m_IsReflectingLaser = true;
        UpdateLaser();
       
    }
   public void UpdateLaser()
    {
        m_LineRenderer.gameObject.SetActive(true);

        float l_Distance = m_MaxDist;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_MaxDist, m_Layer.value, QueryTriggerInteraction.Ignore))
        {
            l_Distance = l_RayCastHit.distance;
            if (l_RayCastHit.collider.CompareTag("RefractionCube"))
            {
                l_RayCastHit.collider.GetComponent<RefractionCube>().Reflect();
            }
            if (l_RayCastHit.collider.CompareTag("Player"))
            {
                Debug.Log("Muelto");
            }
        }
        Vector3 l_Position = new Vector3(0.0f, 0.0f, l_Distance);
        m_LineRenderer.SetPosition(1, l_Position);
    }
}
