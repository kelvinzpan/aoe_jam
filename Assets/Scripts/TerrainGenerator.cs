using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int depth = 20;
    public int width = 256;
    public int height = 256;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public GameObject[] itemsToPickFrom;
    public int numToSpawn = 20;

    void Start ()
    {
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        int randomIndex = Random.Range(0, itemsToPickFrom.Length);

        for (int i = 0; i < numToSpawn; i++)
        {
            float offset = randomIndex < 2 ? 5f : 0.2f;
            SpawnItem(itemsToPickFrom[randomIndex], terrain.terrainData, offset);
            randomIndex = Random.Range(0, itemsToPickFrom.Length);
        }
    }

    TerrainData GenerateTerrain (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights ()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight (int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    void SpawnItem (GameObject Item, TerrainData terrainData, float hOffset)
    {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
        float heightVal = terrainData.GetHeight(x, y) - hOffset;

        Vector3 randPosition = new Vector3(x, heightVal, y);

        GameObject clone = Instantiate(Item, randPosition, Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
    }
}
