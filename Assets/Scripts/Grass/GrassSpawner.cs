using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform mower;
    public float grassDensity = 20f;
    public float bendAngle = 0f;
    public float heightScale = 0.7f;
    public float widthScale = 1.5f;

    [Header("Area (World Coordinates)")]
    public Transform areaPoint1;
    public Transform areaPoint2;

    private void Start()
    {
        // GrassCutter 찾아서 mower 연결
        var cutter = FindFirstObjectByType<GrassCutter>();
        if (cutter != null)
            cutter.mower = mower;

        SpawnAllGrass();
    }

    // area1 ~ area2 영역에 잔디 생성
    private void SpawnAllGrass()
    {
        float minX = Mathf.Min(areaPoint1.position.x, areaPoint2.position.x);
        float maxX = Mathf.Max(areaPoint1.position.x, areaPoint2.position.x);
        float minZ = Mathf.Min(areaPoint1.position.z, areaPoint2.position.z);
        float maxZ = Mathf.Max(areaPoint1.position.z, areaPoint2.position.z);

        float areaWidth = maxX - minX;
        float areaHeight = maxZ - minZ;
        int grassCount = Mathf.RoundToInt(areaWidth * areaHeight * grassDensity);

        for (int i = 0; i < grassCount; i++)
        {
            float x = Random.Range(minX, maxX);
            float z = Random.Range(minZ, maxZ);
            Vector3 grassPos = new Vector3(x, 0, z);

            float randomBend = bendAngle + Random.Range(-10f, 10f);
            GrassManager.Instance.AddGrass(grassPos, heightScale, widthScale, randomBend);
        }
    }
}
