using UnityEngine;

public class trampolinePlatfform : MonoBehaviour
{
    public float fuerzaImpulso = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (TrampolinMovable.EstaManipulando)
        {
            return;
        }

        IEmpujable jugador = other.GetComponent<IEmpujable>();

        if (jugador != null)
        {
            jugador.AplicarImpulso(fuerzaImpulso);
        }
    }
}
