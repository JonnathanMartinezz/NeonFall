using Unity.Mathematics;
using UnityEngine;

public class leverScript : MonoBehaviour
{
    public GameObject bisagraPalanca;
    public GameObject doorToOpen;

    public bool playerNear;

    private Acciones controles;

    public quaternion rotacionInicial;
    public quaternion rotacionObjetivo;

    public float velocidadRotacion = 3f;

    private void Awake()
    {
        controles = new Acciones();
    }

    void OnEnable()
    {
        controles.Enable();
    }

    void OnDisable()
    {
        controles.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotacionInicial = bisagraPalanca.transform.rotation;
        rotacionObjetivo = rotacionInicial;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNear && controles.Personaje.Interactuar.WasPressedThisFrame())
        {
            activarPalanca();
        }
        bisagraPalanca.transform.rotation = Quaternion.Slerp(bisagraPalanca.transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    public void activarPalanca()
    {
        rotacionObjetivo = rotacionInicial * Quaternion.Euler(30, 0, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}
