using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace TapsellSDK {

	public class Gravity {
		public static int TOP = 1;
		public static int BOTTOM = 2;
		public static int LEFT = 3;
		public static int RIGHT = 4;
		public static int CENTER = 5;
	}

	public class BannerType {
		public static int BANNER_320x50 = 1;
		public static int BANNER_320x100 = 2;
		public static int BANNER_250x250 = 3;
		public static int BANNER_300x250 = 4;
	}

	[Serializable]
	public class TapsellError {
		public string message;
		public string zoneId;
	}

	[Serializable]
	public class TapsellAd {
		public string adId;
		public string zoneId;
	}

	[Serializable]
	public class TapsellNativeBannerAd {
		public string adId;
		public string zoneId;
		public string title;
		public string description;
		public string iconUrl;
		public string callToActionText;
		public string portraitStaticImageUrl;
		public string landscapeStaticImageUrl;

		public Texture2D portraitBannerImage;
		public Texture2D landscapeBannerImage;
		public Texture2D iconImage;

		public string GetTitle () {
			return title;
		}

		public string GetDescription () {
			return description;
		}

		public string GetCallToAction () {
			return callToActionText;
		}

		public Texture2D GetPortraitBannerImage () {
			return portraitBannerImage;
		}

		public Texture2D GetLandscapeBannerImage () {
			return landscapeBannerImage;
		}

		public Texture2D GetIcon () {
			return iconImage;
		}

		public void Clicked () {
			Tapsell.OnNativeBannerAdClicked (this.zoneId, this.adId);
		}
	}

	[Serializable]
	public class TapsellAdFinishedResult {
		public string adId;
		public string zoneId;
		public bool completed;
		public bool rewarded;
	}

	public class TapsellShowOptions {
		public static int ROTATION_LOCKED_PORTRAIT = 1;
		public static int ROTATION_LOCKED_LANDSCAPE = 2;
		public static int ROTATION_UNLOCKED = 3;
		public static int ROTATION_LOCKED_REVERSED_PORTRAIT = 4;
		public static int ROTATION_LOCKED_REVERSED_LANDSCAPE = 5;

		public bool backDisabled = false;
		public bool immersiveMode = false;
		public int rotationMode = ROTATION_UNLOCKED;
		public bool showDialog = false;
	}

	public class Tapsell {

#if UNITY_IOS && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern void _TSInitialize (string appkey);
		[DllImport ("__Internal")]
		private static extern void _TSRequestAdForZone (string zoneId, string cached);
		[DllImport ("__Internal")]
		private static extern void _TSShowAd (string adId, string backDisabled, int rotationMode, string showDialog);
		[DllImport ("__Internal")]
		private static extern string _TSGetVersion ();
		[DllImport ("__Internal")]
		private static extern void _TSSetDebugMode (string debugMode);
		[DllImport ("__Internal")]
		private static extern string _TSIsDebugMode ();
		[DllImport ("__Internal")]
		private static extern void _TSSetAppUserId (string appUserId);
		[DllImport ("__Internal")]
		private static extern string _TSGetAppUserId ();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass tapsell;
#endif

		private static Dictionary<string, Action<TapsellNativeBannerAd>> nativeBannerFilledPool =
			new Dictionary<string, Action<TapsellNativeBannerAd>> ();
		private static Dictionary<string, Action<string>> bannerFilledPool =
			new Dictionary<string, Action<string>> ();
		private static Dictionary<string, Action<TapsellAd>> adFilledPool =
			new Dictionary<string, Action<TapsellAd>> ();
		private static Dictionary<string, Action<TapsellError>> errorPool =
			new Dictionary<string, Action<TapsellError>> ();
		private static Dictionary<string, Action<TapsellAd>> expiringPool =
			new Dictionary<string, Action<TapsellAd>> ();
		private static Dictionary<string, Action<string>> noAdAvailablePool =
			new Dictionary<string, Action<string>> ();
		private static Dictionary<string, Action<string>> noNetworkPool =
			new Dictionary<string, Action<string>> ();
		private static Dictionary<string, Action<TapsellAd>> adOpenedPool =
			new Dictionary<string, Action<TapsellAd>> ();
		private static Dictionary<string, Action<TapsellAd>> adClosedPool =
			new Dictionary<string, Action<TapsellAd>> ();
		private static Dictionary<string, Action<string>> hideBannerPool =
			new Dictionary<string, Action<string>> ();

		private static Action<TapsellAdFinishedResult> adFinishedAction = null; //new Action<TapsellAdFinishedResult>();

#if UNITY_ANDROID && !UNITY_EDITOR
		private static MonoBehaviour mMonoBehaviour;
#endif

		private static GameObject tapsellManager = null;

		public static void Initialize (string key) {
			if (tapsellManager == null) {
				tapsellManager = new GameObject ("TapsellManager");
				UnityEngine.Object.DontDestroyOnLoad (tapsellManager);
				tapsellManager.AddComponent<TapsellMessageHandler> ();
			}

#if UNITY_ANDROID && !UNITY_EDITOR
			SetJavaObject ();
			tapsell.CallStatic ("initialize", key, "4.2.7.0");
#elif UNITY_IOS && !UNITY_EDITOR
			_TSInitialize (key);
#endif
		}

		private static void SetJavaObject () {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell = new AndroidJavaClass ("ir.tapsell.sdk.TapsellUnity");
#endif
		}

		public static void SetDebugMode (bool debug) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("setDebugMode", debug);
#elif UNITY_IOS && !UNITY_EDITOR
			string debugMode = "false";
			if (debug) {
				debugMode = "true";
			}
			_TSSetDebugMode (debugMode);
#endif
		}

		public static void SetMaxAllowedBandwidthUsage (int maxBpsSpeed) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("setMaxAllowedBandwidthUsage", maxBpsSpeed);
#elif UNITY_IOS && !UNITY_EDITOR
			// do nothing
#endif
		}

		public static void SetMaxAllowedBandwidthUsagePercentage (int maxPercentage) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("setMaxAllowedBandwidthUsagePercentage", maxPercentage);
