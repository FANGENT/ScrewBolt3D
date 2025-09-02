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

                //Decrement here, only when a bolt is actually placed
                ModelController.Instance.modelAttributes.UpdatingRemainingBolts(ContainerColorName, 1);

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
        transform.DOMove(
            BoltContainerManager.Instance.InitialTransformsForContainers[PlacementIndex].position, 0.7f).
            From(BoltContainerManager.Instance.FinalTransformsForContainers[PlacementIndex].position).
            SetDelay(0.7f).
            OnStart(()=> BoltContainerManager.Instance.MakeNewContainerWhereUnscrewedBoltsCanBePlaced(PlacementIndex));
    }
}
