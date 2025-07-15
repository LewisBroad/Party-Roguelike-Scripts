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
                Debug.DrawRay(rayStart, Vector3.down * 100f, Color.yellow, 3f);

                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    Debug.DrawRay(navHit.position, Vector3.up * 2f, Color.green, 3f);
                    Debug.Log($" Valid spawn point: {navHit.position} from volume {gameObject.name}");
                    return navHit.position;
                }
                else
                {
                    Debug.LogWarning($" No NavMesh at ray hit {hit.point} in volume {gameObject.name}");
                }
            }
            else
            {
                Debug.LogWarning($" Raycast missed ground in volume {gameObject.name} from {rayStart}");
            }
        }

        Debug.LogWarning($" Volume {gameObject.name} failed to find valid position after 10 attempts");
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(TryGetComponent<Collider>(out Collider collider))
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
