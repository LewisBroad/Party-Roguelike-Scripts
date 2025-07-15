using UnityEngine;
using UnityEngine.AI;

public class SpawnVolume : MonoBehaviour
{
    // public Vector3 GetRandomPointInVolume()
    // {
    //     Bounds bounds = GetComponent<Collider>().bounds;
    //     Vector3 point = new Vector3(
    //         Random.Range(bounds.min.x, bounds.max.x),
    //         bounds.max.y,
    //         Random.Range(bounds.min.z, bounds.max.z));

    //     if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, bounds.size.y))
    //     {
    //         if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
    //         {
    //             return navHit.position;
    //         }
    //     }
    //     return Vector3.zero;
    // }
    public Vector3 GetRandomPointInVolume()
    {
        Bounds bounds = GetComponent<Collider>().bounds;

        for (int i = 0; i < 10; i++)
        {
            Vector3 rayStart = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 10f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Ignore))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    Debug.DrawRay(rayStart, Vector3.down * 100f, Color.cyan, 2f);
                    Debug.Log($"[SpawnVolume] Hit at {hit.point}, NavMesh at {navHit.position}");
                    return navHit.position;
                }
            }
        }

        Debug.LogWarning($"[SpawnVolume] {gameObject.name} failed to find a valid NavMesh point.");
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(TryGetComponent<Collider>(out Collider collider))
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
