/**
 * AdminController.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 23/10/17 (dd/mm/yy)
 * Revised on: 27/10/17 (dd/mm/yy)
 */
/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;

/// <summary>
/// Handles the Admin Login and his functions
/// </summary>
public class AdminController : MonoBehaviour
{
    /// <summary>
    /// The Username and Password panel
    /// </summary>
    [SerializeField]
    GameObject usernameAndPasswordPanel;
    /// <summary>
    /// The alert text in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Text alertSignInText;
    /// <summary>
    /// The result text in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Text resultText;
    /// <summary>
    /// The username field in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    InputField userSignInText;
    /// <summary>
    /// The password field in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    InputField passwordSignInText;
    /// <summary>
    /// The loading text in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Text loadingText;

    /// <summary>
    /// Temp row data list, used in <see cref="RegisterPlayer(string[])"/> and <see cref="Save"/>
    /// </summary>
    List<string[]> rowData = new List<string[]>();
    /// <summary>
    /// Temp players list, used in <see cref="RegisterPlayer(string[])"/> and <see cref="Save"/>
    /// </summary>
    List<PlayFab.AdminModels.PlayerProfile> playerProfileList = new List<PlayFab.AdminModels.PlayerProfile>(0);
    /// <summary>
    /// Temp counter to knows if all players in <see cref="playerProfileList"/> are putted in <see cref="RegisterPlayer(string[])"/>
    /// </summary>
    int playerCounter;

