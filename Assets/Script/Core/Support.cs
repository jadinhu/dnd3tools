/**
* Support.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 23/02/19 (dd/mm/yy)
* Revised on: 19/12/21 (dd/mm/yy)
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Helper menu and calculator tool
/// </summary>
public class Support : MonoBehaviour
{
    /// <summary>
    /// Singleton of this class
    /// </summary>
    public static Support Instance { get; private set; }
    /// <summary>
    /// A keyboard simulator to be used in WebGL build
    /// </summary>
    [SerializeField]
    GameObject webKeyboardButton;
    /// <summary>
    /// List of Controllers of Attack Panels on <see cref="attackPanelList"/>
    /// </summary>
    [SerializeField]
    List<AttackPanel> attackPanelScriptList = new List<AttackPanel>(3);
    /// <summary>
    /// List of UI Attack Panels on bottom 
    /// </summary>
    [SerializeField]
    List<GameObject> attackPanelList = new List<GameObject>(3);
    /// <summary>
    /// The single canvas in this scene
    /// </summary>
    [SerializeField]
    GameObject canvasApp;
    /// <summary>
    /// Helper panel with app options
    /// </summary>
    [SerializeField]
    GameObject panelHelper;
    /// <summary>
    /// List of helper descriptions (used to next information: legal, panel attacks, spell schools)
    /// </summary>
    [SerializeField]
    List<GameObject> helperDescriptionList;
    /// <summary>
    /// Back the page of <see cref="helperDescriptionList"/>
    /// </summary>
    [SerializeField]
    Button backButtonHelper;
    /// <summary>
    /// Next the page of <see cref="helperDescriptionList"/>
    /// </summary>
    [SerializeField]
    Button nextButtonHelper;
    /// <summary>
    /// The panel with settings with (for now) sound enable/disable
    /// </summary>
    [SerializeField]
    GameObject panelSettings;
    /// <summary>
    /// Toggle to set sound enable or disable
    /// </summary>
    [SerializeField]
    Toggle toggleSettingsSound;
    /// <summary>
    /// Panel about user profile
    /// </summary>
    [SerializeField]
    GameObject panelProfile;
    /// <summary>
    /// Panel to select the current profile
    /// </summary>
    [SerializeField]
    GameObject panelSelectProfile;
    /// <summary>
    /// Panel to create a new profile
    /// </summary>
    [SerializeField]
    GameObject panelCreateProfile;
    /// <summary>
    /// Panel with cloud options (save, load)
    /// </summary>
    [SerializeField]
    GameObject panelCloud;
    /// <summary>
    /// Button to go to cloud options <see cref="panelCloud"/>
    /// </summary>
    [SerializeField]
    Button buttonCloud;
    /// <summary>
    /// Panel with msg "loading cloud"
    /// </summary>
    [SerializeField]
    GameObject panelLoadingCloud;
    /// <summary>
    /// List of buttons to select the current <see cref="attackPanelList"/>
    /// </summary>
    [SerializeField]
    List<Button> attackPanelButtonList = new List<Button>();
    /// <summary>
    /// The current Attack Panel displayed
    /// </summary>
    public AttackPanel AttackPanelSelected { get; private set; }
    /// <summary>
    /// The index of current <see cref="helperDescriptionList"/>
    /// </summary>
    int helperDescriptionListIndex;
    /// <summary>
    /// If the Panel of Attack Calculator are displayed
    /// </summary>
    bool attackPanelShowed = true;


    void Start()
    {
        Setup();
    }

    /// <summary>
    /// Sets the started values for support menu options
    /// </summary>
    void Setup()
    {
        if (Instance == null)
            Instance = this;
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            webKeyboardButton.SetActive(false);
        AttackPanelSelected = attackPanelScriptList[0];
        UpdateProfile();
        for (int i = 0; i < attackPanelScriptList.Count; i++)
            attackPanelScriptList[i].Initialize();
        ActiveButtonAttackPanel(attackPanelButtonList[0]);
    }

    /// <summary>
    /// Loads the Login scene and logout
    /// </summary>
    public void GoToLogin()
    {
        CheckLogin.SetAutoLogin(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Shows or hides the current <see cref="AttackPanelSelected"/> with Animation 
    /// </summary>
    public void ShowOrRetriveAttackPanel()
    {
        attackPanelShowed ^= true;
        if (attackPanelShowed)
            canvasApp.GetComponent<Animation>().Play("AttackPanelShow");
        else
            canvasApp.GetComponent<Animation>().Play("AttackPanelRetrive");
    }

    /// <summary>
    /// Shows or hides the panel of New Toggle
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowAddToggle(bool open)
    {
        AttackPanelSelected.ShowAddToggle(open);
    }

    /// <summary>
    /// Change the type bonus of new toggle (strenght or explicit integer)
    /// </summary>
    public void ChangeBonusToggle()
    {
        AttackPanelSelected.ChangeBonusToggle();
    }

    /// <summary>
    /// Add the new toggle setted
    /// </summary>
    public void AddToggle()
    {
        AttackPanelSelected.AddToggle();
    }

    /// <summary>
    /// Active the button for <see cref="attackPanelList"/> if there is a attack panel in this index
    /// </summary>
    /// <param name="btn">button clicked to enable the panel based on it index</param>
    public void ActiveButtonAttackPanel(Button btn)
    {
        for (int i = 0; i < attackPanelButtonList.Count; i++)
        {
            ColorBlock cb = attackPanelButtonList[i].colors;
            if (btn.name.Equals(attackPanelButtonList[i].name))
                cb.normalColor = Color.yellow;
            else
                cb.normalColor = Color.white;
            attackPanelButtonList[i].colors = cb;
            btn.enabled = false;
            btn.enabled = true;
        }
    }

    /// <summary>
    /// Change the displayed Attack Panel to another
    /// </summary>
    /// <param name="attackPanel">Panel to go</param>
    public void ChangeAttackPanel(AttackPanel attackPanel)
    {
        AttackPanelSelected = attackPanel;
        int attackPanelIndex = attackPanelScriptList.IndexOf(attackPanel);
        Debug.Log("attackPanelIndex " + attackPanelIndex);
        attackPanelList[attackPanelIndex].SetActive(true);
        for (int i = 0; i < attackPanelList.Count; i++)
        {
            if (attackPanelIndex != i)
            {
                attackPanelList[i].SetActive(false);
                attackPanelScriptList[i].ResetHolding();
            }
        }
    }

    /// <summary>
    /// Calls <see cref="AttackPanel.SelectImageToggle"/> from <see cref="AttackPanelSelected"/>
    /// to select a image for a new toggle from browser
    /// </summary>
    public void SelectImageToggle()
    {
        AttackPanelSelected.SelectImageToggle();
    }

    /// <summary>
    /// Shows or hides the Panel to delete a toggle. <seealso cref="AttackPanel.ShowDeleteToggleAttack(bool)"/>
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowDeleteToggleAttack(bool open)
    {
        AttackPanelSelected.ShowDeleteToggleAttack(open);
    }

    /// <summary>
    /// Confirms the deletion of holded toggle. <seealso cref="AttackPanel.ConfirmDeleteToggleAttack"/>
    /// </summary>
    public void ConfirmDeleteToggleAttack()
    {
        AttackPanelSelected.ConfirmDeleteToggleAttack();
    }

    /// <summary>
    /// Returns the <see cref="attackPanelList"/>
    /// </summary>
    public List<AttackPanel> GetAttackPanelList()
    {
        return attackPanelScriptList;
    }

    /// <summary>
    /// Update the profile settings about sound in <see cref="toggleSettingsSound"/>
    /// </summary>
    public void UpdateProfile()
    {
        toggleSettingsSound.isOn = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelSettings-sound") > 0;
    }

    /// <summary>
    /// Updates the profile list in <see cref="profileDropdown"/> from <see cref="Dao.GetProfileList"/>
    /// </summary>
    public void UpdateProfileList()
    {
        List<string> profileList = Dao.Instance.GetProfileList();
        Dropdown profileDropdown = panelSelectProfile.GetComponentInChildren<Dropdown>();
        profileDropdown.ClearOptions();
        if (profileList != null && profileList.Count > 0)
        {
            int currentValue = 0;
            for (int i = 0; i < profileList.Count; i++)
            {
                if (Dao.Instance.ProfileName.Equals(profileList[i]))
                    currentValue = i;
            }
            profileDropdown.interactable = true;
            profileDropdown.AddOptions(profileList);
            profileDropdown.value = currentValue;
        }
        else
            profileDropdown.interactable = false;
        profileDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Shows or hides the <see cref="panelHelper"/>
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowHelperPanel(bool open)
    {
        panelHelper.SetActive(open);
    }

    /// <summary>
    /// Change the helper page to next or back in <see cref="helperDescriptionList"/>
    /// </summary>
    /// <param name="next">next page if true, back page if false</param>
    public void NextHelperDescription(bool next)
    {
        if (next)
        {
            backButtonHelper.interactable = true;
            helperDescriptionList[helperDescriptionListIndex].SetActive(false);
            helperDescriptionListIndex++;
            helperDescriptionList[helperDescriptionListIndex].SetActive(true);
            if (helperDescriptionList.Count == helperDescriptionListIndex + 1)
                nextButtonHelper.interactable = false;
        }
        else
        {
            nextButtonHelper.interactable = true;
            helperDescriptionList[helperDescriptionListIndex].SetActive(false);
            helperDescriptionListIndex--;
            helperDescriptionList[helperDescriptionListIndex].SetActive(true);
            if (0 == helperDescriptionListIndex)
                backButtonHelper.interactable = false;
        }
    }

    /// <summary>
    /// Shows or hides the <see cref="panelSettings"/>
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowPanelSettings(bool open)
    {
        panelSettings.SetActive(open);
    }

    
    public void SaveSettings()
    {
        AudioHandler.Instance.SetAudioOn(toggleSettingsSound.isOn);
        PlayerPrefs.SetInt("PanelSettings-sound", toggleSettingsSound.isOn ? 1 : 0);
        ShowPanelSettings(false);
    }

    public void ShowPanelProfile(bool open)
    {
        panelProfile.SetActive(open);
        panelSelectProfile.SetActive(open);
        panelCreateProfile.SetActive(false);
    }

    public void ShowPanelSelectProfile(bool open)
    {
        panelSelectProfile.SetActive(open);
    }

    public void ShowPanelCreateProfile(bool open)
    {
        panelCreateProfile.SetActive(open);
    }

    public void SelectProfile()
    {
        Dropdown dropProfile = panelSelectProfile.GetComponentInChildren<Dropdown>();
        string profile = dropProfile.options[dropProfile.value].text;
        if (!string.IsNullOrEmpty(profile))
        {
            Dao.Instance.SetProfile(dropProfile.options[dropProfile.value].text);
            panelProfile.SetActive(false);
        }
    }

    public void CreateProfile()
    {
        string profileName = panelCreateProfile.GetComponentInChildren<InputField>().text;
        print("profileName " + profileName);
        if (!string.IsNullOrEmpty(profileName) && !profileName.Trim().Equals(""))
        {
            Dao.Instance.CreateProfile(profileName);
            UpdateProfileList();
            ShowPanelProfile(false);
        }
    }

    public void ShowPanelCloud(bool open)
    {
        panelCloud.SetActive(open);
    }

    public void ShowPanelLoadingCloud(bool open)
    {
        panelLoadingCloud.SetActive(open);
    }
}
