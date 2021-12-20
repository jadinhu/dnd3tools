/**
 * PlayfabLoginAndRegister.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 18/12/18 (dd/mm/yy)
 * Revised on: 18/12/18 (dd/mm/yy)
 */
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabController : MonoBehaviour
{
    public static void SendUserData(string key, string data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {key, data}
        }
        },
        result => Debug.Log("success UpdateUserData"),
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public static string GetUserData(string key)
    {
        string value = null;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = FacebookAndPlayFabInfo.userPlayFabId,
            Keys = new List<string>
            {
                key
            }
        }, result =>
        {
            if (result.Data == null)
                Debug.Log("result.Data null");
            else
            {
                Debug.Log("data: " + result.Data[key].Value.ToString());
                value = result.Data[key].Value.ToString();
            }
        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
        return value;
    }
}