#elif UNITY_IOS && !UNITY_EDITOR
			// do nothing
#endif
		}

		public static void ClearBandwidthUsageConstrains () {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("clearBandwidthUsageConstrains");
#elif UNITY_IOS && !UNITY_EDITOR
			// do nothing
#endif
		}

		public static void RequestAd (
			string zoneId,
			Boolean isCached,
			Action<TapsellAd> onAdAvailableAction,
			Action<string> onNoAdAvailableAction,
			Action<TapsellError> onErrorAction,
			Action<string> onNoNetworkAction,
			Action<TapsellAd> onExpiringAction) {

			RequestAd (
				zoneId, isCached, onAdAvailableAction, onNoAdAvailableAction, onErrorAction,
				onNoNetworkAction, onExpiringAction, null, null);
		}

		public static void RequestAd (
			string zoneId,
			Boolean isCached,
			Action<TapsellAd> onAdAvailableAction,
			Action<string> onNoAdAvailableAction,
			Action<TapsellError> onErrorAction,
			Action<string> onNoNetworkAction,
			Action<TapsellAd> onExpiringAction,
			Action<TapsellAd> onOpenedAction,
			Action<TapsellAd> onClosedAction) {

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

			if (adFilledPool.ContainsKey (zoneId)) {
				adFilledPool.Remove (zoneId);
			}

			if (errorPool.ContainsKey (zoneId)) {
				errorPool.Remove (zoneId);
			}

			if (noAdAvailablePool.ContainsKey (zoneId)) {
				noAdAvailablePool.Remove (zoneId);
			}

			if (noNetworkPool.ContainsKey (zoneId)) {
				noNetworkPool.Remove (zoneId);
			}

			if (expiringPool.ContainsKey (zoneId)) {
				expiringPool.Remove (zoneId);
			}

			if (adOpenedPool.ContainsKey (zoneId)) {
				adOpenedPool.Remove (zoneId);
			}

			if (adClosedPool.ContainsKey (zoneId)) {
				adClosedPool.Remove (zoneId);
			}

			adFilledPool.Add (zoneId, onAdAvailableAction);
			errorPool.Add (zoneId, onErrorAction);
			noAdAvailablePool.Add (zoneId, onNoAdAvailableAction);
			noNetworkPool.Add (zoneId, onNoNetworkAction);
			expiringPool.Add (zoneId, onExpiringAction);

			if (onOpenedAction != null) {
				adOpenedPool.Add (zoneId, onOpenedAction);
			}

			if (onClosedAction != null) {
				adClosedPool.Add (zoneId, onClosedAction);
			}
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("requestAd", zoneId, isCached);

#elif UNITY_IOS && !UNITY_EDITOR
			string cached = "false";
			if (isCached) {
				cached = "true";
			}
			_TSRequestAdForZone (zone, cached);
#else
			TapsellError error = new TapsellError ();
			error.zoneId = zoneId;
			error.message = "Tapsell ads are only available on Android and iOS platforms.";
			onErrorAction (error);
#endif
		}

		public static void RequestNativeBannerAd (
			MonoBehaviour monoBehaviour,
			string zoneId,
			Action<TapsellNativeBannerAd> onRequestFilled,
			Action<string> onNoAdAvailableAction,
			Action<TapsellError> onErrorAction,
			Action<string> onNoNetworkAction) {

#if UNITY_ANDROID && !UNITY_EDITOR
			mMonoBehaviour = monoBehaviour;

			if (nativeBannerFilledPool.ContainsKey (zoneId)) {
				nativeBannerFilledPool.Remove (zoneId);
			}

			if (errorPool.ContainsKey (zoneId)) {
				errorPool.Remove (zoneId);
			}

			if (noAdAvailablePool.ContainsKey (zoneId)) {
				noAdAvailablePool.Remove (zoneId);
			}

			if (noNetworkPool.ContainsKey (zoneId)) {
				noNetworkPool.Remove (zoneId);
			}

			nativeBannerFilledPool.Add (zoneId, onRequestFilled);
			errorPool.Add (zoneId, onErrorAction);
			noAdAvailablePool.Add (zoneId, onNoAdAvailableAction);
			noNetworkPool.Add (zoneId, onNoNetworkAction);

			tapsell.CallStatic ("requestNativeBannerAd", zoneId);
#else
			TapsellError error = new TapsellError ();
			error.zoneId = zoneId;
			error.message = "Native ads are only available on Android platform.";
			onErrorAction (error);
#endif
		}

		public static void RequestBannerAd (
			string zoneId,
			int bannerType,
			int horizontalGravity,
			int verticalGravity,
			Action<string> onRequestFilled,
			Action<string> onNoAdAvailableAction,
			Action<TapsellError> onErrorAction,
			Action<string> onNoNetworkAction,
			Action<string> onHideBannerAction) {

#if UNITY_ANDROID && !UNITY_EDITOR

			if (bannerFilledPool.ContainsKey (zoneId)) {
				bannerFilledPool.Remove (zoneId);
			}
			if (errorPool.ContainsKey (zoneId)) {
				errorPool.Remove (zoneId);
			}
			if (noAdAvailablePool.ContainsKey (zoneId)) {
				noAdAvailablePool.Remove (zoneId);
			}
			if (noNetworkPool.ContainsKey (zoneId)) {
				noNetworkPool.Remove (zoneId);
			}
			if (hideBannerPool.ContainsKey (zoneId)) {
				hideBannerPool.Remove (zoneId);
			}

			bannerFilledPool.Add (zoneId, onRequestFilled);
			errorPool.Add (zoneId, onErrorAction);
			noAdAvailablePool.Add (zoneId, onNoAdAvailableAction);
			noNetworkPool.Add (zoneId, onNoNetworkAction);
			hideBannerPool.Add (zoneId, onHideBannerAction);

			tapsell.CallStatic (
				"requestBannerAd", zoneId, bannerType, horizontalGravity, verticalGravity);
#else

			TapsellError error = new TapsellError ();
			error.zoneId = zoneId;
			error.message = "Banner ad is only available on android";
			onErrorAction (error);
#endif
		}

		public static void ShowBannerAd (string zoneId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("showBannerAd", zoneId);
#else
			Debug.LogError ("Banner ad is only available on android");
#endif
		}

		public static void HideBannerAd (string zoneId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("hideBannerAd", zoneId);
#else
			Debug.LogError ("Banner ad is only available on android");
#endif
		}

		public static void ShowAd (TapsellAd tapsellAd) {
			ShowAd (tapsellAd, null);
		}

		public static void ShowAd (
			TapsellAd tapsellAd,
			TapsellShowOptions showOptions) {
			if (object.ReferenceEquals (showOptions, null)) {
				showOptions = new TapsellShowOptions ();
			}

#if UNITY_ANDROID && !UNITY_EDITOR
			expiringPool.Remove (tapsellAd.zoneId);
			tapsell.CallStatic ("showAd",
				tapsellAd.adId,
				showOptions.backDisabled,
				showOptions.immersiveMode,
				showOptions.rotationMode,
				showOptions.showDialog);

#elif UNITY_IOS && !UNITY_EDITOR
			string bDisabled = "false";
			if (showOptions.backDisabled) {
				bDisabled = "true";
			}

			string sDialog = "false";

			if (showOptions.showDialog) {
				sDialog = "true";
			}
			_TSShowAd (tapsellAd.adId, bDisabled, showOptions.rotationMode, sDialog);
#endif
		}

		public static void SetRewardListener (
			Action<TapsellAdFinishedResult> onFinishedAction) {
			adFinishedAction = onFinishedAction;
		}

		public static String GetVersion () {
#if UNITY_ANDROID && !UNITY_EDITOR
			return tapsell.CallStatic<String> ("getVersion");
#elif UNITY_IOS && !UNITY_EDITOR
			return _TSGetVersion ();
#else
			return "NO SDK";
#endif
		}

		public static void OnAdAvailable (TapsellAd result) {
			if (adFilledPool.ContainsKey (result.zoneId)) {
				adFilledPool[result.zoneId] (result);
			}
		}
		public static void OnBannerRequestFilled (String zoneId) {
			if (bannerFilledPool.ContainsKey (zoneId)) {
				bannerFilledPool[zoneId] (zoneId);
			}
		}

		public static void OnError (TapsellError error) {
			if (errorPool.ContainsKey (error.zoneId)) {
				errorPool[error.zoneId] (error);
			}
		}

		public static void OnNoAdAvailable (String zoneId) {
			if (noAdAvailablePool.ContainsKey (zoneId)) {
				noAdAvailablePool[zoneId] (zoneId);
			}
		}

		public static void OnNoNetwork (String zoneId) {
			if (noNetworkPool.ContainsKey (zoneId)) {
				noNetworkPool[zoneId] (zoneId);
			}
		}

		public static void OnExpiring (TapsellAd result) {
			if (expiringPool.ContainsKey (result.zoneId)) {
				expiringPool[result.zoneId] (result);
			}
		}

		public static void OnAdShowFinished (TapsellAdFinishedResult result) {
			if (adFinishedAction != null) {
				adFinishedAction (result);
			}
		}

		public static void OnOpened (TapsellAd result) {
			if (adOpenedPool.ContainsKey (result.zoneId)) {
				adOpenedPool[result.zoneId] (result);
			}
		}

		public static void OnClosed (TapsellAd result) {
			if (adClosedPool.ContainsKey (result.zoneId)) {
				adClosedPool[result.zoneId] (result);
			}
		}

		public static void OnHideBanner (String zoneId) {
			if (hideBannerPool.ContainsKey (zoneId)) {
				hideBannerPool[zoneId] (zoneId);
			}
		}

		public static void OnNativeBannerFilled (TapsellNativeBannerAd result) {
#if UNITY_ANDROID && !UNITY_EDITOR
			string zoneId = result.zoneId;
			if (result != null) {
				if (mMonoBehaviour != null && mMonoBehaviour.isActiveAndEnabled) {
					mMonoBehaviour.StartCoroutine (LoadNativeBannerAdImages (result));
				} else {
					if (errorPool.ContainsKey (zoneId)) {
						TapsellError error = new TapsellError ();
						error.zoneId = zoneId;
						error.message = "Invalid MonoBehaviour Object";
						errorPool[zoneId] (error);
					}
				}
			} else {
				if (errorPool.ContainsKey (zoneId)) {
					TapsellError error = new TapsellError ();
					error.zoneId = zoneId;
					error.message = "Invalid Result";
					errorPool[zoneId] (error);
				}
			}
#endif
		}

		static IEnumerator LoadNativeBannerAdImages (TapsellNativeBannerAd result) {

			if (result.iconUrl != null && !result.iconUrl.Equals ("")) {
				UnityWebRequest wwwIcon = UnityWebRequestTexture.GetTexture (result.iconUrl);
				yield return wwwIcon.SendWebRequest ();
				if (wwwIcon.isNetworkError || wwwIcon.isHttpError) {
					Debug.Log (wwwIcon.error);
				} else {
					result.iconImage = ((DownloadHandlerTexture) wwwIcon.downloadHandler).texture;
				}
			}

			if (result.portraitStaticImageUrl != null && !result.portraitStaticImageUrl.Equals ("")) {
				UnityWebRequest wwwPortrait = UnityWebRequestTexture.GetTexture (result.portraitStaticImageUrl);
				yield return wwwPortrait.SendWebRequest ();
				if (wwwPortrait.isNetworkError || wwwPortrait.isHttpError) {
					Debug.Log (wwwPortrait.error);
				} else {
					result.portraitBannerImage = ((DownloadHandlerTexture) wwwPortrait.downloadHandler).texture;
				}
			}

			if (result.landscapeStaticImageUrl != null && !result.landscapeStaticImageUrl.Equals ("")) {
				UnityWebRequest wwwLandscape = UnityWebRequestTexture.GetTexture (result.landscapeStaticImageUrl);
				yield return wwwLandscape.SendWebRequest ();
				if (wwwLandscape.isNetworkError || wwwLandscape.isHttpError) {
					Debug.Log (wwwLandscape.error);
				} else {
					result.landscapeBannerImage = ((DownloadHandlerTexture) wwwLandscape.downloadHandler).texture;
				}
			}

			if (nativeBannerFilledPool.ContainsKey (result.zoneId)) {
				nativeBannerFilledPool[result.zoneId] (result);
			}
		}

		public static void OnNativeBannerAdClicked (string zoneId, string adId) {
#if UNITY_ANDROID && !UNITY_EDITOR
			tapsell.CallStatic ("onNativeBannerAdClicked", zoneId, adId);
#endif
		}
	}
}