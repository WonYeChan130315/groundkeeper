using UnityEngine;
using System.Collections.Generic;

public class GrassCutter : MonoBehaviour
{
    public Transform mower;
    public float mowerRadius = 1f;
    public float bendAngle = 40f;
    public float standUpSpeed = 5f;
    public float growSpeed = 0.5f;
    public float growDelay = 3f;

    private void Update()
    {
        if (mower == null) return;

        Vector3 mowerPos = mower.position;

        foreach (var grass in GrassManager.Instance.grassList)
        {
            float dist = Vector3.Distance(grass.position, mowerPos);

            // mower 반경 내 잔디 자르기
            if (dist < mowerRadius)
            {
                grass.isCut = true;
                grass.mowerAngle = bendAngle;
                grass.cutTime = Time.time;
            }

            // 자라기 처리
            if (grass.isCut)
            {
                float timeSinceCut = Time.time - grass.cutTime;

                // 대기 시간 후 자라기 시작
                if (timeSinceCut > growDelay)
                {
                    float growProgress = Mathf.Clamp01((timeSinceCut - growDelay) * growSpeed);
                    grass.currentAngle = Mathf.Lerp(grass.mowerAngle, grass.baseAngle, growProgress);
                    
                    if (growProgress >= 1f)
                    {
                        grass.isCut = false;
                    }
                }
                // 눕는 애니메이션
                else
                {
                    grass.currentAngle = Mathf.Lerp(grass.currentAngle, grass.mowerAngle, Time.deltaTime * standUpSpeed);
                }
            }
        }
    }
}
