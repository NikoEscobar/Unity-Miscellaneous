using UnityEngine;
using System.Collections;

public class ShadowBehavior : MonoBehaviour
{
    public Transform shadowLocation;

    private Transform shadow;

    private RaycastHit ground;

    private float offset = 0.01f;
    private float normalizedDistance;
    private float maxShadowVisibleDistance = 4f;

    private MeshRenderer shadowRender;

    void Awake()
    {
        shadow = transform;
        shadowRender = GetComponentInChildren<MeshRenderer>();
    }

    void FixedUpdate()
    {
        ApplyShadowBehaviour();
    }

    void ApplyShadowBehaviour()
    {
        if (IsGroundInRange())
        {
            shadowRender.enabled = true;
            SetShadowPositionClampedOnFloor();
            ShrinkShadowByFloorDistance();
            PreventShrinkGoingNegative();
        }
        else
            shadowRender.enabled = false;
    }

    private bool IsGroundInRange()
    {
        if (Physics.Raycast(shadowLocation.position, Vector3.down, out ground, maxShadowVisibleDistance))
            return true;
        else
            return false;
    }

    private void SetShadowPositionClampedOnFloor()
    {
        shadow.position = new Vector3(shadowLocation.position.x, (ground.point.y + offset), shadowLocation.position.z);
        shadow.rotation = Quaternion.FromToRotation(Vector3.back, ground.normal);
    }

    private void ShrinkShadowByFloorDistance()
    {
        normalizedDistance = 1 - ((ground.distance - offset) / maxShadowVisibleDistance);
        shadow.localScale = new Vector3(normalizedDistance, normalizedDistance, normalizedDistance);
    }

    private void PreventShrinkGoingNegative()
    {
        if (normalizedDistance < 0)
        {
            normalizedDistance = 0;
        }
    }
}
