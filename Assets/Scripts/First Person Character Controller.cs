using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    private Acciones controles;
    private CharacterController characterController;
    public Transform camaraTransform;

    public float velocidad = 5f;
    public float gravedad = -9.81f;
    public float velocidadSalto = 5f;
    public float sensibilidadMouse = 1f;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

        characterController.Move(movimiento * velocidad * Time.deltaTime);

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
}
