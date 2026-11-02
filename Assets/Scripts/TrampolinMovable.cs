using System.Collections.Generic;
using UnityEngine;

public class TrampolinMovable : MonoBehaviour
{
    public static bool EstaManipulando { get; private set; }

    private static readonly List<TrampolinMovable> Instancias = new List<TrampolinMovable>();

    public Collider limitePiso;
    public float distanciaMaxima = 4f;
    public float distanciaMirada = 8f;
    public float anguloMiradaMaximo = 45f;

    private Collider colisionSolido;
    private float alturaFija;
    private Vector3 posicionInicialMundo;
    private float radioPlataforma = 1f;
    private Collider[] collidersTrampolin;

    private void Awake()
    {
        collidersTrampolin = GetComponents<Collider>();
        posicionInicialMundo = transform.position;
        alturaFija = transform.position.y;

        CharacterController ccEnTrampolin = GetComponent<CharacterController>();
        if (ccEnTrampolin != null)
        {
            Destroy(ccEnTrampolin);
        }

        ConfigurarColisionSolido();
        CalcularRadioPlataforma();
        ResolverLimitePiso();
    }

    private void OnEnable()
    {
        if (!Instancias.Contains(this))
        {
            Instancias.Add(this);
        }
    }

    private void OnDisable()
    {
        Instancias.Remove(this);
        if (Instancias.Count == 0)
        {
            EstaManipulando = false;
        }
    }

    public static void NotificarMovimientoJugador(Transform jugador, Vector3 deltaIntencionalHorizontal, bool interactuarPresionado)
    {
        foreach (TrampolinMovable trampolin in Instancias)
        {
            trampolin.ProcesarArrastre(jugador, deltaIntencionalHorizontal, interactuarPresionado);
        }
    }

    public static void ActualizarEstadoManipulacion(Transform jugador, bool interactuarPresionado)
    {
        bool algunoManipula = false;
        foreach (TrampolinMovable trampolin in Instancias)
        {
            if (trampolin.EstaEnModoManipulacion(jugador, interactuarPresionado))
            {
                algunoManipula = true;
            }
        }

        EstaManipulando = algunoManipula;
    }

    private void ProcesarArrastre(Transform jugadorActual, Vector3 deltaIntencionalHorizontal, bool interactuarPresionado)
    {
        if (deltaIntencionalHorizontal.sqrMagnitude <= 0f)
        {
            return;
        }

        if (!EstaEnModoManipulacion(jugadorActual, interactuarPresionado))
        {
            return;
        }

        AplicarMovimiento(deltaIntencionalHorizontal);
    }

    private bool EstaEnModoManipulacion(Transform jugadorActual, bool interactuarPresionado)
    {
        if (jugadorActual == null || !interactuarPresionado)
        {
            return false;
        }

        if (!EstaDentroDeDistancia(jugadorActual))
        {
            return false;
        }

        return EstaMirandoAlTrampolin(jugadorActual);
    }

    private bool EstaDentroDeDistancia(Transform jugadorActual)
    {
        return DistanciaHorizontal(jugadorActual.position, transform.position) <= distanciaMaxima;
    }

