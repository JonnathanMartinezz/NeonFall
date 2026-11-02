using UnityEngine;

public class doorScript : MonoBehaviour
{
    public Transform movingDoor;

    public Vector3 desplazamiento;
    public float velocidad = 3f;

    public Vector3 posicionInicial;
    public Vector3 posicionObjetivo;

    public bool locked;

    public Material lockedDoorMaterial;
    public Material unLockedDoorMaterial;
    public AudioSource audioSource;
    public AudioClip ClipOpen;
    public AudioClip ClipClose;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicionInicial = transform.position;
        posicionObjetivo = posicionInicial;

        if (locked)
        {
            this.GetComponent<Renderer>().material = lockedDoorMaterial;
        }
        else
        {
            this.GetComponent<Renderer>().material = unLockedDoorMaterial;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidad * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !locked)
        {
            posicionObjetivo = posicionInicial + desplazamiento;
            audioSource.clip = ClipOpen;
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            posicionObjetivo = posicionInicial;
            audioSource.clip = ClipClose;
            audioSource.Play();
        }
    }

    public void unlockDoor()
    {
        locked = false;
        posicionObjetivo = posicionInicial + desplazamiento;
        this.GetComponent<Renderer>().material = unLockedDoorMaterial;
        audioSource.clip = ClipClose;
        audioSource.Play();
    }

    public void RelockDoor()
    {
        locked = true;
        transform.position = posicionInicial;
        posicionObjetivo = posicionInicial;
        this.GetComponent<Renderer>().material = lockedDoorMaterial;
    }
}
