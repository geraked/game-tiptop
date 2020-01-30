using UnityEngine;

#if UNITY_ANDROID
using TapsellSDK;
#endif

public class TapsellStandardBanner : MonoBehaviour
{
    private const string ID = "tehheqmjlacpqiqtmsnjdrpplpbnfighrhbempojslttssqfbpbtsqrijgmdlftkogatdd";
    public const string ZoneID = "5d6d6f0b607a790001e2a2a4";

    public static bool IsRequesting = true;

    void Start()
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Tapsell.Initialize(ID);
            RequestBannerAd();
            Hide();
        }
#endif
        if (Application.platform != RuntimePlatform.Android)
        {
            IsRequesting = false;
        }
    }

    public static void RequestBannerAd()
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            IsRequesting = true;
            Tapsell.RequestBannerAd(ZoneID, BannerType.BANNER_320x50, Gravity.BOTTOM, Gravity.CENTER,
              (string zoneId) =>
              {
                  Debug.Log("on Ad Available");
                  IsRequesting = false;
              },
              (string zoneId) =>
              {
                  Debug.Log("no Ad Available");
                  IsRequesting = false;
              },
              (TapsellError error) =>
              {
                  Debug.Log(error.message);
                  IsRequesting = false;
              },
              (string zoneId) =>
              {
                  Debug.Log("no Network");
                  IsRequesting = false;
              },
              (string zoneId) =>
              {
                  Debug.Log("Hide Banner");
                  IsRequesting = false;
              });
        }
#endif
    }

    public static void Hide()
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Tapsell.HideBannerAd(ZoneID);
        }
#endif
    }

    public static void Show()
    {
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Tapsell.ShowBannerAd(ZoneID);
        }
#endif
    }
}