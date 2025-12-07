using UnityEngine;
using System.Collections.Generic;

public class VegetationSpawner : MonoBehaviour
{
    [System.Serializable]
    public class VegetationCategory
    {
        public string categoryName = "Category";
        public GameObject[] prefabs;
        public int countPerChunk = 10;
        
        [Header("Scale")]
        public float minScale = 0.8f;
        public float maxScale = 1.2f;
        public Vector3 rotationOffset = Vector3.zero;
        
        [Header("Height Constraints")]
        public float minSpawnHeight = 2f;
        public float maxSpawnHeight = 100f;
        public float maxSlope = 35f;
        
        [Header("Spacing")]
        public float minDistanceBetweenSame = 5f;
        public bool checkDistanceWithOtherCategories = true;
        public float minDistanceFromOthers = 3f;
    }
    
    [Header("Categories")]
    public VegetationCategory trees = new VegetationCategory 
    { 
        categoryName = "Arbres",
        countPerChunk = 15,
        minScale = 0.8f,
        maxScale = 1.2f,
        rotationOffset = new Vector3(-90, 0, 0),
        minSpawnHeight = 2f,
        maxSpawnHeight = 100f,
        maxSlope = 35f,
        minDistanceBetweenSame = 8f,
        checkDistanceWithOtherCategories = true,
        minDistanceFromOthers = 5f
    };
    
    public VegetationCategory rocks = new VegetationCategory 
    { 
        categoryName = "Rochers",
        countPerChunk = 10,
        minScale = 0.5f,
        maxScale = 2f,
        rotationOffset = Vector3.zero,
        minSpawnHeight = 1f,
        maxSpawnHeight = 100f,
        maxSlope = 50f,
        minDistanceBetweenSame = 5f,
        checkDistanceWithOtherCategories = true,
        minDistanceFromOthers = 3f
    };
    
    public VegetationCategory vegetation = new VegetationCategory 
    { 
        categoryName = "Végétation",
        countPerChunk = 25,
        minScale = 0.5f,
        maxScale = 1.5f,
        rotationOffset = Vector3.zero,
        minSpawnHeight = 1f,
        maxSpawnHeight = 80f,
        maxSlope = 40f,
        minDistanceBetweenSame = 2f,
        checkDistanceWithOtherCategories = false,
        minDistanceFromOthers = 1f
    };
    
    [Header("Global Settings")]
    public float waterLevel = 0f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    public bool showGizmos = false;
    
    private Dictionary<Vector2, List<GameObject>> spawnedObjects = new Dictionary<Vector2, List<GameObject>>();
    private Dictionary<Vector2, List<Vector3>> debugPositions = new Dictionary<Vector2, List<Vector3>>();

    public void SpawnOnChunk(Vector2 chunkCoord, Mesh terrainMesh, MeshSettings meshSettings, Transform chunkTransform)
    {
        if (spawnedObjects.ContainsKey(chunkCoord))
        {
            foreach (GameObject obj in spawnedObjects[chunkCoord])
            {
                if (obj != null)
                    obj.SetActive(true);
            }
            return;
        }

        if (terrainMesh == null)
        {
            Debug.LogWarning("VegetationSpawner: Pas de mesh terrain!");
            return;
        }

        List<GameObject> allObjects = new List<GameObject>();
        List<Vector3> allPositions = new List<Vector3>();
        List<Vector3> debugPos = new List<Vector3>();
        
        Vector3[] vertices = terrainMesh.vertices;
        Vector3[] normals = terrainMesh.normals;
        
        if (showDebugLogs)
        {
            Debug.Log($"VegetationSpawner: Chunk {chunkCoord} - Début du spawn");
        }
        
        SpawnCategory(trees, "Arbres", chunkCoord, vertices, normals, chunkTransform, allObjects, allPositions, debugPos);
        SpawnCategory(rocks, "Rochers", chunkCoord, vertices, normals, chunkTransform, allObjects, allPositions, debugPos);
        SpawnCategory(vegetation, "Végétation", chunkCoord, vertices, normals, chunkTransform, allObjects, allPositions, debugPos);

        spawnedObjects[chunkCoord] = allObjects;
        debugPositions[chunkCoord] = debugPos;
        
        if (showDebugLogs)
        {
            Debug.Log($"VegetationSpawner: Chunk {chunkCoord} - Total spawned: {allObjects.Count} objets");
        }
    }

