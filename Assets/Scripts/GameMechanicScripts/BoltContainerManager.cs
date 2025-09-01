using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BoltContainerManager : MonoBehaviour
{
    public static BoltContainerManager Instance;
    private int NumberOfExistingContainers
    {
        get
        {
            return PlayerPrefs.GetInt("NumberOfExistingContainers", 2);
        }
        set
        {
            PlayerPrefs.SetInt("NumberOfExistingContainers", value);
        }
    }
    private int NumberOfExtraContainers
    {
        get
        {
            return 4;
            //return PlayerPrefs.GetInt("NumberOfExtraContainers", 6);
        }
        set
        {
            PlayerPrefs.SetInt("NumberOfExtraContainers", value);
        }
    }
    public RectTransform parentUIPanel;
    public GameObject PrefabReferencex2;
    public GameObject PrefabReferencex3;
    public GameObject EmptyContainerPrefab;
    public GameObject ExtraContainerPrefab;
    public GameObject ExtraLockedContainerPrefab;
    public Transform InitialTransform, FinalTransform;
    public List<BoltContainer> AvailableContainersList;
    public List<EmptyContainer> AvailableEmptyContainersList;
    public List<ExtraContainer> AvailableExtraContainersList;
    public List<ExtraLockedContainer> AvailableExtraLockedContainersList;
    public Transform[] InitialTransformsForContainers, FinalTransformsForContainers;
    public Transform[] InitialTransformsForExtraContainers, FinalTransformsForExtraContainers;

    //public IEnumerator UnscrewAll()
    //{
    //    yield return new WaitForSeconds(1);
    //    while(ModelController.Instance.modelAttributes.ModelBoltsContainer.childCount > 0)
    //    {
    //        int index = Random.Range(0, ModelController.Instance.modelAttributes.ModelBoltsContainer.childCount);
    //        GameObject Bolt = ModelController.Instance.modelAttributes.ModelBoltsContainer.GetChild(index).gameObject;
    //        ModelController.Instance.UnScrewBolt(Bolt);
    //        if (CheckIfInFatalCondition())
    //        {
    //            yield return new WaitForSeconds(500);
    //        }
    //        yield return new WaitForSeconds(0.2f);
    //    }
    //}
    //private bool CheckIfInFatalCondition()
    //{
    //    for (int i = 0; i < AvailableExtraContainersList.Count; i++)
    //    {
    //        if (AvailableExtraContainersList[i].transform.GetChild(0).childCount > 0)
    //        {
    //            if (AvailableExtraContainersList[i].FilledBolt == null)
    //            {
    //                Camera.main.backgroundColor = Color.red;
    //                Debug.LogError("**********Fatal Condition*************");
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}
    private void Awake()
    {
        Instance = this;
        //StartCoroutine(UnscrewAll());
    }
    public void InitializeAllContainers()
    {
        for(int i = 0; i < 4; i++)
        {
            if (i < NumberOfExistingContainers)
            {
                MakeNewContainerWhereUnscrewedBoltsCanBePlaced(i);
            }
            else
            {
                MakeNewEmptyContainerToBeUnlockedOnRewardedVideo(i);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            //--A
            if (i < NumberOfExtraContainers)
            {
                MakeNewExtraContainerWhereUnMatchedBoltsCanBePlaced(i);
            }
            else
            {
                MakeNewExtraLockedContainerWhereUnMatchedBoltsCanBePlaced(i);
            }
        }
    }

    public void MakeNewContainerWhereUnscrewedBoltsCanBePlaced(int PlacementIndex)
    {
        int selectedIndex = Random.Range(0, ModelController.Instance.modelAttributes.TotalNumberOfColors);
        int remainingBolts = ModelController.Instance.modelAttributes.GetNumberOfRemainingBoltsByIndex(selectedIndex);
        if(remainingBolts == 0)
        {
            if (ModelController.Instance.modelAttributes.CheckIfAllBoltsAreRemoved())
            {
                Debug.Log("Game Is Completed");
                GameplayUiManager.Instance.OnLevelComplete();
                return;
            }
            else
            {
                selectedIndex = ModelController.Instance.modelAttributes.GetNotZeroBoltIndex();
                remainingBolts = ModelController.Instance.modelAttributes.GetNumberOfRemainingBoltsByIndex(selectedIndex);
            }
        }

        
        GameObject newContainer;
        int numberOfContainerHoles = -1;
        if(remainingBolts > 4)
        {
            newContainer = Instantiate(PrefabReferencex3, parentUIPanel);
            numberOfContainerHoles = 3;
        }
        else if(remainingBolts == 4)
        {
            newContainer = Instantiate(PrefabReferencex2, parentUIPanel);
            numberOfContainerHoles = 2;
        }
        else if (remainingBolts == 3)
        {
            newContainer = Instantiate(PrefabReferencex3, parentUIPanel);
            numberOfContainerHoles = 3;
        }
        else if(remainingBolts == 2)
        {
            newContainer = Instantiate(PrefabReferencex2, parentUIPanel);
            numberOfContainerHoles = 2;
        }
        else
        {
            newContainer = null;
            return;
        }

        ModelController.Instance.modelAttributes.UpdatingRemainingBolts
            (
            ModelController.Instance.modelAttributes.GetColorNameByIndex(selectedIndex),
            numberOfContainerHoles
            );
        newContainer.GetComponent<BoltContainer>().InitializeThisContainer(
            PlacementIndex,
            ModelController.Instance.modelAttributes.GetColorByIndex(selectedIndex),
            ModelController.Instance.modelAttributes.GetColorNameByIndex(selectedIndex)
            );
    }

    public void MakeNewEmptyContainerToBeUnlockedOnRewardedVideo(int PlacementIndex)
    {
        GameObject newContainer = Instantiate(EmptyContainerPrefab, parentUIPanel);
        AvailableEmptyContainersList.Add(newContainer.GetComponent<EmptyContainer>());
        newContainer.GetComponent<EmptyContainer>().InitializeThisContainer(PlacementIndex);
    }

    public void MakeNewExtraContainerWhereUnMatchedBoltsCanBePlaced(int PlacementIndex)
    {
        GameObject newContainer = Instantiate(ExtraContainerPrefab, parentUIPanel);
        AvailableExtraContainersList.Add(newContainer.GetComponent<ExtraContainer>());
        newContainer.GetComponent<ExtraContainer>().InitializeThisContainer(PlacementIndex);
    }

    public void MakeNewExtraLockedContainerWhereUnMatchedBoltsCanBePlaced(int PlacementIndex)
    {
        GameObject newContainer = Instantiate(ExtraLockedContainerPrefab, parentUIPanel);
        AvailableExtraLockedContainersList.Add(newContainer.GetComponent<ExtraLockedContainer>());
        newContainer.GetComponent<ExtraLockedContainer>().InitializeThisContainer(PlacementIndex);
    }

    public void CallForAllMatchingBoltsFoundInExtraBoltContainers(string ColorName, BoltContainer newlyMadeContainer, int MaximumPlacementsThisContainerCanCarry)
    {
        for(int i = 0; i < AvailableExtraContainersList.Count && MaximumPlacementsThisContainerCanCarry > 0; i++)
        {
            Transform MatchingBoltFoundInExtraContainer = AvailableExtraContainersList[i].GetBoltOfSpecificColor(ColorName);
            if (MatchingBoltFoundInExtraContainer)
            {
                MaximumPlacementsThisContainerCanCarry--;
                Transform Placement = newlyMadeContainer.PlaceInThisContainerIfPossible(MatchingBoltFoundInExtraContainer);
                if (Placement)
                {
                    MatchingBoltFoundInExtraContainer.parent = Placement;
                    Sequence seq = DOTween.Sequence();
                    seq.Join(MatchingBoltFoundInExtraContainer.transform.GetChild(0).DOLocalRotate(new Vector3(1440, 0, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
                    seq.Join(MatchingBoltFoundInExtraContainer.transform.DOMove(MatchingBoltFoundInExtraContainer.transform.position - MatchingBoltFoundInExtraContainer.transform.right * 0.5f, 0.2f).SetEase(Ease.Linear)/*.OnComplete(() => MatchingBoltFoundInExtraContainer.transform.SetParent(null))*/);

                    seq.Append(MatchingBoltFoundInExtraContainer.transform.DOMove(Placement.position, 0.2f)/*.OnComplete(()=> )*/);
                    seq.Join(MatchingBoltFoundInExtraContainer.transform.DORotate(new Vector3(0, 270, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

                    seq.onComplete = () =>
                    {
                        //--A Bolt is placed in the container now, so reset its scale to normal
                        MatchingBoltFoundInExtraContainer.transform.localScale = Vector3.one * 2f;
                    };


                    seq.Play();
                }
            }
        }

        AvailableContainersList.Add(newlyMadeContainer);
    }

    public Transform GetTargetWhereBoltShouldBePlaced(Transform UnscrewedBolt)
    {
        Transform Placement = FindSpaceInExistingBoltContainers(UnscrewedBolt);
        if (Placement)
        {
            return Placement;
        }
        Placement = FindSpaceInExraBoltContainers(UnscrewedBolt);
        if (Placement)
        {
            return Placement;
        }
        return null;
    }

    private Transform FindSpaceInExistingBoltContainers(Transform UnscrewedBolt)
    {
        for(int i = 0; i < AvailableContainersList.Count; i++)
        {
            Transform newlyAssignedPlacement = AvailableContainersList[i].PlaceInThisContainerIfPossible(UnscrewedBolt);
            if (newlyAssignedPlacement)
            {
                return newlyAssignedPlacement;
            }
        }
        return null;
    }

    private Transform FindSpaceInExraBoltContainers(Transform UnscrewedBolt)
    {
        for (int i = 0; i < AvailableExtraContainersList.Count; i++)
        {
            Transform newlyAssignedPlacement = AvailableExtraContainersList[i].PlaceInThisContainerIfPossible(UnscrewedBolt);
            if (newlyAssignedPlacement)
            {
                return newlyAssignedPlacement;
            }
        }
        return null;
    }

    public void OnDrillBtnPressed()
    {
        if (AvailableExtraLockedContainersList.Count == 0)
        {
            Debug.Log("No More Extra Containers To Unlock");
            return;
        }

        // Always unlock the FIRST locked container in the list
        ExtraLockedContainer lockedContainer = AvailableExtraLockedContainersList[0];
        int placementIndex = lockedContainer.PlacementIndex;

        // Remove it from locked list
        AvailableExtraLockedContainersList.RemoveAt(0);
        Destroy(lockedContainer.gameObject);

        // Replace with an unlocked extra container
        MakeNewExtraContainerWhereUnMatchedBoltsCanBePlaced(placementIndex);

        Debug.Log("Unlocked one extra container at index: " + placementIndex);
    }

}

