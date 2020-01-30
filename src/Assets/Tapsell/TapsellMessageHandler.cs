using System;
using TapsellSDK;
using UnityEngine;

public class TapsellMessageHandler : MonoBehaviour {

	public void NotifyAdAvailable (String body) {
		TapsellAd result = new TapsellAd ();
		result = JsonUtility.FromJson<TapsellAd> (body);
		Debug.Log ("notifyAdAvailable:" + result.zoneId + ":" + result.adId);
		Tapsell.OnAdAvailable (result);
	}

	public void NotifyBannerFilled (String zoneId) {
		Debug.Log ("notifyBannerFilled:" + zoneId);
		Tapsell.OnBannerRequestFilled (zoneId);
	}

	public void NotifyNativeBannerFilled (String body) {
		TapsellNativeBannerAd result = new TapsellNativeBannerAd ();
		result = JsonUtility.FromJson<TapsellNativeBannerAd> (body);
		Debug.Log ("notifyNativeBannerFilled:" + result.zoneId + ":" + result.adId);
		Tapsell.OnNativeBannerFilled (result);
	}

	public void NotifyError (String body) {
		TapsellError error = new TapsellError ();
		error = JsonUtility.FromJson<TapsellError> (body);
		Debug.Log ("notifyError:" + error.zoneId + ":" + error.message);
		Tapsell.OnError (error);
	}

	public void NotifyNoAdAvailable (String zoneId) {
		Debug.Log ("notifyNoAdAvailable:" + zoneId);
		Tapsell.OnNoAdAvailable (zoneId);
	}

	public void NotifyExpiring (String body) {
		TapsellAd result = new TapsellAd ();
		result = JsonUtility.FromJson<TapsellAd> (body);
		Debug.Log ("notifyExpiring:" + result.zoneId + ":" + result.adId);
		Tapsell.OnExpiring (result);
	}

	public void NotifyNoNetwork (String zoneId) {
		Debug.Log ("notifyNoNetwork:" + zoneId);
		Tapsell.OnNoNetwork (zoneId);
	}

	public void NotifyHideBanner (String zoneId) {
		Debug.Log ("notifyHideBanner:" + zoneId);
		Tapsell.OnHideBanner (zoneId);
	}

	public void NotifyOpened (String body) {
		TapsellAd result = new TapsellAd ();
		result = JsonUtility.FromJson<TapsellAd> (body);
		Debug.Log ("notifyOpened:" + result.zoneId + ":" + result.adId);
		Tapsell.OnOpened (result);
	}

	public void NotifyClosed (String body) {
		TapsellAd result = new TapsellAd ();
		result = JsonUtility.FromJson<TapsellAd> (body);
		Debug.Log ("notifyClosed:" + result.zoneId + ":" + result.adId);
		Tapsell.OnClosed (result);
	}

	public void NotifyShowFinished (String body) {
		TapsellAdFinishedResult result = new TapsellAdFinishedResult ();
		result = JsonUtility.FromJson<TapsellAdFinishedResult> (body);
		Debug.Log ("notifyShowFinished:" + result.zoneId + ":" + result.adId + ":" + result.rewarded);
		Tapsell.OnAdShowFinished (result);
	}

}