    private void SpawnCategory(VegetationCategory category, string categoryName, Vector2 chunkCoord, 
                               Vector3[] vertices, Vector3[] normals, Transform chunkTransform,
                               List<GameObject> allObjects, List<Vector3> allPositions, List<Vector3> debugPos)
    {
        if (category.prefabs == null || category.prefabs.Length == 0)
        {
            if (showDebugLogs)
                Debug.LogWarning($"VegetationSpawner: Aucun prefab pour {categoryName}");
            return;
        }

        int spawned = 0;
        int attempts = 0;
        int maxAttempts = category.countPerChunk * 5;
        
        while (spawned < category.countPerChunk && attempts < maxAttempts)
        {
            attempts++;
            
            int randomIndex = Random.Range(vertices.Length / 10, vertices.Length * 9 / 10);
            Vector3 localPos = vertices[randomIndex];
            Vector3 normal = normals[randomIndex];
            Vector3 worldPos = chunkTransform.TransformPoint(localPos);
            
            float slope = Vector3.Angle(normal, Vector3.up);
            
            if (worldPos.y > waterLevel + category.minSpawnHeight && 
                worldPos.y < category.maxSpawnHeight && 
                slope < category.maxSlope)
            {
                bool validPosition = true;
                
                foreach (Vector3 existingPos in allPositions)
                {
                    float distance = Vector3.Distance(worldPos, existingPos);
                    
                    if (category.checkDistanceWithOtherCategories)
                    {
                        if (distance < category.minDistanceFromOthers)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                    else
                    {
                        if (distance < category.minDistanceBetweenSame)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                
                if (validPosition)
                {
                    GameObject obj = SpawnObject(category, worldPos, normal, chunkTransform);
                    if (obj != null)
                    {
                        allObjects.Add(obj);
                        allPositions.Add(worldPos);
                        debugPos.Add(worldPos);
                        spawned++;
                        
                        if (showDebugLogs && spawned <= 2)
                        {
                            Debug.Log($"{categoryName} {spawned}: Y={worldPos.y:F2}, Slope={slope:F1}°");
                        }
                    }
                }
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"VegetationSpawner: {spawned}/{category.countPerChunk} {categoryName} spawned (tentatives: {attempts})");
        }
    }

    private GameObject SpawnObject(VegetationCategory category, Vector3 position, Vector3 normal, Transform parent)
    {
        GameObject prefab = category.prefabs[Random.Range(0, category.prefabs.Length)];
        
        float randomYRotation = Random.Range(0f, 360f);
        Quaternion finalRotation = Quaternion.Euler(
            category.rotationOffset.x, 
            randomYRotation + category.rotationOffset.y, 
            category.rotationOffset.z
        );
        
        GameObject obj = Instantiate(prefab, position, finalRotation, parent);
        
        float scale = Random.Range(category.minScale, category.maxScale);
        obj.transform.localScale = Vector3.one * scale;

        return obj;
    }

    public void HideOnChunk(Vector2 chunkCoord)
    {
        if (spawnedObjects.ContainsKey(chunkCoord))
        {
            foreach (GameObject obj in spawnedObjects[chunkCoord])
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    public void ClearOnChunk(Vector2 chunkCoord)
    {
        if (spawnedObjects.ContainsKey(chunkCoord))
        {
            foreach (GameObject obj in spawnedObjects[chunkCoord])
            {
                if (obj != null)
                    Destroy(obj);
            }
            spawnedObjects.Remove(chunkCoord);
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
                Gizmos.DrawWireSphere(pos, 1f);
            }
        }
    }
}