    private bool EstaMirandoAlTrampolin(Transform jugadorActual)
    {
        Transform camaraJugador = ObtenerCamara(jugadorActual);
        if (camaraJugador == null)
        {
            return false;
        }

        Vector3 puntoTrampolin = transform.position + Vector3.up * 0.2f;
        Vector3 direccionAlTrampolin = puntoTrampolin - camaraJugador.position;
        if (direccionAlTrampolin.sqrMagnitude < 0.0001f)
        {
            return true;
        }

        float distancia = direccionAlTrampolin.magnitude;
        if (distancia > distanciaMirada)
        {
            return false;
        }

        if (Vector3.Angle(camaraJugador.forward, direccionAlTrampolin) > anguloMiradaMaximo)
        {
            return false;
        }

        if (DistanciaHorizontal(jugadorActual.position, transform.position) <= radioPlataforma * 1.5f)
        {
            return true;
        }

        RaycastHit[] impactos = Physics.RaycastAll(
            camaraJugador.position,
            direccionAlTrampolin.normalized,
            distancia,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        System.Array.Sort(impactos, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit impacto in impactos)
        {
            if (impacto.collider.CompareTag("Player"))
            {
                continue;
            }

            if (EsColliderDelTrampolin(impacto.collider) || impacto.transform.IsChildOf(transform))
            {
                return true;
            }

            if (!impacto.collider.isTrigger)
            {
                return false;
            }
        }

        return true;
    }

    private bool EsColliderDelTrampolin(Collider col)
    {
        foreach (Collider colliderTrampolin in collidersTrampolin)
        {
            if (col == colliderTrampolin)
            {
                return true;
            }
        }

        return col.transform.IsChildOf(transform);
    }

    private Transform ObtenerCamara(Transform jugadorActual)
    {
        FirstPersonCharacterController fps = jugadorActual.GetComponent<FirstPersonCharacterController>();
        if (fps != null && fps.camaraTransform != null)
        {
            return fps.camaraTransform;
        }

        Camera camara = jugadorActual.GetComponentInChildren<Camera>();
        return camara != null ? camara.transform : null;
    }

    private void AplicarMovimiento(Vector3 delta)
    {
        Vector3 destino = transform.position + delta;
        destino.y = alturaFija;

        if (colisionSolido != null)
        {
            Vector3 halfExtents = colisionSolido.bounds.extents;
            halfExtents.y = Mathf.Max(halfExtents.y, 0.05f);

            if (Physics.BoxCast(
                transform.position,
                halfExtents * 0.95f,
                delta.normalized,
                out RaycastHit impacto,
                transform.rotation,
                delta.magnitude,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            {
                if (!EsColliderDelTrampolin(impacto.collider) && !impacto.transform.IsChildOf(transform))
                {
                    destino = transform.position + Vector3.ProjectOnPlane(delta, impacto.normal);
                    destino.y = alturaFija;
                }
            }
        }

        transform.position = destino;
        AjustarDentroDeLimites();
    }

    private void AjustarDentroDeLimites()
    {
        if (limitePiso == null)
        {
            return;
        }

        Vector3 posicion = transform.position;
        posicion.y = alturaFija;

        Bounds limites = limitePiso.bounds;
        float margen = Mathf.Min(radioPlataforma * 0.5f, 0.75f);
        float minX = limites.min.x + margen;
        float maxX = limites.max.x - margen;
        float minZ = limites.min.z + margen;
        float maxZ = limites.max.z - margen;

        if (minX >= maxX || minZ >= maxZ)
        {
            return;
        }

        posicion.x = Mathf.Clamp(posicion.x, minX, maxX);
        posicion.z = Mathf.Clamp(posicion.z, minZ, maxZ);
        transform.position = posicion;
    }

    private void ConfigurarColisionSolido()
    {
        foreach (Collider col in collidersTrampolin)
        {
            if (!col.isTrigger)
            {
                colisionSolido = col;
                return;
            }
        }

        if (!TryGetComponent(out Renderer renderer))
        {
            return;
        }

        BoxCollider solido = gameObject.AddComponent<BoxCollider>();
        solido.isTrigger = false;
        solido.center = renderer.localBounds.center;
        solido.size = renderer.localBounds.size;
        solido.size = new Vector3(solido.size.x, Mathf.Max(solido.size.y, 0.2f), solido.size.z);
        colisionSolido = solido;
        collidersTrampolin = GetComponents<Collider>();
    }

    private void CalcularRadioPlataforma()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            radioPlataforma = Mathf.Max(renderer.bounds.extents.x, renderer.bounds.extents.z);
            return;
        }

        if (colisionSolido != null)
        {
            radioPlataforma = Mathf.Max(colisionSolido.bounds.extents.x, colisionSolido.bounds.extents.z);
        }
    }

    private void ResolverLimitePiso()
    {
        if (limitePiso != null)
        {
            return;
        }

        Transform busqueda = transform.parent;
        while (busqueda != null)
        {
            Collider mejorCandidato = null;
            foreach (Collider col in busqueda.GetComponentsInChildren<Collider>())
            {
                if (col.isTrigger || !col.gameObject.name.StartsWith("Piso"))
                {
                    continue;
                }

                Vector3 prueba = new Vector3(transform.position.x, col.bounds.center.y, transform.position.z);
                if (col.bounds.Contains(prueba))
                {
                    limitePiso = col;
                    return;
                }

                mejorCandidato ??= col;
            }

            if (mejorCandidato != null)
            {
                limitePiso = mejorCandidato;
                return;
            }

            busqueda = busqueda.parent;
        }
    }

    public void RestablecerPosicionInicial()
    {
        alturaFija = posicionInicialMundo.y;
        transform.position = posicionInicialMundo;
    }

    private static float DistanciaHorizontal(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
