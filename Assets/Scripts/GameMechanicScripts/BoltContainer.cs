using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BoltContainer : MonoBehaviour
{
    public int PlacementIndex = -1;
    public Color ContainerColor;
    public string ContainerColorName;
    //--A
    //public SpriteRenderer ContainerRenderer;
    public Image containerImage;
    public GameObject ColorSpace;
    public GameObject ContainerSpace;
    public Transform[] Placements;
    public Transform[] FilledBolts;
    public int TotalPlacements;

    public void InitializeThisContainer(int placementIndex,Color newAssignedColor, string newAssignedColorName)
    {
        //ContainerRenderer.sprite = Resources.Load<Sprite>("Box/"+ newAssignedColorName);
        //--A
        containerImage.sprite = Resources.Load<Sprite>("Box/"+ newAssignedColorName);
        PlacementIndex = placementIndex;
        TotalPlacements = ContainerSpace.transform.childCount;
        Placements = new Transform[TotalPlacements];
        FilledBolts = new Transform[TotalPlacements];
        for (int i = 0; i < TotalPlacements; i++)
        {
            Placements[i] = ContainerSpace.transform.GetChild(i).transform;
            FilledBolts[i] = null;
        }
        ContainerColor = newAssignedColor;
        ContainerColorName = newAssignedColorName;
        //ContainerRenderer.material.color = ContainerColor;
        transform.DOMove(BoltContainerManager.Instance.FinalTransformsForContainers[PlacementIndex].position, 0.5f).From(BoltContainerManager.Instance.InitialTransformsForContainers[PlacementIndex].position).
            OnComplete(() => BoltContainerManager.Instance.CallForAllMatchingBoltsFoundInExtraBoltContainers(
                ContainerColorName,
                this,
                Placements.Length
                ));
    }

    public Transform PlaceInThisContainerIfPossible(Transform UnscrewedBolt)
    {
        if (UnscrewedBolt.name != ContainerColorName)
        {
            return null;
        }

        for (int i = 0; i < FilledBolts.Length; i++)
        {
            if (FilledBolts[i] == null)
            {
                FilledBolts[i] = UnscrewedBolt;

                //--A
                //Decrement here, only when a bolt is actually placed
                ModelController.Instance.modelAttributes.UpdatingRemainingBolts(ContainerColorName, 1);
                ModelController.Instance.modelAttributes.CheckRemainingBoltsCounts();

                //Optional: check level completion immediately
                if (ModelController.Instance.modelAttributes.CheckIfAllBoltsAreRemoved())
                {
                    Debug.Log("Game Is Completed");
                    GameplayUiManager.Instance.OnLevelComplete();
                }

                CheckIfThisContainerIsFilledCompletelyWithBolts();
                return Placements[i];
            }
        }
        return null;
    }


    private void CheckIfThisContainerIsFilledCompletelyWithBolts()
    {
        for(int i = 0; i < FilledBolts.Length; i++)
        {
            if(FilledBolts[i] == null)
            {
                return;
            }
        }
        ActionsCarriedOutWhenContainerIsFilledCompletely();
    }

    private void ActionsCarriedOutWhenContainerIsFilledCompletely()
    {
        RectTransform containerPos = gameObject.GetComponent<RectTransform>();

        transform.DOMove(
            BoltContainerManager.Instance.InitialTransformsForContainers[PlacementIndex].position, 1.6f)
            .From(BoltContainerManager.Instance.FinalTransformsForContainers[PlacementIndex].position)
            .SetDelay(1.3f)
            .OnStart(() =>
            {
                // First do your container logic
                BoltContainerManager.Instance.MakeNewContainerWhereUnscrewedBoltsCanBePlaced(PlacementIndex);

                // Then wait 0.1s after movement starts, and play star animation
                DOVirtual.DelayedCall(0.01f, () =>
                {
                    // Calculate true world center of the rect
                    Vector3 worldCenter = containerPos.TransformPoint(containerPos.rect.center);

                    // Create a temporary object at that position
                    GameObject tempCenter = new GameObject("TempStarCenter");
                    RectTransform tempRect = tempCenter.AddComponent<RectTransform>();
                    tempRect.position = worldCenter;
                    tempRect.SetParent(containerPos.parent, true); // put it in same canvas space

                    // Play star animation from true center
                    GameplayUiManager.Instance.PLayStarAnimationFromPosition(tempRect);

                    // Clean up after animation if needed
                    Destroy(tempCenter, 2f);
                });

            });
    }


}
