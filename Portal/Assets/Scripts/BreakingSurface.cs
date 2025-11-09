using Unity.VisualScripting;
using UnityEngine;

public class BreakingSurface : MonoBehaviour
{
    private bool m_IsBroken = false;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Cube"))
        {
            Destroy(gameObject);
        }
    }
}
