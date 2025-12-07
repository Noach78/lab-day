using UnityEngine;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject[] treePrefabs;
    public int treesPerChunk = 15;
    public float minTreeScale = 0.8f;
    public float maxTreeScale = 1.2f;
    public Vector3 treePrefabRotationOffset = new Vector3(-90, 0, 0); // Ajustez selon vos prefabs
    
    [Header("Height Constraints")]
    public float waterLevel = 0f;
    public float minSpawnHeight = 2f;
    public float maxSpawnHeight = 100f;
    public float maxSlope = 35f;
    
    [Header("Spacing")]
    public float minDistanceBetweenTrees = 8f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    public bool showGizmos = true;
    
    private Dictionary<Vector2, List<GameObject>> spawnedTrees = new Dictionary<Vector2, List<GameObject>>();
    private Dictionary<Vector2, List<Vector3>> debugPositions = new Dictionary<Vector2, List<Vector3>>();

    // CETTE MÉTHODE DOIT RECEVOIR LA MESH POUR LIRE LES VRAIES POSITIONS
    public void SpawnTreesOnChunk(Vector2 chunkCoord, Mesh terrainMesh, MeshSettings meshSettings, Transform chunkTransform)
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("TreeSpawner: Aucun prefab d'arbre assigné!");
            return;
        }

        if (terrainMesh == null)
        {
            Debug.LogWarning("TreeSpawner: Pas de mesh terrain!");
            return;
        }

        // Éviter de respawn
        if (spawnedTrees.ContainsKey(chunkCoord))
        {
            foreach (GameObject tree in spawnedTrees[chunkCoord])
            {
                if (tree != null)
                    tree.SetActive(true);
            }
            return;
        }

        List<GameObject> treesInChunk = new List<GameObject>();
        List<Vector3> treePositions = new List<Vector3>();
        List<Vector3> debugPos = new List<Vector3>();
        
        // Récupérer les vertices du mesh (qui ont déjà les bonnes hauteurs!)
        Vector3[] vertices = terrainMesh.vertices;
        Vector3[] normals = terrainMesh.normals;
        
        if (showDebugLogs)
        {
            Debug.Log($"TreeSpawner: Chunk {chunkCoord}, Vertices count: {vertices.Length}");
        }
        
        int treesSpawned = 0;
        int attempts = 0;
        int maxAttempts = treesPerChunk * 5;
        
        while (treesSpawned < treesPerChunk && attempts < maxAttempts)
        {
            attempts++;
            
            // Choisir un vertex aléatoire (éviter les bords)
            int randomIndex = Random.Range(vertices.Length / 10, vertices.Length * 9 / 10);
            
            // Position locale du vertex
            Vector3 localPos = vertices[randomIndex];
            Vector3 normal = normals[randomIndex];
            
            // Position mondiale
            Vector3 worldPos = chunkTransform.TransformPoint(localPos);
            
            // Calculer la pente
            float slope = Vector3.Angle(normal, Vector3.up);
            
            // Vérifier les conditions
            if (worldPos.y > waterLevel + minSpawnHeight && 
                worldPos.y < maxSpawnHeight && 
                slope < maxSlope)
            {
                // Vérifier la distance avec les autres arbres
                bool tooClose = false;
                foreach (Vector3 existingPos in treePositions)
                {
                    if (Vector3.Distance(worldPos, existingPos) < minDistanceBetweenTrees)
                    {
                        tooClose = true;
                        break;
                    }
                }
                
                if (!tooClose)
                {
                    // Spawner l'arbre
                    GameObject tree = SpawnTree(worldPos, normal, chunkTransform);
                    if (tree != null)
                    {
                        treesInChunk.Add(tree);
                        treePositions.Add(worldPos);
                        debugPos.Add(worldPos);
                        treesSpawned++;
                        
                        if (showDebugLogs && treesSpawned <= 3)
                        {
                            Debug.Log($"Arbre {treesSpawned}: WorldPos={worldPos} (Y={worldPos.y:F2}), Slope={slope:F1}°");
                        }
                    }
                }
            }
        }

        spawnedTrees[chunkCoord] = treesInChunk;
        debugPositions[chunkCoord] = debugPos;
        
        if (showDebugLogs)
        {
            Debug.Log($"TreeSpawner: {treesSpawned} arbres spawned sur chunk {chunkCoord} (tentatives: {attempts})");
        }
    }

    private GameObject SpawnTree(Vector3 position, Vector3 normal, Transform parent)
    {
        GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        
        // Rotation aléatoire autour de Y d'abord
        float randomYRotation = Random.Range(0f, 360f);
        
        // Créer la rotation combinée : offset du prefab + rotation aléatoire Y
        Quaternion finalRotation = Quaternion.Euler(treePrefabRotationOffset.x, randomYRotation + treePrefabRotationOffset.y, treePrefabRotationOffset.z);
        
        GameObject tree = Instantiate(treePrefab, position, finalRotation, parent);
        
        // Scale aléatoire
        float scale = Random.Range(minTreeScale, maxTreeScale);
        tree.transform.localScale = Vector3.one * scale;

        return tree;
    }

    public void HideTreesOnChunk(Vector2 chunkCoord)
    {
        if (spawnedTrees.ContainsKey(chunkCoord))
        {
            foreach (GameObject tree in spawnedTrees[chunkCoord])
            {
                if (tree != null)
                    tree.SetActive(false);
            }
        }
    }

    public void ClearTreesOnChunk(Vector2 chunkCoord)
    {
        if (spawnedTrees.ContainsKey(chunkCoord))
        {
            foreach (GameObject tree in spawnedTrees[chunkCoord])
            {
                if (tree != null)
                    Destroy(tree);
            }
            spawnedTrees.Remove(chunkCoord);
        }
        
        if (debugPositions.ContainsKey(chunkCoord))
        {
            debugPositions.Remove(chunkCoord);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.green;
        foreach (var kvp in debugPositions)
        {
            foreach (Vector3 pos in kvp.Value)
            {
                Gizmos.DrawSphere(pos, 1f);
            }
        }
    }
}