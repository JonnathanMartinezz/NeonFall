using UnityEngine;

public class GestorReinicioNivel : MonoBehaviour
{
    public static GestorReinicioNivel Instancia { get; private set; }

    public doorScript puerta;
    public GameObject prefabTriggerPuerta;
    public TrampolinMovable trampolin;

    [SerializeField] private string nombreTriggerPuerta = "TriggerPuerta";

    private GameObject instanciaTrigger;
    private DatosSpawnTrigger datosTrigger;

    private struct DatosSpawnTrigger
    {
        public Transform padre;
        public Vector3 posicionLocal;
        public Quaternion rotacionLocal;
        public Vector3 escalaLocal;
        public Material materialInicial;
        public Material materialPrimerGolpe;
        public Material materialSegundoGolpe;
        public doorScript puertaEnTrigger;
        public RuntimeAnimatorController controladorAnimacion;
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
    }

    private void Start()
    {
        if (trampolin == null)
        {
            trampolin = FindAnyObjectByType<TrampolinMovable>();
        }

        if (prefabTriggerPuerta == null)
        {
            Debug.LogWarning(
                "GestorReinicioNivel: asigna el prefab Cube en 'Prefab Trigger Puerta' (arrastra Assets/Prefabs/Cube.prefab al campo).",
                this);
        }

        CapturarEstadoInicialTrigger();
    }

    public void ReiniciarNivel()
    {
        if (puerta != null)
        {
            puerta.RelockDoor();
        }

        RestablecerTriggerPuerta();

        if (trampolin != null)
        {
            trampolin.RestablecerPosicionInicial();
        }
    }

    private void CapturarEstadoInicialTrigger()
    {
        instanciaTrigger = GameObject.Find(nombreTriggerPuerta);
        if (instanciaTrigger == null)
        {
            return;
        }

        Transform transformTrigger = instanciaTrigger.transform;
        DoorUnlockTrigger activador = instanciaTrigger.GetComponent<DoorUnlockTrigger>();
        Renderer render = instanciaTrigger.GetComponent<Renderer>();
        Animator animator = instanciaTrigger.GetComponent<Animator>();

        datosTrigger = new DatosSpawnTrigger
        {
            padre = transformTrigger.parent,
            posicionLocal = transformTrigger.localPosition,
            rotacionLocal = transformTrigger.localRotation,
            escalaLocal = transformTrigger.localScale,
            materialInicial = render != null ? render.sharedMaterial : null,
            materialPrimerGolpe = activador != null ? activador.materialPrimerGolpe : null,
            materialSegundoGolpe = activador != null ? activador.materialSegundoGolpe : null,
            puertaEnTrigger = activador != null ? activador.puerta : puerta,
            controladorAnimacion = animator != null ? animator.runtimeAnimatorController : null
        };
    }

    private void RestablecerTriggerPuerta()
    {
        if (datosTrigger.padre == null || prefabTriggerPuerta == null)
        {
            return;
        }

        if (instanciaTrigger != null)
        {
            Destroy(instanciaTrigger);
            instanciaTrigger = null;
        }

        GameObject triggerAnterior = GameObject.Find(nombreTriggerPuerta);
        if (triggerAnterior != null)
        {
            Destroy(triggerAnterior);
        }

        instanciaTrigger = Instantiate(prefabTriggerPuerta, datosTrigger.padre);
        instanciaTrigger.name = nombreTriggerPuerta;
        instanciaTrigger.tag = "Trigger";

        Transform transformTrigger = instanciaTrigger.transform;
        transformTrigger.localPosition = datosTrigger.posicionLocal;
        transformTrigger.localRotation = datosTrigger.rotacionLocal;
        transformTrigger.localScale = datosTrigger.escalaLocal;

        if (datosTrigger.materialInicial != null)
        {
            Renderer render = instanciaTrigger.GetComponent<Renderer>();
            if (render != null)
            {
                render.sharedMaterial = datosTrigger.materialInicial;
            }
        }

        DoorUnlockTrigger activador = instanciaTrigger.GetComponent<DoorUnlockTrigger>();
        if (activador == null)
        {
            activador = instanciaTrigger.AddComponent<DoorUnlockTrigger>();
        }

        activador.puerta = datosTrigger.puertaEnTrigger != null ? datosTrigger.puertaEnTrigger : puerta;
        activador.materialPrimerGolpe = datosTrigger.materialPrimerGolpe;
        activador.materialSegundoGolpe = datosTrigger.materialSegundoGolpe;

        if (datosTrigger.controladorAnimacion != null)
        {
            Animator animator = instanciaTrigger.GetComponent<Animator>();
            if (animator == null)
            {
                animator = instanciaTrigger.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = datosTrigger.controladorAnimacion;
        }
    }
}
