public class AdvertisementVungle : AdvertisementBase
{
	private const string API = "Vungle";
	
	public AdvertisementVungle(string key_android, string key_ios): base(key_android, key_ios)
	{
	}
	
	// Obtem o video
	public override void fetchVideo(bool force)
	{
		try
		{
			if (!force && tried_fetching_video)
					return;
				
			base.fetchVideo(force);
			
			if (Info.IsEditor())
				return;
					
			#if UNITY_ANDROID
					Initializate.AddPrefab("VungleAndroidManager", typeof(VungleAndroidManager));
			
					VungleAndroid.init(key);
			#elif UNITY_IPHONE
					Initializate.AddPrefab("VungleManager", typeof(VungleManager));
			
					VungleBinding.startWithAppId(key);
			#endif
		}
		catch
		{
			Error(API, ERROR_STARTUP_OBJECT);
		}
	}
	
	public override bool isVideoAvailable()
	{
		try
		{
			if (Info.IsEditor())
				return false;
				
			#if UNITY_ANDROID
				return VungleAndroid.isVideoAvailable();
	
			#elif UNITY_IPHONE
				return VungleBinding.isAdAvailable();
			#else
				return false;
			#endif
		}
		catch
		{
			return Error(API, ERROR_CHECK_VIDEO);
		}
	}
	
	// Mostra o video
	public override bool showVideo()
	{
		if (!isVideoAvailable())
			return false;
		
		try
		{
			if (Info.IsEditor())
				return false;
				
			#if UNITY_ANDROID
				VungleAndroid.displayIncentivizedAdvert(false);
				return true;
			
			#elif UNITY_IPHONE
				VungleBinding.playIncentivizedAd("user", false);
				return true;
			#else
				return false;
			#endif
		}
		catch
		{
			return Error(API, ERROR_PLAY_VIDEO);
		}
	}
}
