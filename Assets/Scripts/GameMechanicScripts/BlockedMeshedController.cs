using UnityEngine;

public class BlockedMeshedController : MonoBehaviour
{
    public Transform[] AllBlockingBolts;
    public Transform[] AllBlockingMeshes;
    public bool Debuggable = false;
    public bool CheckIfItCanFall()
    {
        if (AllBlockingBoltsAreRemoved() && AllBlockingMeshesAreRemoved())
        {
            GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
            return true;
        }
        return false;
    }
    private bool AllBlockingBoltsAreRemoved()
    {
        if(AllBlockingBolts == null)
        {
            return true;
        }
        for(int i = 0; i < AllBlockingBolts.Length; i++)
        {
            if(AllBlockingBolts[i] && AllBlockingBolts[i].parent != null && AllBlockingBolts[i].parent.name == "Screws")
            {
                return false;
            }
        }
        return true;
    }
    private bool AllBlockingMeshesAreRemoved()
    {
        if(AllBlockingMeshes == null)
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
