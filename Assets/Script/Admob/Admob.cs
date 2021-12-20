/**
* Admob.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 10/11/18 (dd/mm/yy)
* Revised on: 13/12/21 (dd/mm/yy)
*/
using GoogleMobileAds.Api;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the AdMob usage for ADS
/// </summary>
public class Admob : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static Admob Instance { get; private set; }
    /// <summary>
    /// If the AdMob is active to work or disabled
    /// </summary>
    [SerializeField]
    bool admobOn;
    /// <summary>
    /// The AdMob ID for android plataform
    /// </summary>
    [SerializeField]
    string idAdAndroid;
    /// <summary>
    /// The AdMob ID for windows plataform
    /// </summary>
    [SerializeField]
    string idAdWindows;
    /// <summary>
    /// Full-screen ADS that cover the interface of their host app
    /// </summary>
    InterstitialAd interstitial;
    /// <summary>
    /// The requester of <see cref="interstitial"/>
    /// </summary>
    AdRequest requestInterstitial;
    /// <summary>
    /// The current AdMob ID used in current plataform
    /// </summary>
    string adUnitId;

    void Start()
    {
        Setup();
    }

    /// <summary>
    /// Instanciate the Singleton <see cref="Instance"/>, check if AdMob must work (<see cref="admobOn"/>),
    /// get the <see cref="adUnitId"/> and Invokes <see cref="PrepareInterstitial"/>
    /// </summary>
    void Setup()
    {
        if (Instance == null)
            Instance = this;
        if (!admobOn)
            return;
        if (Application.platform == RuntimePlatform.Android)
            adUnitId = idAdAndroid;
        else
            adUnitId = idAdWindows;
        Invoke("PrepareAds", 1f);
    }

    /// <summary>
    /// Shows the <see cref="interstitial"/>
    /// </summary>
    public void ShowInterstitial()
    {
        if (!admobOn)
            return;
        RequestInterstitial();
    }

    /// <summary>
    /// Remove and destroy the <see cref="interstitial"/>
    /// </summary>
    public void CleanAll()
    {
        if (!admobOn)
            return;
        interstitial.Destroy();
    }

    /// <summary>
    /// Returns true if <see cref="interstitial"/> is loaded
    /// </summary>
    public bool InterstitialIsLoaded()
    {
        return interstitial.IsLoaded();
    }

    /// <summary>
    /// Starts a coroutine <see cref="LoadInterstitial(InterstitialAd)"/>
    /// </summary>
    void RequestInterstitial()
    {
        // Wait cache to show
        StartCoroutine(LoadInterstitial(interstitial));
    }

    /// <summary>
    /// Waits the <see cref="interstitial"/> are loaded and shows it
    /// </summary>
    /// <param name="interstitial">the interstitial to load</param>
    IEnumerator LoadInterstitial(InterstitialAd interstitial)
    {
        while (!interstitial.IsLoaded())
        {
            yield return null; // wait 1 second to try again
        }
        interstitial.Show();
    }

    /// <summary>
    /// Obligatory method implementation of AdMob API, called on ADS closed.
    /// </summary>
    void Interstitial_OnAdClosed(object sender, System.EventArgs e) { }

    /// <summary>
    /// Prepare the <see cref="interstitial"/> to be called (unused, but keeped for reference)
    /// </summary>
    void PrepareInterstitial()
    {
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Add Method to Listener OnAdClosed
        interstitial.OnAdClosed += Interstitial_OnAdClosed;
        // Create an empty ad request.
        requestInterstitial = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(requestInterstitial);
    }
}