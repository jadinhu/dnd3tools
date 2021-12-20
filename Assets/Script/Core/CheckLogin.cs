/**
 * CheckLogin.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 06/03/19 (dd/mm/yy)
 * Revised on: 14/12/21 (dd/mm/yy)
 */
using UnityEngine;

/// <summary>
/// Manages the login information between scenes
/// </summary>
public class CheckLogin : MonoBehaviour
{
    /// <summary>
    /// If auto login will be enable
    /// </summary>
    static bool autoLogin = true;
    /// <summary>
    /// If there is a account logged
    /// </summary>
    static bool logged = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// If auto login happens at this instance
    /// </summary>
    /// <returns>True if auto login happens</returns>
    public static bool IsAutoLogin()
    {
        return PlayerPrefs.GetString("autoLogin") != null && PlayerPrefs.GetString("autoLogin").Equals("auto");
    }

    /// <summary>
    /// Changes auto login to enable or disable
    /// </summary>
    /// <param name="auto">true to turn auto login enable</param>
    public static void SetAutoLogin(bool auto)
    {
        if (auto)
            PlayerPrefs.SetString("autoLogin", "auto");
        else
            PlayerPrefs.SetString("autoLogin", "");
    }

    /// <summary>
    /// Returns if there is a account logged
    /// </summary>
    /// <returns><see cref="logged"/></returns>
    public static bool IsLogged()
    {
        return logged;
    }

    /// <summary>
    /// Sets if there is a account logged
    /// </summary>
    /// <param name="log">true if there is logged</param>
    public static void SetLogged(bool log)
    {
        logged = log;
    }
}
