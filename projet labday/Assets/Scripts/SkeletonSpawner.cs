using System.Collections;
using UnityEngine;

public class SkeletonSpawner : MonoBehaviour
{
    [Header("Paramètres de Spawn")]
    public GameObject skeletonPrefab; // Le prefab de ton squelette
    public Transform player;          // Le Transform de ton joueur
    public float minSpawnRadius = 5f; // Distance minimum d'apparition
    public float maxSpawnRadius = 15f;// Distance maximum d'apparition
    public float timeBetweenSpawns = 4f; // Temps entre chaque spawn

    void Start()
    {
        if (player != null && skeletonPrefab != null)
        {
            StartCoroutine(SpawnRoutine());
        }
        else
        {
            Debug.LogWarning("Il manque le joueur ou le prefab dans le SkeletonSpawner !");
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnSkeleton();
        }
    }

    void SpawnSkeleton()
    {
        // On génère un angle aléatoire
        float angle = Random.Range(0f, Mathf.PI * 2);
        // On génère une distance aléatoire
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // On calcule la position autour du joueur
        Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        Vector3 spawnPosition = player.position + spawnOffset;

        // On garde la même hauteur que le joueur pour éviter qu'ils spawnent en l'air
        spawnPosition.y = player.position.y; 

        Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);
    }
}