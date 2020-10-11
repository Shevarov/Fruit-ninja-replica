using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject [] fruits;
    [SerializeField]
    private Transform [] spawnPoins;

    public float minDelay = 0.1f;
    public float maxDelay = 1f;

    void Start()
    {
        StartCoroutine(SpawnFruits());
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);

            yield return new WaitForSeconds(delay);

            int fruitIndex = Random.Range(0, fruits.Length);
            GameObject fruit = fruits[fruitIndex];

            int spawnIndex = Random.Range(0, spawnPoins.Length);
            Transform spawnPoint = spawnPoins[spawnIndex];

            GameObject _fruit = Instantiate(fruit, spawnPoint.position,spawnPoint.rotation);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
