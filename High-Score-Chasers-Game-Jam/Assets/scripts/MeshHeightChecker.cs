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
        DontDestroyOnLoad(gameObject);
    }

    public float GetGroundHeight()
    {
        Vector3 rayStartPosition = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);

        if (Physics.Raycast(rayStartPosition, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            float groundHeight = hit.point.y;
            return groundHeight;
        }

        return 0f;
    }

    public Vector3 PositionObjectsToGround(Vector3 rayStartPos)
    {
        Vector3 highStart = new Vector3(rayStartPos.x, rayStartPos.y + 50f, rayStartPos.z);

        if (Physics.Raycast(highStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return new Vector3(rayStartPos.x, hit.point.y, rayStartPos.z);
        }

        return rayStartPos;
    }

}
