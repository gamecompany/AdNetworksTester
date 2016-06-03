#if USE_VUNGLE

using UnityEngine;
using System.Collections.Generic;

//using Prime31;

public static class VungleAdNetwork
{
	[RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
    	VungleSettings settings = VungleSettings.Instance;
		//Your App IDs can be found in the Vungle Dashboard on your apps' pages
		Vungle.init(settings.androidAppId, settings.iosAppId, settings.windowsAppId);
    }

	public static bool PlayAd(string placementName, bool isIncentivized)
    {
		if (Vungle.isAdvertAvailable())
		{
			Dictionary<string, object> options = new Dictionary<string, object> ();
			options ["incentivized"] = isIncentivized;
			Vungle.playAdWithOptions(options);
			return true;
		}
		return false;
	}

    public static bool PlayAd()
    {
        if (Vungle.isAdvertAvailable())
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["incentivized"] = true;
            Vungle.playAdWithOptions(options);
            return true;
        }
        return false;
    }

    public static bool PlayAd(UnityEngine.Events.UnityAction onAdPlay, UnityEngine.Events.UnityAction adNotAviable)
    {
        if (Vungle.isAdvertAvailable())
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            options["incentivized"] = true;
            Vungle.playAdWithOptions(options);
            // Callback
            if (null != onAdPlay) onAdPlay.Invoke();
            return true;
        }
        
        // Failed debug it
        if (null != adNotAviable) adNotAviable.Invoke();

        return false;
    }
}

#endif
