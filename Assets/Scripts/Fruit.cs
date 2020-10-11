
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fruit : MonoBehaviour
{
    public float minforce = 15f;
    public float maxforce = 20f;

    public GameObject vfx;

    private void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        //Vector2 velocity = rigidbody2D.velocity;
        //velocity.y = force;

        //rigidbody2D.velocity = velocity;

        float force = Random.Range(minforce, maxforce);

        rigidbody.AddForce(transform.up*force,ForceMode.Impulse);
        transform.Rotate(90,0,0);
    }

    //private void OnTriggerEnter2D(Collider2D col)
    //{
    //    var blade = col.GetComponent<Blade>();
    //    if (blade != null)
    //    {
    //        Collider2D collider = gameObject.GetComponent<Collider2D>();
    //        Vector2 vector2 = collider.ClosestPoint(col.transform.position);

    //        Vector3 direction = blade.GetDirection();

    //        Cutter.CutGameobject(gameObject, vector2, new Vector3(direction.y, -direction.x, 0), true);

    //        Destroy(gameObject);
    //    }
    //}
}
