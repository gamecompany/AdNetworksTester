using UnityEngine;
using System;
using System.Collections;

public class MoPubManager : MonoBehaviour
{
    static bool bInitialized = false;

    void Awake()
    {
        if (!bInitialized)
        {
            bInitialized = true;
            MoPub.createBanner(BANNER_ADUNIT_ID, MoPubAdPosition.BottomCenter);
        }
    }
}
