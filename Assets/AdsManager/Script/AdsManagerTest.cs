using UnityEngine;
using UnityEngine.UI;

public class AdsManagerTest : MonoBehaviour
{
    public Text _userName;
    public Image _userImage;

    public Text localizedPrice;

    private void Start()
    {
        AdsManager.Instance.onPlayServicesConnectResult += Instance_onPlayServicesConnectResult;
    }

    private void Instance_onPlayServicesConnectResult(string userName, Sprite userImage)
    {
        _userName.text = name;
        _userImage.overrideSprite = userImage;
    }


    // Update is called once per frame
    //public void IncrementAchievemnt()
    //{
    //    AdsManager.Instance.IncrementAchievement(GPGSIds.achievement_master_in_shooting, 100);
    //}
    //public void ReportProgress()
    //{
    //    AdsManager.Instance.ReportProgress(GPGSIds.achievement_master_in_shooting, 100);
    //}
    //public void SubmitScore()
    //{

    //    AdsManager.Instance.SubmitScore(GPGSIds.leaderboard_highscore, 1);
    //}
    public void GetLocalizedPrice(string sku)
    {
        //  localizedPrice.text = AdsManager.Instance.GetLocalizedPrice(sku);
    }

    public void ShowRewardedAd()
    {
        if(AdsManager.Instance.HasRewardedVideo())
        AdsManager.Instance.ShowRewardedVideo(RewardType.cash, 100);
    }

}
