using UnityEngine;
using DG.Tweening;

public class EmptyContainer : MonoBehaviour
{
    public int PlacementIndex = -1;
    public void InitializeThisContainer(int placementIndex)
    {
        PlacementIndex = placementIndex;
        transform.DOMove(BoltContainerManager.Instance.FinalTransformsForContainers[PlacementIndex].position, 0.5f).From(BoltContainerManager.Instance.InitialTransformsForContainers[PlacementIndex].position);
    }
}

