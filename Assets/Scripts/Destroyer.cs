using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Destroyer : MonoBehaviour
{
    public event UnityAction Health;

    private void OnTriggerEnter(Collider col)
    {
        Fruit fruit = col.GetComponent<Fruit>();
        if (fruit != null)
        {
            Health?.Invoke();
        }
        Destroy(col.gameObject);
    }
}
