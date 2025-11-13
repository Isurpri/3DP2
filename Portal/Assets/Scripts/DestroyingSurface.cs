using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DestroyingSurface : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("RefractionCube"))
        {
            other.gameObject.SetActive(false);
        }
        else if(other.CompareTag("Turret"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
