using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float velocidadProyectil = 60f;
    public float cooldown = 3f;

    [Tooltip("Si es 0, usa radioDeteccion de EnemigoIA mientras persigue o ataca.")]
    public float radioDisparo = 8;

    private EnemigoIA enemigoIA;
    private float ultimoDisparo = -999f;

    private void Awake()
    {
        enemigoIA = GetComponent<EnemigoIA>();
    }

    private void Update()
    {
        if (!PuedeDisparar())
        {
            return;
        }

        MirarAlJugador();
        DispararJugador();
        ultimoDisparo = Time.time;
    }

    private bool PuedeDisparar()
    {
        if (proyectilPrefab == null || puntoDisparo == null || enemigoIA == null)
        {
            return false;
        }

        if (enemigoIA.jugador == null)
        {
            return false;
        }

        EnemigoIA.Estado estado = enemigoIA.estadoActual;
        if (estado != EnemigoIA.Estado.Persecucion && estado != EnemigoIA.Estado.Ataque)
        {
            return false;
        }

        float alcance = radioDisparo > 0f ? radioDisparo : enemigoIA.radioDeteccion;
        float distancia = Vector3.Distance(transform.position, enemigoIA.jugador.position);
        if (distancia > alcance)
        {
            return false;
        }

        return Time.time >= ultimoDisparo + cooldown;
    }

    private void MirarAlJugador()
    {
        Vector3 direccion = enemigoIA.jugador.position - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(direccion);
    }

    private void DispararJugador()
    {
        Vector3 origen = puntoDisparo.position;
        Vector3 direccion = (enemigoIA.jugador.position - origen).normalized;
        if (direccion.sqrMagnitude < 0.0001f)
        {
            direccion = puntoDisparo.forward;
        }

        Quaternion rotacion = Quaternion.LookRotation(direccion);
        GameObject proyectil = Instantiate(proyectilPrefab, origen, rotacion);

        Rigidbody rb = proyectil.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direccion * velocidadProyectil;
        }
    }
}