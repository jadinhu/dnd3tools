/**
 * PlayfabLoginAndRegister.cs
 * Created by: Jadson Almeida [jadson.sistemas@gmail.com]
 * Created on: 08/09/17 (dd/mm/yy)
 * Revised on: 23/03/19 (dd/mm/yy)
 */
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the behaviors of the initial menu and it components. Like login, register and password recovery
/// </summary>
public class PlayfabLoginAndRegister : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of this class
    /// </summary>
    public static PlayfabLoginAndRegister Instance { get; private set; }
    /// <summary>
    /// List of buttons about keyboard workaround for WebGL
    /// </summary>
    [SerializeField]
    List<GameObject> webKeyboardButtons = new List<GameObject>(2);
    /// <summary>
    /// The alert text in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Text alertSignInText;
    /// <summary>
    /// The username field in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    InputField userSignInText;
    /// <summary>
    /// The username image alert in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Image userSignInImage;
    /// <summary>
    /// The password field in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    InputField passwordSignInText;
    /// <summary>
    /// The password image alert in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Image passwordSignInImage;
    /// <summary>
    /// The remember toggle in <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    [SerializeField]
    Toggle rememberLoginToggle;
    /// <summary>
    /// The alert text in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    Text alertRegisterText;
    /// <summary>
    /// The alert text about successful in <see cref="registerPanel"/>. Necessary for orkaround the unsync order of events behavior
    /// </summary>
    [SerializeField]
    Text alertRegisterSuccessfulText;
    /// <summary>
    /// The username field in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    InputField userRegisterText;
    /// <summary>
    /// The username image alert in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    Image userRegisterImage;
    /// <summary>
    /// The email field in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    InputField emailRegisterText;
    /// <summary>
    /// The email image alert in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    Image emailRegisterImage;
    /// <summary>
    /// The password field in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    InputField passwordRegisterText;
    /// <summary>
    /// The password image alert in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    Image passwordRegisterImage;
    /// <summary>
    /// The password repeat field in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    InputField passwordRepeatRegisterText;
    /// <summary>
    /// The password repeat image alert in <see cref="registerPanel"/>
    /// </summary>
    [SerializeField]
    Image passwordRepeatRegisterImage;
    /// <summary>
    /// The alert text in <see cref="passwordRecoveryPanel"/>
    /// </summary>
    [SerializeField]
    Text alertPasswordRecoveryText;
    /// <summary>
    /// The password field in <see cref="passwordRecoveryPanel"/>
    /// </summary>
    [SerializeField]
    InputField passwordRecoveryText;
    /// <summary>
    /// The email image alert in <see cref="passwordRecoveryPanel"/>
    /// </summary>
    [SerializeField]
    Image passwordRecoveryImage;
    /// <summary>
    /// The Home panel
    /// </summary>
    [SerializeField]
    GameObject homePanel;
    /// <summary>
    /// The Username and Password panel
    /// </summary>
    [SerializeField]
    GameObject usernameAndPasswordPanel;
    /// <summary>
    /// The Register panel
    /// </summary>
    [SerializeField]
    GameObject registerPanel;
    /// <summary>
    /// The Password Recovery panel
    /// </summary>
    [SerializeField]
    GameObject passwordRecoveryPanel;
    /// <summary>
    /// The Loading spellbook panel
    /// </summary>
    [SerializeField]
    GameObject loadingPanel;
    /// <summary>
    /// The facebook login button
    /// </summary>
    [SerializeField]
    Button facebookButton;

    void Start()
    {
        Instance = this;
#if UNITY_STANDALONE
        Screen.SetResolution(500, 800, false);
        Screen.fullScreen = false;
#endif
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            webKeyboardButtons[0].SetActive(false);
            webKeyboardButtons[1].SetActive(false);
        }
        if (Application.platform != RuntimePlatform.Android)
            facebookButton.interactable = false;
        EnableUsernameAndPasswordPanel(true);
    }

    /// <summary>
    /// Set the attribute "interactable" of all children <see cref="UnityEngine.UI"/> elements in "panel" parameter with "active" parameter
    /// </summary>
    /// <param name="panel">The panel of the children elements</param>
    /// <param name="active">Valeu to set "interactable" in all elements</param>
    public void SetElementsInteractable(GameObject panel, bool active)
    {
        Selectable[] list;
        if (panel == null)
            list = homePanel.GetComponentsInChildren<Selectable>();
        else
            list = panel.GetComponentsInChildren<Selectable>();
        foreach (Selectable selectable in list)
            selectable.interactable = active;
    }

    #region Enable / Disable Panels
    /// <summary>
    /// Enable or disable <see cref="homePanel"/>
    /// </summary>
    public void EnableHomePanel(bool active)
    {
        homePanel.SetActive(active);
    }

    /// <summary>
    /// Enable or disable <see cref="usernameAndPasswordPanel"/>
    /// </summary>
    public void EnableUsernameAndPasswordPanel(bool active)
    {
        if (active && !string.IsNullOrEmpty(PlayerPrefs.GetString("username")) && !string.IsNullOrEmpty(PlayerPrefs.GetString("userpassword")))
        {
            userSignInText.text = PlayerPrefs.GetString("username");
            passwordSignInText.text = PlayerPrefs.GetString("userpassword");
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString("rememberLoginOn")) && PlayerPrefs.GetString("rememberLoginOn").Equals("true"))
            {
                rememberLoginToggle.isOn = true;
                if (CheckLogin.IsAutoLogin())
                    Loading();
            }
            else
                rememberLoginToggle.isOn = false;
        }
        usernameAndPasswordPanel.SetActive(active);
    }

    void Loading()
    {
        loadingPanel.SetActive(true);
        Login();
    }

    /// <summary>
    /// Enable or disable <see cref="registerPanel"/>
    /// </summary>
    public void EnableRegisterPanel(bool active)
    {
        alertRegisterSuccessfulText.text = "";
        registerPanel.SetActive(active);
    }

    /// <summary>
    /// Enable or disable <see cref="passwordRecoveryPanel"/>
    /// </summary>
    public void EnablePasswordRecoveryPanel(bool active)
    {
        passwordRecoveryPanel.SetActive(active);
    }

    /// <summary>
    /// Enable or disable <see cref="loadingPanel"/>
    /// </summary>
    public void EnableLoadingPanel(bool active)
    {
        loadingPanel.SetActive(active);
    }
    #endregion Enable / Disable Panels

    public void SkipLogin()
    {
        CheckLogin.SetAutoLogin(false);
        LoadSpellbook();
    }

    #region Log In
    /// <summary>
    /// Try login with username and password in Playfab
    /// </summary>
    public void Login()
    {
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
        if (rememberLoginToggle.isOn)
        {
            PlayerPrefs.SetString("username", userSignInText.text);
            PlayerPrefs.SetString("userpassword", passwordSignInText.text);
            PlayerPrefs.SetString("rememberLoginOn", "true");
            CheckLogin.SetAutoLogin(true);
        }
        else
        {
            PlayerPrefs.SetString("username", "");
            PlayerPrefs.SetString("userpassword", "");
            PlayerPrefs.SetString("rememberLoginOn", "");
        }
        loadingPanel.SetActive(true);
        CheckLogin.SetLogged(true);
        LoadSpellbook();
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
                userSignInImage.enabled = true;
                break;
            case 1002:
                alertSignInText.text = "Account Banned";
                userSignInImage.enabled = true;
                break;
            case 1003:
                alertSignInText.text = "Invalid Username Or Password";
                userSignInImage.enabled = true;
                passwordSignInImage.enabled = true;
                break;
            case 6666:
                alertSignInText.text = "Player Already Logged";
                userSignInImage.enabled = true;
                break;
            default:
                if (result.ToString().Contains("Username must be between 3 and 20"))
                {
                    alertSignInText.text = "Username size is 3 to 20";
                    userSignInImage.enabled = true;
                }
                else if (result.ToString().Contains("Password must be between 6"))
                {
                    alertSignInText.text = "Password size is 6 to 100";
                    passwordSignInImage.enabled = true;
                }
                else if (result.ToString().Contains("401 Unauthorized"))
                {
                    alertSignInText.text = "Wrong Passworod";
                    passwordSignInImage.enabled = true;
                }
                break;
        }
        SetElementsInteractable(usernameAndPasswordPanel, true);
    }
    #endregion Log In

    #region Register
    /// <summary>
    /// Register the new account in Playfab
    /// </summary>
    public void Register()
    {
        SetElementsInteractable(registerPanel, false);
        alertRegisterSuccessfulText.text = "";
        // test password repeated
        if (!passwordRegisterText.text.Equals(passwordRepeatRegisterText.text))
        {
            passwordRepeatRegisterImage.enabled = true;
            alertRegisterText.text = "Repeat the same password";
            SetElementsInteractable(registerPanel, true);
            return;
        }
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
        {
            RequireBothUsernameAndEmail = true,
            Email = emailRegisterText.text,
            Username = userRegisterText.text,
            Password = passwordRegisterText.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFail);
    }

    /// <summary>
    /// Shows confirm message in <see cref="alertRegisterSuccessfulText"/>, cleans all fields of the <see cref="registerPanel"/> 
    /// and calls <see cref="SetElementsInteractable(GameObject, bool)"/> with <see cref="registerPanel"/> and true
    /// </summary>
    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("A new account was registered!");
        userRegisterText.text = "";
        emailRegisterText.text = "";
        passwordRegisterText.text = "";
        passwordRepeatRegisterText.text = "";
        alertRegisterSuccessfulText.text = "Account register successful!";
        SetElementsInteractable(registerPanel, true);
    }

    /// <summary>
    /// Shows a fail message from <see cref="Register"/> and calls <see cref="SetElementsInteractable(GameObject, bool)"/> 
    /// with <see cref="registerPanel"/> and true
    /// </summary>
    void RegisterFail(PlayFabError result)
    {
        print("Fail trying sign in:\n" + result);
        switch (result.HttpCode)
        {
            case 1009:
                alertRegisterText.text = "Username Not Available";
                userRegisterImage.enabled = true;
                break;
            case 1008:
                alertRegisterText.text = "Invalid Username";
                userRegisterImage.enabled = true;
                break;
            case 1007:
                alertRegisterText.text = "Invalid Password";
                passwordRegisterImage.enabled = true;
                break;
            case 1006:
                alertRegisterText.text = "Email Address Not Available";
                emailRegisterImage.enabled = true;
                break;
            case 1005:
                alertRegisterText.text = "Invalid Email Address";
                emailRegisterImage.enabled = true;
                break;
            case 1234:
                alertRegisterText.text = "Profane Display Name";
                userRegisterImage.enabled = true;
                break;
            default:
                if (result.ToString().Contains("Username must be between 3 and 20"))
                {
                    alertRegisterText.text = "Username size is 3 to 20";
                    userRegisterImage.enabled = true;
                }
                else if (result.ToString().Contains("Password must be between 6"))
                {
                    alertRegisterText.text = "Password size is 6 to 100";
                    passwordRegisterImage.enabled = true;
                }
                else if (result.ToString().Contains("Email address is not valid"))
                {
                    alertRegisterText.text = "Email address is not valid";
                    emailRegisterImage.enabled = true;
                }
                else if (result.ToString().Contains("Email address already exists"))
                {
                    alertRegisterText.text = "Email address already exists";
                    emailRegisterImage.enabled = true;
                }
                else if (result.ToString().Contains("User name already exists"))
                {
                    alertRegisterText.text = "User name already exists";
                    emailRegisterImage.enabled = true;
                }
                break;
        }
        SetElementsInteractable(registerPanel, true);
    }
    #endregion Register

    #region Password Recovery
    /// <summary>
    /// Send a playfab password recovery email to address in <see cref="passwordRecoveryText"/>
    /// </summary>
    public void PasswordRecovery()
    {
        SetElementsInteractable(passwordRecoveryPanel, false);
        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
        {
            Email = passwordRecoveryText.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, PasswordRecoverySuccess, PasswordRecoveryFail);
    }

    /// <summary>
    /// Shows conform message in <see cref="alertPasswordRecoveryText"/> and calls <see cref="SetElementsInteractable(GameObject, bool)"/> 
    /// with <see cref="passwordRecoveryPanel"/> and true
    /// </summary>
    void PasswordRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        alertPasswordRecoveryText.text = "E-mail sended.";
        SetElementsInteractable(passwordRecoveryPanel, true);
    }

    /// <summary>
    /// Shows a fail message from <see cref="PasswordRecovery"/> and calls <see cref="SetElementsInteractable(GameObject, bool)"/> 
    /// with <see cref="passwordRecoveryPanel"/> and true
    /// </summary>
    void PasswordRecoveryFail(PlayFabError result)
    {
        print("Fail trying sign in:\n" + result);
        switch (result.HttpCode)
        {
            case 1001:
                alertPasswordRecoveryText.text = "Account not founded";
                passwordRecoveryImage.enabled = true;
                break;
            default:
                if (result.ToString().Contains("Username must be between 3 and 20"))
                {
                    alertPasswordRecoveryText.text = "Username size is 3 to 20";
                    passwordRecoveryImage.enabled = true;
                }
                else if (result.ToString().Contains("Email address is not valid"))
                {
                    alertPasswordRecoveryText.text = "Email address is not valid";
                    passwordRecoveryImage.enabled = true;
                }
                break;
        }
        SetElementsInteractable(passwordRecoveryPanel, true);
    }
    #endregion Password Recovery

    /// <summary>
    /// Clean usernames and emails texts and turn off the respective alert images
    /// </summary>
    public void CleanUserImage()
    {
        alertSignInText.text = "";
        userSignInImage.enabled = false;
        alertRegisterText.text = "";
        userRegisterImage.enabled = false;
        emailRegisterImage.enabled = false;
        alertPasswordRecoveryText.text = "";
        passwordRecoveryImage.enabled = false;
    }

    /// <summary>
    /// Clean password texts and turn off the respective alert images
    /// </summary>
    public void CleanPasswordImage()
    {
        alertSignInText.text = "";
        passwordSignInImage.enabled = false;
        alertRegisterText.text = "";
        passwordRegisterImage.enabled = false;
        passwordRepeatRegisterImage.enabled = false;
    }

    /// <summary>
    /// Loads scene number 1
    /// </summary>
    public void LoadSpellbook()
    {
        homePanel.SetActive(false);
        usernameAndPasswordPanel.SetActive(false);
        EnableLoadingPanel(true);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Upper all string of the <see cref="Text.text"/>
    /// </summary>
    /// <param name="text">Text target</param>
    public void UpCaseText(Text text)
    {
        text.text = text.text.ToUpper();
    }
}