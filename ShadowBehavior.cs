using UnityEngine;
using System.Collections;

public class ShadowBehavior : MonoBehaviour
{

    RaycastHit shadowHit;
    public float offsetAlign = 0f;
    private Transform shadow;
    public Transform shadowLocation;

    private float normalizedDistance;
    private Vector3 scaling = Vector3.zero;
    public GameObject shadowQuad;
    private MeshRenderer shadowQuadRend;

    void Start()
    {
        shadow = transform;
        shadowQuadRend = shadowQuad.GetComponent<MeshRenderer>();
    }

    void FixedUpdate()
    {
        StayGrounded();
    }

    void StayGrounded()
    {
        if (Physics.Raycast(shadowLocation.position, Vector3.down, out shadowHit, 4))
        {
            shadowQuadRend.enabled = true;
            shadow.position = new Vector3(shadowLocation.position.x, (shadowHit.point.y + offsetAlign), shadowLocation.position.z);
            shadow.rotation = Quaternion.FromToRotation(Vector3.back, shadowHit.normal);
            normalizedDistance = 1 - ((shadowHit.distance - offsetAlign) / 4);
            shadow.localScale = new Vector3(normalizedDistance, normalizedDistance, normalizedDistance);

            if (normalizedDistance < 0)
            {
                normalizedDistance = 0;
            }

        }
        else
            shadowQuadRend.enabled = false;
    }

}
