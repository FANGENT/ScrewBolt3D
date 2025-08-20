using UnityEngine;
using DG.Tweening;

public class ExtraContainer : MonoBehaviour
{
    public int PlacementIndex = -1;
    public Transform Placement;
    public Transform FilledBolt;

    public void InitializeThisContainer(int placementIndex)
    {
        PlacementIndex = placementIndex;
        transform.DOMove(BoltContainerManager.Instance.FinalTransformsForExtraContainers[PlacementIndex].position, 0.5f).From(BoltContainerManager.Instance.InitialTransformsForExtraContainers[PlacementIndex].position);
    }

    public Transform PlaceInThisContainerIfPossible(Transform UnscrewedBolt)
    {
        if (FilledBolt)
        {
            return null;
        }
        else
        {
            FilledBolt = UnscrewedBolt;
            return Placement;
        }
    }

    public Transform GetBoltOfSpecificColor(string colorName)
    {
        if(FilledBolt == null)
        {
            return null;
        }
        if(colorName != FilledBolt.name)
        {
            return null;
        }
        else
        {
            Transform FilledBoltReference = FilledBolt;
            FilledBolt = null;
            return FilledBoltReference;
        }
    }
}
