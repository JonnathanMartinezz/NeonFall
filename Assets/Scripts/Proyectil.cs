using UnityEngine;

public class Proyectil : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        GameObject objetivo = collision.gameObject;

        if (objetivo.CompareTag("Trigger"))
        {
            DoorUnlockTrigger activador = objetivo.GetComponent<DoorUnlockTrigger>();
            if (activador != null)
            {
                activador.RecibirGolpeDeProyectil();
            }
            else
            {
                Destroy(objetivo);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 0.5f);
    }
}
