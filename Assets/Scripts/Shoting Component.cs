using System.Runtime.CompilerServices;
using UnityEngine;

public class ShotingComponent : MonoBehaviour
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float velocidadProyectil = 60f;
    public float cooldown = 0.3f;

    private Acciones controles;
    private float ultimoDisparo = -999f;

    void Awake()
    {
        controles = new Acciones();
    }

    void OnEnable() => controles.Enable();
    void OnDisable() => controles.Disable();

    // Update is called once per frame
    void Update()
    {
        if (controles.Personaje.Disparo.WasPressedThisFrame() && Time.time >= ultimoDisparo + cooldown)
        {
            Disparar();
            ultimoDisparo = Time.time;
        }
    }

    void Disparar()
    {
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
        Rigidbody rb = proyectil.GetComponent<Rigidbody>();
        rb.linearVelocity = puntoDisparo.forward * velocidadProyectil;
    }
}
