using UnityEngine;

public class PivotEspada : MonoBehaviour
{
    public Espada espada;

    public void llamarEspadaDamage()
    {
        espada.switchDamage();
    }
}
