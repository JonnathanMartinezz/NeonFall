using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemigoIA : MonoBehaviour
{
    public enum Estado { Patrulla, Persecucion, Ataque}
    public Estado estadoActual = Estado.Patrulla;

    public Transform[] waypoints;
    public Transform jugador;
    public float radioDeteccion = 5f;
    public float radioAtaque = 2.5f;
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;

    private NavMeshAgent agent;
    private Animator animatorEspada;
    public Animator animatorCuerpo;
    private int waypointActual = 0;
    private bool atacando = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorEspada = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);

        switch (estadoActual)
        {
            case Estado.Patrulla:
                agent.speed = velocidadPatrulla;
                Patrullar();
                if (distancia <= radioDeteccion) estadoActual = Estado.Persecucion;
                break;

            case Estado.Persecucion:
                agent.speed = velocidadPersecucion;
                agent.isStopped = false;
                agent.SetDestination(jugador.position);
                if (distancia <= radioAtaque) estadoActual = Estado.Ataque;
                else if (distancia > radioDeteccion * 1.5f) estadoActual = Estado.Patrulla;
                break;

            case Estado.Ataque:
                agent.isStopped = true;
                if (!atacando) StartCoroutine(RealizarAtaque());
                if (distancia > radioAtaque) estadoActual = Estado.Persecucion;
                break;

            default:
                Debug.Log("Es Default");
                break;
        }
    }

    void Patrullar()
    {
        agent.isStopped = false;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waypointActual = (waypointActual + 1) % waypoints.Length;
            agent.SetDestination(waypoints[waypointActual].position);
        }
    }

    IEnumerator RealizarAtaque()
    {
        atacando = true;
        animatorEspada.SetTrigger("Atacar");
        animatorCuerpo.SetTrigger("Atacar");
        yield return new WaitForSeconds(1f);
        atacando = false;
    }
}
