using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuegoArtificial : MonoBehaviour
{
    private Rigidbody2D rb;
    private float angle;
    private float anguloAcumulado = 0;
    [SerializeField] GameObject spriteObject;
    [SerializeField] GameObject particulasChispasObject;
    [SerializeField] GameObject particulasExplosionObject;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(5f, 10f));
        StartCoroutine(triggerExplosion());
    }
    private IEnumerator triggerExplosion()
    {
        yield return new WaitForSeconds(5f);
        particulasChispasObject.GetComponent<ParticleSystem>().Stop();
        particulasExplosionObject.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = rb.velocity.normalized; // Normaliza la dirección del movimiento
        angle = ((Mathf.Atan2(direction.y, direction.x)-90) * Mathf.Rad2Deg); // Calcula el ángulo en grados
        spriteObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, anguloAcumulado + angle)); // Asigna la rotación 2D en función del ángulo calculado
        particulasChispasObject.transform.rotation = Quaternion.Euler(new Vector3(100 + (anguloAcumulado + angle), -90, (anguloAcumulado + angle))); // Asigna la rotación 2D en función del ángulo calculado

    }
}