    #region Log In
    /// <summary>
    /// Try login with username and password in Playfab
    /// </summary>
    public void Login()
    {
        resultText.text = "";
        playerCounter = 0;
        SetElementsInteractable(usernameAndPasswordPanel, false);
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest()
        {
            Username = userSignInText.text,
            Password = passwordSignInText.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.LoginWithPlayFab(request, LoginSuccess, LoginFail);
    }

    /// <summary>
    /// Update <see cref="FacebookAndPlayFabInfo.userPlayFabId"/>, save local login fields (remember login) if 
    /// <see cref="rememberLoginToggle"/> is on, and loads the Game scene
    /// </summary>
    void LoginSuccess(LoginResult result)
    {
        FacebookAndPlayFabInfo.userPlayFabId = result.PlayFabId;
        GetPrizeReport();
    }

    /// <summary>
    /// Shows a fail message from <see cref="Login"/> and calls <see cref="SetElementsInteractable(GameObject, bool)"/> 
    /// with <see cref="usernameAndPasswordPanel"/> and true
    /// </summary>
    void LoginFail(PlayFabError result)
    {
        print("Fail trying log in:\n" + result);
        switch (result.HttpCode)
        {
            case 1001:
                alertSignInText.text = "Account Not Found";
                break;
            case 1002:
                alertSignInText.text = "Account Banned";
                break;
            case 1003:
                alertSignInText.text = "Invalid Username Or Password";
                break;
            case 6666:
                alertSignInText.text = "Player Already Logged";
                break;
            default:
                if (result.ToString().Contains("Username must be between 3 and 20"))
                {
                    alertSignInText.text = "Username size is 3 to 20";
                }
                else if (result.ToString().Contains("Password must be between 6"))
                {
                    alertSignInText.text = "Password size is 6 to 100";
                }
                else if (result.ToString().Contains("401 Unauthorized"))
                {
                    alertSignInText.text = "Wrong Passworod";
                }
                break;
        }
        SetElementsInteractable(usernameAndPasswordPanel, true);
    }
    #endregion Log In

    /// <summary>
    /// Set the attribute "interactable" of all children <see cref="UnityEngine.UI"/> elements in "panel" parameter with "active" parameter
    /// </summary>
    /// <param name="panel">The panel of the children elements</param>
    /// <param name="active">Valeu to set "interactable" in all elements</param>
    public void SetElementsInteractable(GameObject panel, bool active)
    {
        Selectable[] list;
        list = panel.GetComponentsInChildren<Selectable>();
        foreach (Selectable selectable in list)
            selectable.interactable = active;
    }

    /// <summary>
    /// Set the header of the report table and calls <see cref="GetAllPlayers(string)"/>
    /// </summary>
    void GetPrizeReport()
    {
        string[] rowDataTemp = new string[4];
        rowDataTemp[0] = "Player ID";
        rowDataTemp[1] = "Email";
        rowDataTemp[2] = "Date";
        rowDataTemp[3] = "Item ID";
        rowData.Add(rowDataTemp);
        GetAllPlayers(null);
    }

    #region Get All Players
    /// <summary>
    /// Calls <see cref=" PlayFabAdminAPI.GetPlayersInSegment(PlayFab.AdminModels.GetPlayersInSegmentRequest, Action{PlayFab.AdminModels.GetPlayersInSegmentResult}, Action{PlayFabError}, object, Dictionary{string, string})"/>
    /// </summary>
    /// <param name="continuationToken">Token to continue next page (null if is the first page)</param>
    void GetAllPlayers(string continuationToken)
    {
        PlayFab.AdminModels.GetPlayersInSegmentRequest request;
        if (string.IsNullOrEmpty(continuationToken))
        {
            request = new PlayFab.AdminModels.GetPlayersInSegmentRequest()
            {
                MaxBatchSize = 10000,
                SecondsToLive = 300,
                SegmentId = "563C090CF7DC80B2"
            };
        }
        else
        {
            request = new PlayFab.AdminModels.GetPlayersInSegmentRequest()
            {
                ContinuationToken = continuationToken,
                MaxBatchSize = 10000,
                SecondsToLive = 300,
                SegmentId = "563C090CF7DC80B2"
            };
        }
        PlayFabAdminAPI.GetPlayersInSegment(request, GetAllPlayersSuccessful, GetAllPlayersFail);
    }

    /// <summary>
    /// Add <see cref="PlayFab.AdminModels.GetPlayersInSegmentResult.PlayerProfiles"/> to <see cref="playerProfileList"/> and calls again
    /// <see cref="GetAllPlayers(string)"/> until all players pages are in <see cref="playerProfileList"/>. At this point, calls <see cref="GetItemPrize(string)"/>
    /// for all players in <see cref="playerProfileList"/>
    /// </summary>
    void GetAllPlayersSuccessful(PlayFab.AdminModels.GetPlayersInSegmentResult result)
    {
        playerProfileList.AddRange(result.PlayerProfiles);
        if (!string.IsNullOrEmpty(result.ContinuationToken))
        {
            print("ContinuationToken On!");
            GetAllPlayers(result.ContinuationToken);
            return;
        }
        print("Players List size: " + playerProfileList.Count);
        StartCoroutine(GetItemPrizeCoroutine());
    }

    IEnumerator GetItemPrizeCoroutine()
    {
        int size = playerProfileList.Count;
        int counter = 0;
        foreach (PlayFab.AdminModels.PlayerProfile player in playerProfileList)
        {
            counter++;
            loadingText.text = "Loading player's data: " + counter + "/" + size;
            GetItemPrize(player.PlayerId);
            yield return new WaitForSeconds(1.5f);
        }
        print("GetItemPrizeCoroutine Finished.");
    }

    /// <summary>
    /// Print the error message about <see cref="GetAllPlayers(string)"/>
    /// </summary>
    void GetAllPlayersFail(PlayFabError error)
    {
        print("TestDataReport error: " + error.Error);
    }
    #endregion Get All Players

    #region GetPrize
    /// <summary>
    /// Support class for <see cref="ExecuteCloudScriptResult"/> in <see cref="GetItemPrizeSuccessful(ExecuteCloudScriptResult)"/>
    /// </summary>
    [Serializable]
    class ItemPrizeResult
    {
        public string Result;
    }

    /// <summary>
    /// Calls "getPrizeReport" in cloudscript
    /// </summary>
    /// <param name="playerId">Player Id to pass in the execution</param>
    void GetItemPrize(string playerId)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "getPrizeReport",
            FunctionParameter = new
            {
                PlayFabId = playerId
            },
        };
        PlayFabClientAPI.ExecuteCloudScript(request, GetItemPrizeSuccessful, GetItemPrizeFail);
    }

    /// <summary>
    /// Calls <see cref="RegisterPlayer(string[])"/> with the result. If all players in <see cref="playerProfileList"/> are registered,
    /// calls <see cref="Save"/>
    /// </summary>
    /// <param name="result"></param>
    void GetItemPrizeSuccessful(ExecuteCloudScriptResult result)
    {
        playerCounter++;
        if (result.Error != null)
        {
            print("result.Error.Error: " + result.Error.Error);
            print("result.Error.Message: " + result.Error.Message);
            print("result.Error.StackTrace: " + result.Error.StackTrace);
        }
        print("result.FunctionResult: " + result.FunctionResult);
        if (result.FunctionResult == null || string.IsNullOrEmpty(result.FunctionResult.ToString()))
        {
            if (playerCounter == playerProfileList.Count)
                Save();
            return;
        }

        ItemPrizeResult itemPrizeResult = JsonUtility.FromJson<ItemPrizeResult>(result.FunctionResult.ToString());
        print("Result: " + itemPrizeResult.Result);
        string[] data = itemPrizeResult.Result.Split(',');
        RegisterPlayer(data);
        if (playerCounter == playerProfileList.Count)
            Save();
    }

    /// <summary>
    /// Print the error message about <see cref="GetItemPrize(string)"/>
    /// </summary>
    void GetItemPrizeFail(PlayFabError error)
    {
        print("TestDataReport error: " + error.Error);
    }
    #endregion GetPrize

    /// <summary>
    /// Registar player data in <see cref="rowData"/>
    /// </summary>
    /// <param name="data">player data</param>
    void RegisterPlayer(string[] data)
    {
        int loops = (data.Length - 2) / 2;
        for (int i = 0; i < loops; i++)
        {
            string[] rowDataTemp = new string[4];
            rowDataTemp[0] = data[0];
            rowDataTemp[1] = data[1];
            rowDataTemp[2] = data[2 * (i + 1)];
            rowDataTemp[3] = data[2 * (i + 1) + 1];
            rowData.Add(rowDataTemp);
        }
    }

    /// <summary>
    /// Save a file with <see cref="rowData"/> in <see cref="GetPath"/>
    /// </summary>
    void Save()
    {
		resultText.text = "Trying to Save...";
        try
        {
            string[][] output = new string[rowData.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = rowData[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, output[index]));

            string filePath = GetPath();

            StreamWriter outStream = File.CreateText(filePath);
            outStream.WriteLine(sb);
            outStream.Close();
            SetElementsInteractable(usernameAndPasswordPanel, true);
            resultText.text = "File exported.";
        }
        catch (Exception e)
        {
            resultText.text = e.Message;
        }
    }

    /// <summary>
    /// System path to save <see cref="rowData"/>
    /// </summary>
    string GetPath()
    {
        DateTime date = new DateTime();
#if UNITY_EDITOR
        return "C:///prize report/" + "Saved_data " + String.Format("{0:dd-MM-yyyy-hh-mm-ss}", date) + ".txt";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "Saved_data " + String.Format("{0:dd-MM-yyyy-hh-mm-ss}", date) + ".txt";
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/" + "Saved_data " + String.Format("{0:dd-MM-yyyy-hh-mm-ss}", date) + ".txt";
#else
		return Application.dataPath + "/" + "Saved_data " + String.Format("{0:dd-MM-yyyy-hh-mm-ss}", date) + ".txt";
#endif
    }
}*/