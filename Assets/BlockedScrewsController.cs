using UnityEngine;

public class BlockedScrewsController : MonoBehaviour
{
    public Transform[] AllBlockingMeshes;
    public bool Debuggable = false;
    public bool CheckIfItCanBePulledOut()
    {
        return AllBlockingMeshesAreRemoved();
    }
    private bool AllBlockingMeshesAreRemoved()
    {
        if (AllBlockingMeshes == null)
        {
            return true;
        }
        for (int i = 0; i < AllBlockingMeshes.Length; i++)
        {
            if (AllBlockingMeshes[i] && AllBlockingMeshes[i].GetComponent<Rigidbody>().isKinematic)
            {
                return false;
            }
        }
        return true;
    }
}
