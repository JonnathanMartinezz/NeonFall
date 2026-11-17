using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    public float danio = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject objetivo = collision.gameObject;

        if (objetivo.CompareTag("Player"))
        {
            IEmpujable jugador = objetivo.GetComponent<IEmpujable>();
            if (jugador != null)
            {
                jugador.RestarSalud(danio);
            }

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 0.5f);
    }
}