using UnityEngine;

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
        if (other.CompareTag("DeathFloor"))
        {
            Respawn();
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ConfigurarPuntoRespawn();
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

        if (GestorReinicioNivel.Instancia != null)
        {
            GestorReinicioNivel.Instancia.ReiniciarNivel();
        }

        characterController.enabled = false;
        transform.SetPositionAndRotation(puntoRespawn, rotacionRespawn);
        characterController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
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

        bool interactuarPresionado = controles.Personaje.Interactuar.IsPressed();
        TrampolinMovable.ActualizarEstadoManipulacion(transform, interactuarPresionado);

        if (estaEnSuelo && controles.Personaje.Saltar.WasPressedThisFrame() && !TrampolinMovable.EstaManipulando)
        {
            velocidadVertical.y = velocidadSalto;
        }

        characterController.Move(velocidadVertical * Time.deltaTime);

        TrampolinMovable.NotificarMovimientoJugador(transform, movimientoHorizontal, interactuarPresionado);
    }

    public void AplicarImpulso(float fuerza)
    {
        if (TrampolinMovable.EstaManipulando)
        {
            return;
        }

        velocidadVertical.y = fuerza;
    }
}
