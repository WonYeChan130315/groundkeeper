using UnityEngine;
using System.Collections.Generic;

public class GrassManager : MonoBehaviour
{
    public static GrassManager Instance { get; private set; }

    [Header("Grass Settings")]
    private Mesh grassMesh;
    public Material grassMaterial;
    public int maxGrassCount = 500000;
    public Transform mower;
    public float cutHeightScale = 0.3f;

    [Header("Rendering")]
    public int instancesPerBatch = 1023;

    public List<GrassData> grassList = new List<GrassData>();
    private Matrix4x4[] batchMatrices;

    private void Awake()
    {
        Instance = this;
        batchMatrices = new Matrix4x4[instancesPerBatch];

        grassMesh = GrassMeshGenerator.Create();
    }

    public void AddGrass(Vector3 position, float heightScale = 1f, float widthScale = 1f, float bendAngle = 0f)
    {
        if (grassList.Count >= maxGrassCount) return;

        float randomRotY = Random.Range(0f, 360f);
        float randomScale = Random.Range(0.8f, 1.2f);
        float randomHeight = Random.Range(0.8f, 1.2f);

        GrassData data = new GrassData
        {
            position = position,
            baseAngle = bendAngle,
            currentAngle = bendAngle,
            targetAngle = bendAngle,
            heightScale = heightScale,
            widthScale = widthScale,
            randomRotY = randomRotY,
            randomScale = randomScale,
            randomHeight = randomHeight,
            isCut = false,
            mowerAngle = 0f,
            cutTime = 0f
        };

        grassList.Add(data);
    }

    public void Clear()
    {
        grassList.Clear();
    }

    private void LateUpdate()
    {
        if (grassMaterial == null) return;

        Vector3 mowerPos = mower != null ? mower.position : Vector3.zero;

        int batchIndex = 0;

        for (int i = 0; i < grassList.Count; i++)
        {
            var grass = grassList[i];

            float effectiveHeightScale = grass.isCut ? grass.heightScale * cutHeightScale : grass.heightScale * grass.randomHeight;
            float effectiveWidthScale = grass.isCut ? grass.widthScale * cutHeightScale : grass.widthScale * grass.randomScale;

            // 깎인 잔디: 크기 줄이고 mower 방향으로 회전
            if (grass.isCut)
            {
                Vector3 dirToGrass = (grass.position - mowerPos).normalized;
                float angle = Mathf.Atan2(dirToGrass.x, dirToGrass.z) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(grass.currentAngle, angle, 0);

                batchMatrices[batchIndex] = Matrix4x4.TRS(
                    grass.position,
                    rotation,
                    new Vector3(effectiveWidthScale, effectiveHeightScale, effectiveWidthScale)
                );
            }
            // 안 깎인 잔디: 원래 크기와 랜덤 방향
            else
            {
                Quaternion rotation = Quaternion.Euler(grass.currentAngle, grass.randomRotY, 0);

                batchMatrices[batchIndex] = Matrix4x4.TRS(
                    grass.position,
                    rotation,
                    new Vector3(grass.widthScale * grass.randomScale, grass.heightScale * grass.randomHeight, grass.widthScale * grass.randomScale)
                );
            }
            
            batchIndex++;

            if (batchIndex >= instancesPerBatch)
            {
                Graphics.DrawMeshInstanced(
                    grassMesh,
                    0,
                    grassMaterial,
                    batchMatrices,
                    batchIndex,
                    null,
                    UnityEngine.Rendering.ShadowCastingMode.Off,
                    false
                );
                batchIndex = 0;
            }
        }

        if (batchIndex > 0)
        {
            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                batchMatrices,
                batchIndex,
                null,
                UnityEngine.Rendering.ShadowCastingMode.Off,
                false
            );
        }
    }

    public int GrassCount => grassList.Count;
}
