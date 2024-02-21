using UnityEngine;
using GoogleMobileAds.Api;

using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;

public class AdMobController : MonoBehaviour
{

    public string appID = "ca-app-pub-3476607833523334~9426593563";

    string interID = "ca-app-pub-3476607833523334/7143522061";
    string rewardedID = "ca-app-pub-3476607833523334/3077968750";


    //���߿� �ȵ���̵� ������ ������ ���� ��ȹ ������ ���ʿ� ��������
//#if UNITY_ANDROID


//#elif UNITY_IPHONE
//        string interID = "ca-app-pub-3476607833523334/7143522061";
//        string rewardedID = "ca-app-pub-3476607833523334/3077968750";

//#endif

    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;
    NativeAd nativeAd;

    private void Start()
    {
        //MobileAds.RaiseAdEventsOnUnityMainThread = true;
        //MobileAds.Initialize(initStatus =>
        //{
        //    LoadInterstitialAd();
        //    Debug.Log("�ֵ�� Init");
        //});

    }

    #region Interstitial

    public void LoadInterstitialAd()
    {
        Debug.Log("�ֵ�� Load LoadInterstitialAd");

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        InterstitialAd.Load(interID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Interstitial ad failed to load" + error);
                return;
            }

            Debug.Log("Interstitial ad loaded !!" + ad.GetResponseInfo());

            interstitialAd = ad;
            InterstitialEvent(interstitialAd);
        });

    }
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("���鱤�� �غ���� ����");
        }
    }
    public void InterstitialEvent(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion

    #region Rewarded

    public void LoadRewardedAd()
    {
        if (rewardedAd!=null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        RewardedAd.Load(rewardedID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Interstitial ad failed to load" + error);
                return;
            }
            Debug.Log("Interstitial ad loaded !!" + ad.GetResponseInfo());

            rewardedAd = ad;
            RewardedAdEvents(rewardedAd);

        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // ���߿� ���� ���� ���� ���⿡ ��������
                Debug.Log("�÷��̾�� ���� ������ �־����ϴ�");

            });
        }
        else
        {
            Debug.Log("������ ���� �غ���� �ʾҽ��ϴ�.");
        }
    }

    public void RewardedAdEvents(RewardedAd ad) 
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    #endregion
}
