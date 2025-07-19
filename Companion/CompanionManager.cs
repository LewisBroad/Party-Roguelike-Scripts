using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    public GameObject[] playableCharacterPrefabs;
    public Transform[] spawnPoints;

    public void SpawnTeammates(GameObject selectedPlayer)
    {
        List<GameObject> availableCharacters = playableCharacterPrefabs.ToList();
        availableCharacters.Remove(selectedPlayer);

        for (int i = 0; i < 4; i++)
        {
            GameObject prefab = availableCharacters[i];
            GameObject teammate = Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);

            AICharacterController ai = teammate.GetComponent<AICharacterController>();
            if (ai != null)
            {
                ai.SetFollowTarget(selectedPlayer.transform);
            }
        }
    }
}
