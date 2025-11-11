using Unity.VisualScripting;
using UnityEngine;

public class DestroyingSurface : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {        
        if(other.CompareTag("Cube"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
