
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class Blade : MonoBehaviour
{
    public event UnityAction Slice;

    [SerializeField]
    private GameObject trailPrefab;

    private GameObject currentTrail;
    private SphereCollider _collider;

    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.enabled = false;
    }

    public void UpdatePosition(Vector2 position)
    {
        Vector2 vector = Camera.main.ScreenToWorldPoint(position);
        transform.position = vector;
    }

    public void StartCutting()
    {
        currentTrail = Instantiate(trailPrefab, transform,false);
        _collider.enabled = true;
    }

    public void StopCutting()
    {
        currentTrail.transform.SetParent(null);
        Destroy(currentTrail);
        _collider.enabled = false;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    private void OnTriggerEnter(Collider col)
    {
        var fruit = col.GetComponent<Fruit>();
        if (fruit != null)
        {
            Slice?.Invoke();

            Vector2 contactPoint = col.ClosestPoint(_collider.transform.position);

            Vector3 normal = new Vector3(direction.y, -direction.x, 0);


            Destroy(Instantiate(fruit.vfx, col.transform.position, Quaternion.FromToRotation(fruit.vfx.transform.right,direction)),2);
            Cutter.CutGameobject(col.gameObject, contactPoint, normal, true);

            Destroy(col.gameObject);
        }
    }

}
