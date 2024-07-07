using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
    private Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0);
    private float forwardRayLength = 0.8f;
    private float heightRayLength = 5;
    private float ledgeRayLength = 5000;
    private LayerMask obstacleLayer;
    private string obstacleLayerName = "Obstacles";
    private float ledgeHeightThreshold = 0.75f;

    private Vector3 forwardRayOffsetJumpMax = new Vector3(0, 1.75f, 0);
    private Vector3 forwardRayOffsetJumpMid = new Vector3(0, 1.0f, 0);
    private Vector3 forwardRayOffsetJumpMin = new Vector3(0, 0.2f, 0);
    private float forwardRayLengthJump = 5.5f;
    private Vector3 forwardRayOffsetSlideMax = new Vector3(0, 0.75f, 0);
    private Vector3 forwardRayOffsetSlideMid = new Vector3(0, 0.4f, 0);
    private Vector3 forwardRayOffsetSlideMin = new Vector3(0, 0.1f, 0);
    private float forwardRayLengthSlide = 5.5f;

    private void Awake()
    {
        obstacleLayer = LayerMask.GetMask(obstacleLayerName);
    }

    public ObstacleHitData ObstacleCheck()
    {
        var hitData = new ObstacleHitData();

        var forwardOrigin = transform.position + forwardRayOffset;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, 
            out hitData.forwardHit, forwardRayLength, obstacleLayer);

        // Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.red : Color.white);

        if (hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down,
                out hitData.heightHit, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.red : Color.white);
        }
        
        return hitData;
    }

    public bool ClearPathJumpCheck()
    {
        var hitData = new ObstacleHitData();

        var forwardOriginMax = transform.position + forwardRayOffsetJumpMax;
        var forwardOriginMid = transform.position + forwardRayOffsetJumpMid;
        var forwardOriginMin = transform.position + forwardRayOffsetJumpMin;
        return !Physics.Raycast(forwardOriginMax, transform.forward, 
            out hitData.forwardHit, forwardRayLengthJump, obstacleLayer) &&
            !Physics.Raycast(forwardOriginMid, transform.forward, 
            out hitData.forwardHit, forwardRayLengthJump, obstacleLayer) &&
            !Physics.Raycast(forwardOriginMin, transform.forward, 
            out hitData.forwardHit, forwardRayLengthJump, obstacleLayer);
    }

    public bool ClearPathSlideCheck()
    {
        var hitData = new ObstacleHitData();

        var forwardOriginMax = transform.position + forwardRayOffsetSlideMax;
        var forwardOriginMid = transform.position + forwardRayOffsetSlideMid;
        var forwardOriginMin = transform.position + forwardRayOffsetSlideMin;
        return !Physics.Raycast(forwardOriginMax, transform.forward, 
            out hitData.forwardHit, forwardRayLengthSlide, obstacleLayer) &&
            !Physics.Raycast(forwardOriginMid, transform.forward, 
            out hitData.forwardHit, forwardRayLengthSlide, obstacleLayer) &&
            !Physics.Raycast(forwardOriginMin, transform.forward, 
            out hitData.forwardHit, forwardRayLengthSlide, obstacleLayer);
    }

    public bool LedgeCheck(Vector3 moveDir, out LedgeData ledgeData)
    {
        ledgeData = new LedgeData();

        if (moveDir == Vector3.zero)
            return false;

        float originOffset = 0.5f;
        var origin = transform.position + moveDir * originOffset + Vector3.up;

        if (PhysicsUtil.ThreeRaycasts(origin, Vector3.down, 0.25f, transform, 
            out List<RaycastHit> hits, ledgeRayLength, obstacleLayer, true))
        {
            var validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

            if (validHits.Count > 0)
            {
                var surfaceRayOrigin = validHits[0].point;
                surfaceRayOrigin.y = transform.position.y - 0.1f;

                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
                {
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);

                    float height = transform.position.y - validHits[0].point.y;

                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;

                    return true;
                }
            }
        }

        return false;
    }
    
    public struct ObstacleHitData
    {
        public bool forwardHitFound;
        public bool heightHitFound;
        public RaycastHit forwardHit;
        public RaycastHit heightHit;
    }
}



public struct LedgeData
{
    public float height;
    public float angle;
    public RaycastHit surfaceHit;
}
