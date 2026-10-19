using UnityEngine;

public class Proyectil : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 0.5f);
    }
}
