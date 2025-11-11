using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public GameObject m_CompanionCubePrefab;
    public Transform m_SpawnerTransform;
    public GameObject m_CurrentCube;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Spawn();
        }
    }
    void Spawn()
    {
        if (m_CurrentCube != null)
        {
            Destroy(m_CurrentCube);
        }
        m_CurrentCube = GameObject.Instantiate(m_CompanionCubePrefab);
        m_CurrentCube.transform.position = m_SpawnerTransform.position;
        m_CurrentCube.transform.rotation = m_SpawnerTransform.rotation;
    }
}
