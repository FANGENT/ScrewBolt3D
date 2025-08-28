using UnityEngine;
using DG.Tweening;

public class ExtraLockedContainer : MonoBehaviour
{
    public int PlacementIndex = -1;
    public Transform Placement;
    public Transform FilledBolt;

    public void InitializeThisContainer(int placementIndex)
    {
        PlacementIndex = placementIndex;
        transform.DOMove(BoltContainerManager.Instance.InitialTransformsForExtraContainers[PlacementIndex].position, 0.5f).From(BoltContainerManager.Instance.FinalTransformsForExtraContainers[PlacementIndex].position);
    }
}
