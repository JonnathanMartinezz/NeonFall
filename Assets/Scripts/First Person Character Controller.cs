using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class FirstPersonCharacterController : MonoBehaviour, IEmpujable
{
    private Acciones controles;
    private CharacterController characterController;
    public Transform camaraTransform;

    public float velocidad = 5f;
    public float gravedad = -9.81f;
    public float velocidadSalto = 5f;
    public float sensibilidadMouse = 1f;

    public Transform posicionInicial;

    private Vector3 puntoRespawn;
    private Quaternion rotacionRespawn;
    private float ultimoRespawn = -999f;
    private const float CooldownRespawn = 0.25f;

    private Vector3 velocidadVertical;
    private float rotacionVertical = 0f;

    public float suavizadoMouse = 10f;

    private Vector2 mirarSuavizado;

    public float salud = 100;

    public TextMeshProUGUI textoSalud;

    public GameObject panelMensajeNarrativo;
    public TextMeshProUGUI textoMensajeNarrativo;
    public float duracionMensajeNarrativo = 10f;

    [TextArea(2, 5)]
    public string mensajeExtraccion =
        "Con la energía suficiente logras contactar con tu tropa, la cual va camino a tu dirección. Mantente a salvo en espera de tu extracción.";

    private Coroutine ocultarMensajeCoroutine;

    private void Awake()
    {
        controles = new Acciones();
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        controles.Enable();
    }

    private void OnDisable()
    {
        controles.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Picable")
        {
            salud += 10;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("DeathFloor"))
        {
            RestarSalud(50);
            Respawn();
        }

        if (other.name == "ThunderTP")
        {
            MostrarMensajeNarrativo();
            Destroy(other.gameObject);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ConfigurarPuntoRespawn();

        if (panelMensajeNarrativo != null)
        {
            panelMensajeNarrativo.SetActive(false);
        }
    }

    private void MostrarMensajeNarrativo()
    {
        if (panelMensajeNarrativo == null)
        {
            return;
        }

        if (textoMensajeNarrativo == null)
        {
            textoMensajeNarrativo = panelMensajeNarrativo.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (textoMensajeNarrativo != null)
        {
            textoMensajeNarrativo.text = mensajeExtraccion;
        }

        panelMensajeNarrativo.SetActive(true);

        if (ocultarMensajeCoroutine != null)
        {
            StopCoroutine(ocultarMensajeCoroutine);
        }

        ocultarMensajeCoroutine = StartCoroutine(OcultarMensajeNarrativo());
    }

    private IEnumerator OcultarMensajeNarrativo()
    {
        yield return new WaitForSeconds(duracionMensajeNarrativo);
        panelMensajeNarrativo.SetActive(false);
    }

    private void ConfigurarPuntoRespawn()
    {
        if (posicionInicial != null && !posicionInicial.IsChildOf(transform))
        {
            puntoRespawn = posicionInicial.position;
            rotacionRespawn = posicionInicial.rotation;
            return;
        }

        puntoRespawn = transform.position;
        rotacionRespawn = transform.rotation;
    }

    private void Respawn()
    {
        if (Time.time < ultimoRespawn + CooldownRespawn)
        {
            return;
        }

        ultimoRespawn = Time.time;
        velocidadVertical = Vector3.zero;

        characterController.enabled = false;
        transform.SetPositionAndRotation(puntoRespawn, rotacionRespawn);
        characterController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        textoSalud.text = "" + salud;

        bool estaEnSuelo = characterController.isGrounded;

        //Mirar

        Vector2 mirarCrudo = controles.Personaje.Mirar.ReadValue<Vector2>();
        mirarSuavizado = Vector2.Lerp(mirarSuavizado, mirarCrudo, suavizadoMouse * Time.deltaTime);

        transform.Rotate(Vector3.up * mirarSuavizado.x * sensibilidadMouse);

        rotacionVertical -= mirarSuavizado.y * sensibilidadMouse;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -80f, 80f);
        camaraTransform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);


        //Mover

        Vector2 input = controles.Personaje.Caminar.ReadValue<Vector2>();

        Vector3 movimiento = transform.right * input.x + transform.forward * input.y;
        Vector3 movimientoHorizontal = movimiento * velocidad * Time.deltaTime;

        characterController.Move(movimientoHorizontal);

        //Gravedad y Salto

        if (estaEnSuelo && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }

        velocidadVertical.y += gravedad * Time.deltaTime;

        if (estaEnSuelo && controles.Personaje.Saltar.WasPressedThisFrame())
        {
            velocidadVertical.y = velocidadSalto;
        }

        characterController.Move(velocidadVertical * Time.deltaTime);
    }

    public void AplicarImpulso(float fuerza)
    {
        velocidadVertical.y = fuerza;
    }

    public void RestarSalud(float danio)
    {
        salud -= danio;
        EvaluarVida();
    }

    public void EvaluarVida()
    {
        if (salud <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
