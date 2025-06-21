using UnityEngine;
using UnityEngine.AI;

public class SpawnVolume : MonoBehaviour
{
    public Vector3 GetRandomPointInVolume()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 point = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.max.y,
            Random.Range(bounds.min.z, bounds.max.z));

        if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, bounds.size.y))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                return navHit.position;
            }
        }
        return Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(TryGetComponent<Collider>(out Collider collider))
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
