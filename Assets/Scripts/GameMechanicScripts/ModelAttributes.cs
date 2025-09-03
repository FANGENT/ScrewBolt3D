using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class ModelAttributes : MonoBehaviour
{
    public Transform ModelBoltsContainer;
    public Transform ModelPartsContainer;
    /*[HideInInspector]*/
    public List<string> AllExistingColorNames;
    /*[HideInInspector]*/
    public List<Color> AllExistingColor;
    /*[HideInInspector]*/
    public List<int> AllExistingColorCount;
    /*[HideInInspector]*/
    public List<int> RemainingExistingColorCount;
    /*[HideInInspector]*/
    public int TotalNumberOfColors;

    private BlockedMeshedController[] ModelParts;

    private void Awake()
    {
        AllExistingColorNames.Add(ModelBoltsContainer.GetChild(0).name);
        AllExistingColor.Add(ModelBoltsContainer.GetChild(0).GetComponent<MeshRenderer>().material.color);
        for (int i = 0; i < ModelBoltsContainer.childCount; i++)
        {
            if(!CheckIfItExist(ModelBoltsContainer.GetChild(i).name))
            {
                AllExistingColorNames.Add(ModelBoltsContainer.GetChild(i).name);
                AllExistingColor.Add(ModelBoltsContainer.GetChild(i).GetComponent<MeshRenderer>().material.color);
            }   
        }
        for(int i = 0; i < AllExistingColorNames.Count; i++)
        {
            int counter = RepeatitionNumber(AllExistingColorNames[i]);
            AllExistingColorCount.Add(counter);
            RemainingExistingColorCount.Add(counter);
        }
        TotalNumberOfColors = AllExistingColorNames.Count;

        ModelParts = new BlockedMeshedController[ModelPartsContainer.childCount];
        for(int i = 0; i < ModelPartsContainer.childCount; i++)
        {
            ModelParts[i] = ModelPartsContainer.GetChild(i).GetComponent<BlockedMeshedController>();
        }
        SetPivotForRotation();
    }

    private bool CheckIfItExist(string newColorName)
    {
        for(int i = 0; i < AllExistingColorNames.Count; i++)
        {
            if(AllExistingColorNames[i] == newColorName)
            {
                return true;
            }
        }
        return false;
    }

    private int RepeatitionNumber(string colorName)
    {
        int counter = 0;
        for (int i = 0; i < ModelBoltsContainer.childCount; i++)
        {
            if (colorName == ModelBoltsContainer.GetChild(i).name)
            {
                counter++;
            }
        }
        return counter;
    }

    private int GetIndexByColorName(string colorName)
    {
        for(int i = 0; i < AllExistingColorNames.Count; i++)
        {
            if(AllExistingColorNames[i] == colorName)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetNumberOfRemainingBoltsByIndex(int index)
    {
        return RemainingExistingColorCount[index];
    }

    public bool CheckIfAllBoltsAreRemoved()
    {
        for(int i = 0; i < RemainingExistingColorCount.Count; i++)
        {
            if(RemainingExistingColorCount[i] > 0)
            {
                return false;
            }
        }
        return true;
    }

    public int GetNotZeroBoltIndex()
    {
        for (int i = 0; i < RemainingExistingColorCount.Count; i++)
        {
            if (RemainingExistingColorCount[i] > 0)
            {
                return i;
            }
        }
        return -1;
    }

    public void UpdatingRemainingBolts(string colorName , int number)
    {
        RemainingExistingColorCount[GetIndexByColorName(colorName)] -= number;
    }

    public string GetColorNameByIndex(int index)
    {
        return AllExistingColorNames[index];
    }

    public Color GetColorByIndex(int index)
    {
        return AllExistingColor[index];
    }

    public void SetPivotForRotation()
    {
        int NumberOfAffectiveMeshes = 0;
        float totalX = 0;
        float totalY = 0;
        Vector3 meanPosition = Vector3.zero;
        for (int i = 0; i < ModelParts.Length; i++)
        {
            if (ModelParts[i] && ModelParts[i].GetComponent<Rigidbody>().isKinematic)
            {
                NumberOfAffectiveMeshes++;
                MeshRenderer mr = ModelParts[i].GetComponent<MeshRenderer>();
                totalX += mr.bounds.center.x;
                totalY += mr.bounds.center.y;
            }
        }
        if (NumberOfAffectiveMeshes == 0)
        {
            return;
        }
        meanPosition = new Vector3(totalX / (NumberOfAffectiveMeshes * 1.0f), totalY / (NumberOfAffectiveMeshes * 1.0f), 0);
        transform.GetChild(0).DOMove(transform.GetChild(0).position - meanPosition, 0.5f);
    }
    public void MakeNecessaryPartsFall()
    {
        for (int i = 0; i < ModelParts.Length; i++)
        {
            if (ModelParts[i] && ModelParts[i].CheckIfItCanFall())
            {
                for (int j = 0; j < i; j++)
                {
                    if (ModelParts[j])
                    {
                        ModelParts[j].CheckIfItCanFall();
                    }
                }
            }
        }
    }


    //--A
    public void CheckRemainingBoltsCounts()
    {
        int total = 0;
        int remainingTotal = 0;
        for (int i = 0; i < RemainingExistingColorCount.Count; i++)
        {
            //Debug.Log("Color Name: " + AllExistingColorNames[i] + " Remaining Count: " + RemainingExistingColorCount[i]);
            remainingTotal += RemainingExistingColorCount[i];
        }

        for(int i = 0; i < AllExistingColorCount.Count; i++)
        {
            total += AllExistingColorCount[i];
        }
        float progressInPercentage = ((total - remainingTotal) / (total * 1.0f)) * 100.0f;
        GameplayUiManager.Instance.levelProgressText.text = progressInPercentage.ToString("0") + "%";
        GameplayUiManager.Instance.levelProgressImage.fillAmount = (total - remainingTotal) / (total * 1.0f);
    }
}

