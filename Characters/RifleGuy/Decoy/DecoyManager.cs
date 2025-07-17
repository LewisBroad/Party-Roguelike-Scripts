using UnityEngine;

public class DecoyManager : MonoBehaviour
{
    public static DecoyManager Instance;

    private Transform activeDecoy;

    void Awake()
    {
        Instance = this;
    }

    public void SetDecoy(Transform decoyTransform)
    {
        activeDecoy = decoyTransform;
    }

    public Transform GetDecoyTransform()
    {
        return activeDecoy;
    }

    public bool IsDecoyActive()
    {
        return activeDecoy != null;
    }

    public void ClearDecoy()
    {
        activeDecoy = null;
    }
}
