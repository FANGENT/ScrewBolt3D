using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EmptyContainer : MonoBehaviour
{
    public int PlacementIndex = -1;
    private Button emptyContainerBtn;

    private void Start()
    {
        emptyContainerBtn = GetComponent<Button>();
        emptyContainerBtn.onClick.AddListener(OnClickEmptyContainer);
    }
    public void InitializeThisContainer(int placementIndex)
    {
        PlacementIndex = placementIndex;
        transform.DOMove(BoltContainerManager.Instance.FinalTransformsForContainers[PlacementIndex].position, 0.5f).From(BoltContainerManager.Instance.InitialTransformsForContainers[PlacementIndex].position);
    }

    //--A
    void TestOnClickEmptyContainer()
    {
        BoltContainerManager.Instance.MakeNewContainerWhereUnscrewedBoltsCanBePlaced(PlacementIndex);
        gameObject.SetActive(false);
    }

    void OnClickEmptyContainer()
    {
        GameplayUiManager.Instance.OnEmptyContainerButtonClicked(PlacementIndex, gameObject);
    }

}

