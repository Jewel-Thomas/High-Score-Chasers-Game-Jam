using UnityEngine;

public class MeshHeightChecker : MonoBehaviour
{
    public static MeshHeightChecker Instance;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 20f;

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public float GetGroundHeight()
    {
        Vector3 rayStartPosition = transform.position + Vector3.up * 0.5f;

        if(Physics.Raycast(rayStartPosition, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            float groundHeight = hit.point.y;
            return groundHeight;
        }

        return 0f;
    }

}
