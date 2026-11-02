using UnityEngine;

public class DoorUnlockTrigger : MonoBehaviour
{
    public doorScript puerta;
    public Material materialPrimerGolpe;
    public Material materialSegundoGolpe;
    public int golpesParaDestruir = 3;

    public AudioSource audioSource;
    public AudioClip ClipDestruir;

    public AudioClip ClipGolpear;

    private int golpesRecibidos;
    private bool destruido;
    private int ultimoFrameImpacto = -1;
    private Renderer render;

    private void Awake()
    {
        render = GetComponent<Renderer>();
    }

    public void RecibirGolpeDeProyectil()
    {
        if (destruido)
        {
            return;
        }

        if (Time.frameCount == ultimoFrameImpacto)
        {
            return;
        }

        ultimoFrameImpacto = Time.frameCount;
        golpesRecibidos++;

        if (golpesRecibidos == 1)
        {
            AplicarMaterial(materialPrimerGolpe);
            return;
        }

        if (golpesRecibidos == 2)
        {
            AplicarMaterial(materialSegundoGolpe);
            return;
        }

        if (golpesRecibidos >= golpesParaDestruir)
        {
            Destruir();
        }
    }

    private void AplicarMaterial(Material material)
    {
        if (render == null || material == null)
        {
            return;
        }

        render.material = material;

        audioSource.clip = ClipGolpear;
        audioSource.Play();

    }

    private void Destruir()
    {
        destruido = true;

        if (puerta != null)
        {
            puerta.unlockDoor();
        }

        Destroy(gameObject);

        audioSource.clip = ClipDestruir;
        audioSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Proyectil>() == null)
        {
            return;
        }

        RecibirGolpeDeProyectil();
    }
}
