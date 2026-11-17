using UnityEngine;

public class trampolinePlatfform : MonoBehaviour
{
    public float fuerzaImpulso = 15f;

    private void OnTriggerEnter(Collider other)
    {
        IEmpujable jugador = other.GetComponent<IEmpujable>();

        if (jugador != null)
        {
            jugador.AplicarImpulso(fuerzaImpulso);
        }
    }
}