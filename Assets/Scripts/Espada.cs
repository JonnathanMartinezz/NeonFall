using JetBrains.Annotations;
using UnityEngine;

public class Espada : MonoBehaviour
{
    public float danio;

    public bool canDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDamage)
        {
            IEmpujable jugador = other.GetComponent<IEmpujable>();

            if (jugador != null)
            {
                jugador.RestarSalud(danio);
            }
        }
    }

    public void switchDamage()
    {
        canDamage = !canDamage;
    }
}
