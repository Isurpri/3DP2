using UnityEngine;
using UnityEngine.Events;

public class PortalButton : MonoBehaviour
{
    public UnityEvent m_Event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            m_Event.Invoke();
        }
        if (other.CompareTag("Player"))
        {
            m_Event.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            m_Event.Invoke();
        }
        if (other.CompareTag("Player"))
        {
            m_Event.Invoke();
        }
    }
}
