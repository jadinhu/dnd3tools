/**
* Dao.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 10/10/18 (dd/mm/yy)
* Revised on: 19/12/21 (dd/mm/yy)
*/
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the data to save and load (database, cloud and profiles)
/// </summary>
public class Dao : MonoBehaviour
{
    public static Dao Instance { get; private set; }
    /// <summary>
    /// Text title with profile name
    /// </summary>
    [SerializeField]
    Text textTitleProfileName;
    /// <summary>
    /// Panel message about player not logged in playfab
    /// </summary>
    [SerializeField]
    GameObject panelMustBeLogged;
    /// <summary>
    /// Panel of the cloud connection message
    /// </summary>
    [SerializeField]
    GameObject panelCloudMessage;
    /// <summary>
    /// Text of the cloud connection message
    /// </summary>
    [SerializeField]
    Text textCloudMessage;
    /// <summary>
    /// Text about profile option app
    /// </summary>
    [SerializeField]
    GameObject textNotifyProfile;
    /// <summary>
    /// Path of the dndtools database
    /// </summary>
    [SerializeField]
    string dndtoolsPath;
    /// <summary>
    /// If the app will import all database from dndtools database
    /// </summary>
    [SerializeField]
    bool onlyCreateDatabase;
    /// <summary>
    /// If the app will export own <see cref="PlayerPrefs"/> to file
    /// </summary>
    [SerializeField]
    bool exportPlayerPrefs;
    /// <summary>
    /// If the app will import a file for <see cref="PlayerPrefs"/> 
    /// </summary>
    [SerializeField]
    bool importPlayerPrefs;
    /// <summary>
    /// List of the all spells loaded
    /// </summary>
    public List<Spell> Spells { get; private set; }
    /// <summary>
    /// Profile's name used in this moment (default is empty)
    /// </summary>
    public string ProfileName { get; private set; }

    /// <summary>
    /// Runs when app is executed, before user can handle the app
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Default path of database
    /// </summary>
    public string DatabasePath
    {
        get
        {
            return "assets/resources/database/";
        }
    }

    /// <summary>
    /// Shows or hides the cloud panel message
    /// </summary>
    /// <param name="open">shows if true</param>
    public void OpenPanelCloudMessage(bool open)
    {
        panelCloudMessage.SetActive(open);
    }

    /// <summary>
    /// Shows or hides the panel message "must be logged"
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowPanelMustBeLogged(bool open)
    {
        panelMustBeLogged.SetActive(open);
    }

    /// <summary>
    /// Saves profile data in PlayFab cloud
    /// </summary>
    public void SaveInCloud()
    {
        if (!CheckLogin.IsLogged())
        {
            ShowPanelMustBeLogged(true);
            return;
        }
        Dictionary<string, string> data;
        if (string.IsNullOrEmpty(ProfileName))
        {
            data = new Dictionary<string, string>
            {
                { "spellsAllNames", PlayerPrefs.GetString("spellsAllNames") }
            };
        }
        else
        {
            string profileNames = PlayerPrefs.GetString("Profile List");
            List<string> list = new List<string>(profileNames.Split(','));
            data = new Dictionary<string, string>
            {
                { "Profile List", profileNames }
            };
            for (int i = 0; i < list.Count; i++)
            {
                data.Add(list[i] + "-spellsAllNames", PlayerPrefs.GetString(list[i] + "-spellsAllNames"));
            }
        }
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = data
        },
        result =>
        {
            Debug.Log("success save data in cloud");
            Support.Instance.ShowPanelLoadingCloud(false);
            textCloudMessage.text = "Success save data in cloud!";
            OpenPanelCloudMessage(true);
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
            Support.Instance.ShowPanelLoadingCloud(false);
            textCloudMessage.text = "Error trying save data in cloud. Sorry.";
            OpenPanelCloudMessage(true);
        });
    }

    /// <summary>
    /// Loads the profiles data from PlayFab cloud
    /// </summary>
    public void LoadFromCloud()
    {
        if (!CheckLogin.IsLogged())
        {
            ShowPanelMustBeLogged(true);
            return;
        }
        string profileNames = null;
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = FacebookAndPlayFabInfo.userPlayFabId,
            Keys = new List<string>
            {
                "Profile List"
            }
        }, result =>
        {
            if (result.Data == null)
            {
                Debug.Log("result.Data null");
                textCloudMessage.text = "Error trying load data from cloud. Sorry.";
                OpenPanelCloudMessage(true);
            }
            else
            {
                profileNames = result.Data["Profile List"].Value.ToString();
                Debug.Log("profileNames: " + profileNames);
                if (string.IsNullOrEmpty(profileNames))
                {
                    textNotifyProfile.SetActive(true);
                    textCloudMessage.text = "There is not profiles in cloud yet.";
                    OpenPanelCloudMessage(true);
                }
                else
                {
                    Debug.Log("haveList");
                    PlayerPrefs.SetString("Profile List", profileNames);
                    List<string> list = new List<string>(profileNames.Split(','));
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] += "-spellsAllNames";
                    }
                    PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                    {
                        PlayFabId = FacebookAndPlayFabInfo.userPlayFabId,
                        Keys = list
                    }, result2 =>
                    {
                        if (result2.Data == null)
                        {
                            Debug.Log("result.Data null");
                            textCloudMessage.text = "There is not profiles in cloud yet.";
                            OpenPanelCloudMessage(true);
                        }
                        else
                        {
                            Debug.Log("data: " + result2.Data.ToString());
                            textCloudMessage.text = "Your profile list are ready!";
                            OpenPanelCloudMessage(true);
                            textNotifyProfile.SetActive(false);
                            for (int i = 0; i < list.Count; i++)
                            {
                                Debug.Log("data: " + result2.Data[list[i]].ToString());
                                PlayerPrefs.SetString(list[i], result2.Data[list[i]].Value.ToString());
                            }
                            List<string> list2 = new List<string>(profileNames.Split(','));
                            if (list2.Contains(ProfileName))
                            {
                                SpellHandler.Instance.UpdateProfile();
                                textTitleProfileName.text = ProfileName;
                            }
                        }
                    }, (error) =>
                    {
                        Debug.Log(error.GenerateErrorReport());
                        textCloudMessage.text = "Error trying load data from cloud. Sorry.";
                        OpenPanelCloudMessage(true);
                    });
                    Support.Instance.UpdateProfileList();
                }
            }
            Support.Instance.ShowPanelLoadingCloud(false);
        }, (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
            Support.Instance.ShowPanelLoadingCloud(false);
            textCloudMessage.text = "Error trying load data from cloud. Sorry.";
            OpenPanelCloudMessage(true);
        });
    }

    /// <summary>
    /// Changes the profile to another and load it
    /// </summary>
    /// <param name="profile">name of selected profile</param>
    public void SetProfile(string profile)
    {
        ProfileName = profile;
        PlayerPrefs.SetString("CurrentProfile", ProfileName);
        LoadProfile();
    }

    /// <summary>
    /// Creates a new profile
    /// </summary>
    /// <param name="profileName">name of new profile</param>
    public void CreateProfile(string profileName)
    {
        if (string.IsNullOrEmpty(profileName))
            return;
        textNotifyProfile.SetActive(false);
        string profileNames = PlayerPrefs.GetString("Profile List");
        if (!profileNames.Equals(profileName, System.StringComparison.InvariantCultureIgnoreCase) &&
            !profileNames.StartsWith(profileName + ",") &&
            !profileNames.Contains("," + profileName + ",") &&
            !profileNames.EndsWith("," + profileName))
        {
            if (string.IsNullOrEmpty(profileNames))
                profileNames = profileName;
            else
                profileNames += "," + profileName;
            PlayerPrefs.SetString("Profile List", profileNames);
        }
        // transfer player prefs
        if (string.IsNullOrEmpty(ProfileName))
        {
            PlayerPrefs.SetString(profileName + "-spellsAllNames", PlayerPrefs.GetString("spellsAllNames"));
            PlayerPrefs.SetString(profileName + "-List of Toggle Attack-0", PlayerPrefs.GetString("List of Toggle Attack-0"));
            string toggleNames = PlayerPrefs.GetString("List of Toggle Attack-0");
            string toggleImages = PlayerPrefs.GetString("List of Toggle Attack-0");
            if (!string.IsNullOrEmpty(toggleNames))
            {
                string[] toggleNameList = toggleNames.Split(',');
                for (int i = 0; i < toggleNameList.Length; i++)
                {
                    PlayerPrefs.SetString(profileName + "-Toggle-0-" + toggleNameList[i], PlayerPrefs.GetString("Toggle-0-" + toggleNameList[i]));
                }
                AttackPanel attackPanelScript = Support.Instance.GetAttackPanelList()[0];
                foreach (Toggle toggle in attackPanelScript.GetAttackToggles())
                {
                    if (toggle)
                    {
                        toggle.GetComponentInChildren<RawImage>().enabled = toggle.isOn;
                        if (toggle.isOn)
                            PlayerPrefs.SetInt(profileName + "-PanelAttack-0-" + toggle.name, 1);
                        else
                            PlayerPrefs.SetInt(profileName + "-PanelAttack-0-" + toggle.name, 0);
                    }
                }
            }
            if (!string.IsNullOrEmpty(toggleImages))
            {
                string[] toggleImageList = toggleImages.Split(',');
                for (int i = 0; i < toggleImageList.Length; i++)
                {
                    PlayerPrefs.SetString(profileName + "-ToggleImage-0-" + toggleImageList[i], PlayerPrefs.GetString("ToggleImage-0-" + toggleImageList[i]));
                }
            }
            PlayerPrefs.SetInt(profileName + "-PanelAttackBBA-0", PlayerPrefs.GetInt("PanelAttackBBA-0"));
            PlayerPrefs.SetInt(profileName + "-PanelAttackStr-0", PlayerPrefs.GetInt("PanelAttackStr-0"));
            PlayerPrefs.SetInt(profileName + "-PanelAttackModAtk-0", PlayerPrefs.GetInt("PanelAttackModAtk-0"));
            PlayerPrefs.SetInt(profileName + "-PanelAttackModDmg-0", PlayerPrefs.GetInt("PanelAttackModDmg-0"));
            PlayerPrefs.SetInt(profileName + "-Panel0AttackTwohand-0", PlayerPrefs.GetInt("Panel0AttackTwohand-0"));
            PlayerPrefs.SetInt(profileName + "-SearchPanel-class", PlayerPrefs.GetInt("SearchPanel-class"));
            PlayerPrefs.SetInt(profileName + "-SearchPanel-level", PlayerPrefs.GetInt("SearchPanel-level"));
            PlayerPrefs.SetInt(profileName + "-SearchPanel-sourcebook", PlayerPrefs.GetInt("SearchPanel-sourcebook"));
        }
        print(profileNames);
        SetProfile(profileName);
    }

    /// <summary>
    /// Returns the profile list or null if have no profiles yet
    /// </summary>
    public List<string> GetProfileList()
    {
        string profileNames = PlayerPrefs.GetString("Profile List");
        List<string> list = new List<string>(profileNames.Split(','));
        return !string.IsNullOrEmpty(list[0]) ? list : null;
    }

    /// <summary>
    /// Sets a spell has favorite
    /// </summary>
    /// <param name="spell">the spell to be favorite</param>
    /// <param name="level">level of this spell</param>
    public void FavoriteSpell(Spell spell, string level)
    {
        string spellsAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            spellsAllNames = PlayerPrefs.GetString("spellsAllNames");
        else
            spellsAllNames = PlayerPrefs.GetString(ProfileName + "-spellsAllNames");
        string spellId = spell.id;
        if (!spellsAllNames.Equals(level + spellId, System.StringComparison.InvariantCultureIgnoreCase) &&
            !spellsAllNames.StartsWith(level + spellId + ",") &&
            !spellsAllNames.Contains("," + level + spellId + ",") &&
            !spellsAllNames.EndsWith("," + level + spellId))
        {
            if (string.IsNullOrEmpty(spellsAllNames))
                spellsAllNames = level + spellId;
            else
                spellsAllNames += "," + level + spellId;
            if (string.IsNullOrEmpty(ProfileName))
                PlayerPrefs.SetString("spellsAllNames", spellsAllNames);
            else
                PlayerPrefs.SetString(ProfileName + "-spellsAllNames", spellsAllNames);
        }
    }
    
    /// <summary>
    /// Loads the favorite spell list of a specified level
    /// </summary>
    /// <param name="level">the specified level</param>
    /// <returns>List of favorite spell of specified level</returns>
    public List<Spell> LoadFavoriteSpellsByLevel(string level)
    {
        List<Spell> spellList = new List<Spell>();
        string spellsAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            spellsAllNames = PlayerPrefs.GetString(ProfileName + "-spellsAllNames");
        else
            spellsAllNames = PlayerPrefs.GetString(ProfileName + "-spellsAllNames");
        string[] spellsNameList = spellsAllNames.Split(',');
        List<string> spellsNameByLevel = new List<string>();
        int size = spellsNameList.Length;
        for (int i = 0; i < size; i++)
        {
            if (spellsNameList[i].StartsWith(level))
                spellsNameByLevel.Add(spellsNameList[i].Substring(1));
        }
        if (spellsNameByLevel.Count > 0)
        {
            for (int i = 0; i < spellsNameByLevel.Count; i++)
            {
                foreach (Spell spell in Spells)
                {
                    if (spell.id.Equals(spellsNameByLevel[i]))
                    {
                        Spell s = spell;
                        s.level = level;
                        spellList.Add(spell);
                        break;
                    }
                }
            }
            spellList = spellList.OrderBy(si => si.name).ToList();
        }
        return spellList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="level"></param>
    public void RemoveFavoriteSpell(Spell spell, int level)
    {
        string spellsAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            spellsAllNames = PlayerPrefs.GetString(ProfileName + "-spellsAllNames");
        else
            spellsAllNames = PlayerPrefs.GetString(ProfileName + "-spellsAllNames");
        string spellId = spell.id;
        int indexStart = -1;
        if (spellsAllNames.Equals(level + spellId, System.StringComparison.InvariantCultureIgnoreCase) ||
            spellsAllNames.StartsWith(level + spellId + ","))
            indexStart = 0;
        else if (spellsAllNames.Contains("," + level + spellId + ","))
            indexStart = spellsAllNames.IndexOf("," + level + spellId + ",") + 1;
        else if (spellsAllNames.EndsWith("," + level + spellId))
            indexStart = spellsAllNames.Length - ("," + level + spellId).Length;
        if (spellsAllNames.Contains(level + spellId))
        {
            print(level + spellId + " in " + indexStart);
            indexStart = spellsAllNames.IndexOf(level + spellId, indexStart);
            int indexEnd = indexStart + (level + spellId).Length;
            string newSpellAllNames = spellsAllNames.Substring(0, indexStart);
            newSpellAllNames += spellsAllNames.Substring(indexEnd);
            // fix possibly problems
            newSpellAllNames = newSpellAllNames.Replace(",,", ",");
            print(newSpellAllNames);
            if (newSpellAllNames.EndsWith(","))
                newSpellAllNames = newSpellAllNames.Substring(0, newSpellAllNames.Length - 1);
            if (newSpellAllNames.StartsWith(","))
                newSpellAllNames = newSpellAllNames.Substring(1);
            // save the string list
            if (string.IsNullOrEmpty(ProfileName))
                PlayerPrefs.SetString("spellsAllNames", newSpellAllNames);
            else
                PlayerPrefs.SetString(ProfileName + "-spellsAllNames", newSpellAllNames);
            print(newSpellAllNames);
        }
    }

    public void SaveOrUpdateToggleAttack(Toggle toggle, Texture2D textureNewToggle)
    {
        AttackPanel attackPanel = Support.Instance.AttackPanelSelected;
        int attackPanelIndex = Support.Instance.GetAttackPanelList().IndexOf(attackPanel);
        string toggleAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            toggleAllNames = PlayerPrefs.GetString("List of Toggle Attack-" + attackPanelIndex);
        else
            toggleAllNames = PlayerPrefs.GetString(ProfileName + "-List of Toggle Attack-" + attackPanelIndex);
        string toggleName = toggle.name.Replace("<b>", "").Replace("</b>", "").Replace(",", "");
        // save label (name can be listed starting with "ToggleImage")
        if (string.IsNullOrEmpty(ProfileName))
            PlayerPrefs.SetString("Toggle-" + attackPanelIndex + "-" + toggleName, toggle.GetComponentInChildren<Text>().text);
        else
            PlayerPrefs.SetString(ProfileName + "-Toggle-" + attackPanelIndex + "-" + toggleName, toggle.GetComponentInChildren<Text>().text);
        // if not exist
        if (string.IsNullOrEmpty(toggleAllNames) || (!toggleAllNames.StartsWith(toggleName) &&
            !toggleAllNames.EndsWith(toggleName) && !toggleAllNames.Contains("," + toggleName + ",")))
        {
            if (string.IsNullOrEmpty(toggleAllNames))
                toggleAllNames = toggleName;
            else
                toggleAllNames += "," + toggleName;
            if (string.IsNullOrEmpty(ProfileName))
                PlayerPrefs.SetString("List of Toggle Attack-" + attackPanelIndex, toggleAllNames);
            else
                PlayerPrefs.SetString(ProfileName + "-List of Toggle Attack-" + attackPanelIndex, toggleAllNames);
        }
        if (string.IsNullOrEmpty(ProfileName))
            WriteTextureToPlayerPrefs("ToggleImage-" + attackPanelIndex + "-" + toggleName, textureNewToggle);
        else
            WriteTextureToPlayerPrefs(ProfileName + "-ToggleImage-" + attackPanelIndex + "-" + toggleName, textureNewToggle);
    }

    public List<Toggle> LoadToggleAttack(Toggle toggleBase, AttackPanel attackPanel)
    {
        List<Toggle> toggleList = new List<Toggle>();
        int attackPanelIndex = Support.Instance.GetAttackPanelList().IndexOf(attackPanel);
        string toggleAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            toggleAllNames = PlayerPrefs.GetString("List of Toggle Attack-" + attackPanelIndex);
        else
            toggleAllNames = PlayerPrefs.GetString(ProfileName + "-List of Toggle Attack-" + attackPanelIndex);
        if (string.IsNullOrEmpty(toggleAllNames))
            return toggleList;
        string[] toggleNameList = toggleAllNames.Split(',');
        for (int i = 0; i < toggleNameList.Length; i++)
        {
            GameObject toggleObj = Instantiate(toggleBase.gameObject);
            toggleObj.SetActive(true);
            toggleObj.name = toggleNameList[i];
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            Texture2D tex;
            if (string.IsNullOrEmpty(ProfileName))
                tex = ReadTextureFromPlayerPrefs("ToggleImage-" + attackPanelIndex + "-" + toggleNameList[i]);
            else
                tex = ReadTextureFromPlayerPrefs(ProfileName + "-ToggleImage-" + attackPanelIndex + "-" + toggleNameList[i]);
            ((Image)toggle.targetGraphic).sprite = ((Image)toggle.graphic).sprite =
                Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            string text;
            if (string.IsNullOrEmpty(ProfileName))
                text = PlayerPrefs.GetString("Toggle-" + attackPanelIndex + "-" + toggleNameList[i]);
            else
                text = PlayerPrefs.GetString(ProfileName + "-Toggle-" + attackPanelIndex + "-" + toggleNameList[i]);
            toggleObj.GetComponentInChildren<Text>().text = text;
            toggleObj.transform.SetParent(toggleBase.transform.parent);
            toggleObj.transform.localScale = toggleBase.transform.localScale;
            toggleObj.transform.localPosition = toggleBase.transform.localPosition;
            toggleList.Add(toggle);
        }
        toggleList = toggleList.OrderBy(si => si.name).ToList();
        return toggleList;
    }

    public void RemoveToggleAttack(Toggle toggle)
    {
        AttackPanel attackPanel = Support.Instance.AttackPanelSelected;
        int attackPanelIndex = Support.Instance.GetAttackPanelList().IndexOf(attackPanel);
        string toggleAllNames;
        if (string.IsNullOrEmpty(ProfileName))
            toggleAllNames = PlayerPrefs.GetString("List of Toggle Attack-" + attackPanelIndex);
        else
            toggleAllNames = PlayerPrefs.GetString(ProfileName + "-List of Toggle Attack-" + attackPanelIndex);
        string toggleName = toggle.name.Replace("<b>", "").Replace("</b>", "").Replace(",", "");
        if (toggleAllNames.Contains(toggleName))
        {
            print(toggleName);
            int indexStart = toggleAllNames.IndexOf(toggleName);
            int indexEnd = indexStart + (toggleName).Length;
            string newToggleAllNames = toggleAllNames.Substring(0, indexStart);
            newToggleAllNames += toggleAllNames.Substring(indexEnd);
            // fix possibly problems
            newToggleAllNames = newToggleAllNames.Replace(",,", ",");
            print(newToggleAllNames);
            if (newToggleAllNames.EndsWith(","))
                newToggleAllNames = newToggleAllNames.Substring(0, newToggleAllNames.Length - 1);
            if (newToggleAllNames.StartsWith(","))
                newToggleAllNames = newToggleAllNames.Substring(1, newToggleAllNames.Length - 1);
            // save the string list and remove the spell description key
            if (string.IsNullOrEmpty(ProfileName))
            {
                PlayerPrefs.SetString("List of Toggle Attack-" + attackPanelIndex, newToggleAllNames);
                PlayerPrefs.DeleteKey("Toggle-" + attackPanelIndex + "-" + toggleName);
                PlayerPrefs.DeleteKey("ToggleImage-" + attackPanelIndex + "-" + toggleName);
                PlayerPrefs.DeleteKey("PanelAttack-" + attackPanelIndex + "-" + toggle.name);
            }
            else
            {
                PlayerPrefs.SetString(ProfileName + "-List of Toggle Attack-" + attackPanelIndex, newToggleAllNames);
                PlayerPrefs.DeleteKey(ProfileName + "-Toggle-" + attackPanelIndex + "-" + toggleName);
                PlayerPrefs.DeleteKey(ProfileName + "-ToggleImage-" + attackPanelIndex + "-" + toggleName);
                PlayerPrefs.DeleteKey(ProfileName + "-PanelAttack-" + attackPanelIndex + "-" + toggle.name);
            }
            print(newToggleAllNames);
        }
    }

    public void WriteTextureToPlayerPrefs(string tag, Texture2D tex)
    {
        print(tag);
        // if texture is png otherwise you can use tex.EncodeToJPG().
        byte[] texByte = tex.EncodeToPNG();
        // convert byte array to base64 string
        string base64Tex = System.Convert.ToBase64String(texByte);
        // write string to playerpref
        PlayerPrefs.SetString(tag, base64Tex);
        PlayerPrefs.Save();
    }

    public Texture2D ReadTextureFromPlayerPrefs(string tag)
    {
        // load string from playerpref
        string base64Tex = PlayerPrefs.GetString(tag, null);

        if (!string.IsNullOrEmpty(base64Tex))
        {
            // convert it to byte array
            byte[] texByte = System.Convert.FromBase64String(base64Tex);
            Texture2D tex = new Texture2D(128, 128, TextureFormat.ARGB32, false);

            //load texture from byte array
            if (tex.LoadImage(texByte))
            {
                return tex;
            }
        }
        return null;
    }

    /// <summary>
    /// Fix text removing and replacing characters like div, br, multiple spaces etc.
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    public string FixText(string text)
    {
        text = RemoveLiOlUl(text);
        text = ReplaceCodeToSinal(text);
        text = RemoveDivClassNiceTexttile(text);
        text = ReplaceEmToBold(text);
        text = ReplaceH2ToBold(text);
        text = RemoveAbbrToBold(text);
        text = ReplaceStrongToBold(text);
        text = ReplaceSpanToBold(text);
        text = ReplaceHrefToLink(text);
        text = RemoveText(text, "<link=", ">");
        text = text.Replace("</link>", "");
        text = RemoveOthersDivs(text);
        text = BuildTable(text);
        text = ReplaceBrToLineBroken(text);
        text = ReplaceParagraphToLineBroken(text);
        text = RemoveGenerics(text);
        return text;
    }

    public void GetProfileInCloud()
    {

    }

    void Initialize()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("YOU HAVE MORE THAN ONE DAO INSTANCE (singleton)");
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ProfileName = PlayerPrefs.GetString("CurrentProfile");
        textTitleProfileName.text = ProfileName;
        if (string.IsNullOrEmpty(ProfileName))
            textNotifyProfile.SetActive(true);
        else
            GetProfileInCloud();
        Spells = new List<Spell>();
        if (onlyCreateDatabase)
            CreateDatabase();
        else
            LoadDatabase();
    }

    void LoadProfile()
    {
        textTitleProfileName.text = ProfileName;
        for (int i = 0; i < Support.Instance.GetAttackPanelList().Count; i++)
            Support.Instance.GetAttackPanelList()[i].UpdateProfile();
        GetComponent<SpellHandler>().UpdateProfile();
    }

    /// <summary>
    /// Search spell in database by spell id
    /// </summary>
    /// <param name="spellName">spell id</param>
    /// <returns>Spell finded or spell empty</returns>
    Spell SearchSpellId(string spellId)
    {
        print(spellId);
        if (!string.IsNullOrEmpty(spellId))
        {
            int size = Spells.Count;
            for (int i = 0; i < size; i++)
            {
                if (Spells[i].id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                    return Spells[i];
            }
        }
        return new Spell();
    }

    /// <summary>
    /// Search spells in database by spell name
    /// </summary>
    /// <param name="spellName">spells names</param>
    /// <returns>Spells finded or empty list</returns>
    List<Spell> SearchSpell(string spellName)
    {
        print(spellName);
        List<Spell> list = new List<Spell>();
        if (!string.IsNullOrEmpty(spellName))
        {
            int size = Spells.Count;
            for (int i = 0; i < size; i++)
            {
                if (Spells[i].name.IndexOf(spellName, System.StringComparison.InvariantCultureIgnoreCase) > -1)
                    list.Add(Spells[i]);
            }
        }
        return list;
    }

    /// <summary>
    /// Search spell list by class level
    /// </summary>
    /// <param name="spellClass">name of the caster class (if "all" ignore the class condiction)</param>
    /// <param name="spellLevel">level of the spell (if "all" ignore the level condiction)</param>
    /// <returns>list of spells contain the caster class name and/or spell level</returns>
    List<Spell> SearchSpell(string spellClass, string spellLevel)
    {
        print(spellClass + " / " + spellLevel);
        List<Spell> list = new List<Spell>();
        int size = Spells.Count;
        for (int i = 0; i < size; i++)
        {
            foreach (string classLevel in Spells[i].classLevel)
            {
                // if class: all or name is equals
                if (string.IsNullOrEmpty(spellClass) || classLevel.IndexOf(spellClass, System.StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    // if level: all or number is equals
                    if (string.IsNullOrEmpty(spellLevel) || classLevel.Contains(spellLevel))
                        list.Add(Spells[i]);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Search spell list by spell name and class level
    /// </summary>
    /// <param name="spellName">spell name</param>
    /// <param name="spellClass">name of the caster class (if "all" ignore the class condiction)</param>
    /// <param name="spellLevel">level of the spell (if "all" ignore the level condiction)</param>
    /// <returns>list of spells contain the spell name and the caster class name and/or spell level</returns>
    public List<Spell> SearchSpell(string spellName, string spellClass, string spellLevel,
        string spellSourceBooks, int save, int spellResistence)
    {
        Debug.Log("save " + save);
        Debug.Log("spellResistence " + spellResistence);
        spellName = spellName.TrimStart(' ').TrimEnd(' ');
        List<Spell> list = new List<Spell>();
        int size = Spells.Count;
        System.StringComparison stringComparison = System.StringComparison.CurrentCultureIgnoreCase;
        for (int i = 0; i < size; i++)
        {
            foreach (string classLevel in Spells[i].classLevel)
            {
                // if class: all or name is equals
                if (string.IsNullOrEmpty(spellClass) || classLevel.IndexOf(spellClass, stringComparison) > -1)
                {
                    // if level: all or number is equals
                    if (string.IsNullOrEmpty(spellLevel) || classLevel.Contains(spellLevel))
                    {
                        if (string.IsNullOrEmpty(spellSourceBooks) ||
                            spellSourceBooks.IndexOf(Spells[i].sourcebook, stringComparison) > -1)
                        {
                            if (save == 0 || (save == 1 && Spells[i].save) || (save == 2 && !Spells[i].save))
                            {
                                if (spellResistence == 0 || (spellResistence == 1 && Spells[i].spellResistence) || 
                                    (spellResistence == 2 && !Spells[i].spellResistence))
                                {
                                    if (string.IsNullOrEmpty(spellName) ||
                                    Spells[i].name.IndexOf(spellName, stringComparison) > -1)
                                        if (!list.Contains(Spells[i]))
                                            list.Add(Spells[i]);
                                }
                            }
                        }
                    }
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Load all database (spell list) from resources folder
    /// </summary>
    void LoadDatabase()
    {
        // get all spells
        Object[] objList = Resources.LoadAll("Database", typeof(TextAsset));
        int listSize = objList.Length;
        for (int i = 0; i < listSize; i++)
        {
            // get file text
            TextAsset textAsset = (TextAsset)objList[i];
            string fileText = textAsset.text;
            if (string.IsNullOrEmpty(fileText))
                continue;
            // create a spell with file text
            Spell spell = LoadSpell(textAsset.name, fileText);
            Spells.Add(spell);
        }
    }

    /// <summary>
    /// Create a <see cref="Spell"/> with a spell.txt
    /// </summary>
    /// <param name="spellText">string of the spell.txt</param>
    /// <returns>a new spell</returns>
    Spell LoadSpell(string fileName, string spellText)
    {
        string fixedSpellText = FixText(spellText);
        string[] lines = fixedSpellText.Split("\n"[0]);
        string spellName = lines[0].Substring(0, lines[0].IndexOf("("));
        spellName = spellName.Replace("<b>", "").Replace("</b>", "");
        string sourcebook = lines[0].Substring(lines[0].IndexOf("(") + 1, lines[0].IndexOf(")") - lines[0].IndexOf("(") - 1);
        sourcebook = sourcebook.Replace(":", "");
        if (sourcebook.Contains(","))
            sourcebook = sourcebook.Substring(0, sourcebook.IndexOf(",", 0));
        List<string> classLevelList = FindSpellLevel(fixedSpellText);
        return new Spell(fileName, spellName, sourcebook, fixedSpellText, lines[2], CheckSpellSavingThrow(fixedSpellText),
            CheckSpellResistence(fixedSpellText), classLevelList);
    }

    /// <summary>
    /// Find spell name with site text
    /// </summary>
    /// <param name="spellText">site text</param>
    /// <returns>spell name</returns>
    string FindSpellName(string spellText)
    {
        int indexStart = spellText.IndexOf("<b>") + 3;
        int indexEnd = spellText.IndexOf("</b>");
        return FixText(spellText.Substring(indexStart, indexEnd - indexStart));
    }

    /// <summary>
    /// Find spell class+level list with site text
    /// </summary>
    /// <param name="spellText">site text</param>
    /// <returns>Class+level list of the spell (ex.: Wizard 4)</returns>
    List<string> FindSpellLevel(string spellText)
    {
        List<string> list = new List<string>();
        // get the started index about class spell list
        int indexTitle = spellText.IndexOf("<b>Level:</b>");
        int indexStart = indexTitle + 13;
        // get the end index about class spell list
        int indexEnd = spellText.IndexOf("<b>Components:</b>");
        // cut the piece of text contains only class spell list
        string classLevelPiece = spellText.Substring(indexStart, indexEnd - indexStart);
        list.AddRange(classLevelPiece.Split(','));
        // remove de first blank space
        for (int i = 0; i < list.Count; i++)
        {
            if (string.IsNullOrEmpty(list[i].Trim()))
            {
                list.RemoveAt(i);
                continue;
            }
            list[i] = list[i].TrimStart(' ');
        }
        return list;
    }

    /// <summary>
    /// Check if the spell have a saving throw
    /// </summary>
    /// <param name="spellText">site text</param>
    /// <returns>true if spell have saving throw, false if none</returns>
    bool CheckSpellSavingThrow(string spellText)
    {
        int indexStart = spellText.IndexOf("<b>Saving Throw:</b> None");
        if (indexStart < 0)
            indexStart = spellText.IndexOf("<b>Saving Throw:</b>None");
        else if (indexStart < 0)
            indexStart = spellText.IndexOf("<b>Saving Throw:</b>  None");
        return indexStart < 0;
    }

    /// <summary>
    /// Check if the spell have SR yes
    /// </summary>
    /// <param name="spellText">site text</param>
    /// <returns>true if spell have SR, false if no</returns>
    bool CheckSpellResistence(string spellText)
    {
        int indexStart = spellText.IndexOf("Spell Resistance:</b> Yes");
        if (indexStart < 0)
            indexStart = spellText.IndexOf("Spell Resistance:</b>Yes");
        else if (indexStart < 0)
            indexStart = spellText.IndexOf("Spell Resistance:</b>  Yes");
        return indexStart > -1;
    }

    /// <summary>
    /// Create spell's database using dndtools folder
    /// </summary>
    void CreateDatabase()
    {
        int counter = 0;
        List<string> spellsText = new List<string>();
        // get books folders from dndtools
        string[] bookFolders = Directory.GetDirectories(dndtoolsPath);
        int bookFoldersSize = bookFolders.Length;
        for (int i = 0; i < bookFoldersSize; i++)
        {
            if (bookFolders[i].Contains("descriptors") || bookFolders[i].Contains("domains") ||
                bookFolders[i].Contains("schools") || bookFolders[i].Contains("sub-schools"))
                continue;
            string bookPath = bookFolders[i].Replace(dndtoolsPath, DatabasePath);
            // create database folders for books
            Directory.CreateDirectory(bookPath);
            // get spells folders from dndtools
            string[] spellFolders = Directory.GetDirectories(bookFolders[i] + "/");
            int spellFoldersSize = spellFolders.Length;
            // search spells in all spells folders of the dndtools
            for (int j = 0; j < spellFoldersSize; j++)
            {
                counter++;
                if (counter < 4560)
                    continue;
                // read spell file
                string spellText = File.ReadAllText(spellFolders[j] + "/index.html");
                spellsText.Add(spellText);
                CreateSpellFile(spellText, bookPath);
                print(counter);
            }
        }
    }

    /// <summary>
    /// Create a spell.txt file
    /// </summary>
    /// <param name="spellText">the text of the file</param>
    /// <param name="bookPath">path to file be saved</param>
    void CreateSpellFile(string spellText, string bookPath)
    {
        Spell spell = GetDataFromHtml(spellText);
        File.WriteAllText(bookPath + "/" + spell.name, spell.description);
    }

    /// <summary>
    /// Create a spell extracting the spell text from dndtools file
    /// </summary>
    /// <param name="siteText">text of the file from dndtools</param>
    /// <returns>a new spell</returns>
    Spell GetDataFromHtml(string siteText)
    {
        string spellName = FindSpellNameFromDndtools(siteText);
        string description = FindSpellDescriptionFromDndtools(siteText);
        string sourcebook = FindSpellSourceBookFromDndtools(siteText);
        List<string> classLevelList = FindSpellLevelFromDndtools(siteText);
        return new Spell(spellName, sourcebook, description, "", description.Split("\n"[0])[2],
            CheckSpellSavingThrow(description), CheckSpellResistence(description), classLevelList);
    }

    /// <summary>
    /// Find spell name with site text for DndTools external base
    /// </summary>
    /// <param name="siteText">site text</param>
    /// <returns>spell name</returns>
    string FindSpellNameFromDndtools(string siteText)
    {
        int indexStart = siteText.IndexOf("inaccurate");
        indexStart = siteText.IndexOf("<h2", indexStart + 1);
        int indexEnd = siteText.IndexOf("</h2", indexStart);
        string name = siteText.Substring(indexStart + 4, indexEnd - indexStart - 4);
        return ReplaceCodeToSinal(name).Replace(",", "");
    }

    /// <summary>
    /// Find spell full text with site text for DndTools external base
    /// </summary>
    /// <param name="siteText">site text</param>
    /// <returns>Full spell description (name, level, details etc.)</returns>
    string FindSpellDescriptionFromDndtools(string siteText)
    {
        int indexStart = siteText.IndexOf("inaccurate");
        indexStart = siteText.IndexOf("<h2", indexStart + 1);
        string text = siteText.Substring(indexStart, siteText.IndexOf("<div id=\"content_footer\">") - indexStart);
        return FixTextFromDndtools(text);
    }

    /// <summary>
    /// Find spell source book with site text for DndTools external base
    /// </summary>
    /// <param name="siteText">site text</param>
    /// <returns>Spell source book</returns>
    string FindSpellSourceBookFromDndtools(string siteText)
    {
        int indexStart = siteText.IndexOf("inaccurate");
        indexStart = siteText.IndexOf("<h2", indexStart + 1);
        indexStart = siteText.IndexOf("\">", indexStart);
        int indexEnd = siteText.IndexOf("</a>)", indexStart);
        string name = siteText.Substring(indexStart + 2, indexEnd - indexStart - 2);
        return ReplaceCodeToSinal(name).Replace(",", "");
    }

    /// <summary>
    /// Find spell class+level list with site text for DndTools external base
    /// </summary>
    /// <param name="siteText">site text</param>
    /// <returns>Class+level list of the spell (ex.: Wizard 4)</returns>
    List<string> FindSpellLevelFromDndtools(string siteText)
    {
        List<string> list = new List<string>();
        int indexTitle = siteText.IndexOf("<strong>Level:");
        int indexStart = siteText.IndexOf("<a href", indexTitle);
        int indexEnd = siteText.IndexOf("<br/>", indexStart);
        string classLevelPiece = siteText.Substring(indexStart, indexEnd - indexStart);
        list.AddRange(classLevelPiece.Split(','));
        for (int i = 0; i < list.Count; i++)
        {
            int indexNext = list[i].IndexOf("\">") + 2;
            string classLevel = CutUntilNextNumber(list[i].Substring(indexNext));
            if (!string.IsNullOrEmpty(classLevel))
            {
                list[i] = FixTextFromDndtools(classLevel);
                //print(list[i]);
            }
        }
        return list;
    }

    /// <summary>
    /// Substring starting in 0 until find some number (ex.: send "alex7wx" return "alex7")
    /// </summary>
    /// <param name="original">original string</param>
    /// <returns>cutted string or null if can't find a number</returns>
    string CutUntilNextNumber(string original)
    {
        int size = original.Length;
        for (int i = 0; i < size; i++)
        {
            char c = original[i];
            if (c >= '0' && c <= '9')
                return original.Substring(0, i + 1);
        }
        return null;
    }

    /// <summary>
    /// Fix text removing and replacing characters like div, br, multiple spaces etc. of the DndTools database
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string FixTextFromDndtools(string text)
    {
        text = ReplaceParagraphToLineBroken(text);
        text = RemoveGenerics(text);
        text = RemoveLiOlUl(text);
        text = ReplaceCodeToSinal(text);
        text = RemoveDivClassNiceTexttile(text);
        text = ReplaceEmToBold(text);
        text = ReplaceH2ToBold(text);
        text = RemoveAbbrToBold(text);
        text = ReplaceStrongToBold(text);
        text = ReplaceHrefToLink(text);
        text = RemoveText(text, "<link=", ">");
        text = text.Replace("</link>", "");
        text = RemoveOthersDivs(text);
        text = RemoveMultipleSpaces(text);
        text = RemoveLineBroken(text);
        text = ReplaceBrToLineBroken(text);
        text = ReplaceParagraphToLineBroken(text);
        text = BuildTable(text);
        return text;
    }

    /// <summary>
    /// Fix spell description building a top table information: title, class, level, school etc.
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string BuildTable(string text)
    {
        int startTableIndex = text.IndexOf("<table>") + 7;
        if (startTableIndex > 6)
        {
            string tableSearched;
            int endTableIndex = text.IndexOf("</table>");
            if (endTableIndex == -1)
                endTableIndex = text.LastIndexOf("</tr>");
            tableSearched = text.Substring(startTableIndex, endTableIndex - startTableIndex);
            List<string> titles = new List<string>();
            List<string> lines = new List<string>();
            int nextTh = tableSearched.IndexOf("<th>");
            while (nextTh > -1)
            {
                int startTitle = nextTh + 4;
                string title = tableSearched.Substring(startTitle, tableSearched.IndexOf("</th>", nextTh) - startTitle);
                titles.Add(title);
                nextTh = tableSearched.IndexOf("<th>", nextTh + 1);
            }
            int nextTr = tableSearched.IndexOf("<tr>");
            int nextTd = tableSearched.IndexOf("<td>");
            while (nextTr > -1)
            {
                string line = "";
                do
                {
                    int startContent = nextTd + 4;
                    line += " | " + tableSearched.Substring(startContent, tableSearched.IndexOf("</td>", nextTd) - startContent);
                    nextTd = tableSearched.IndexOf("<td>", nextTd + 1);
                } while (nextTd > -1 && nextTd < nextTr);
                lines.Add(line);
                nextTr = tableSearched.IndexOf("<tr>", nextTr + 1);
            }
            string finalTable;
            if (titles.Count > 0)
            {
                finalTable = titles[0];
                for (int i = 1; i < titles.Count; i++)
                {
                    finalTable += " | " + titles[i];
                }
            }
            else
            {
                finalTable = "";
            }
            for (int i = 0; i < lines.Count; i++)
            {
                finalTable += "\n" + lines[i].Substring(3);
            }
            text = text.Substring(0, startTableIndex) + finalTable + text.Substring(endTableIndex);
            if (finalTable.IndexOf("<table>") > -1)
                text = BuildTable(text);
        }
        return text.Replace("<sup>", "").Replace("</sup>", "")
            .Replace("<table>", "").Replace("</table>", "");
    }

    /// <summary>
    /// Removes line breaks
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveLineBroken(string text)
    {
        return text.Replace("\n", "");
    }

    /// <summary>
    /// Removes some special characters, like '*' and 'Ã—'
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveGenerics(string text)
    {
        return text.Replace("×", "x").Replace("(BE)", "").Replace("		", "- ").Replace("ï¿½ ;", "-")
            .Replace("*", "").Replace("Ã—", "x").Replace("'", "");
    }

    /// <summary>
    /// Removes some undesired html tags
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveLiOlUl(string text)
    {
        return text.Replace("<li>", "").Replace("</li>", "").Replace("<ol>", "").Replace("</ol>", "")
            .Replace("<ul>", "").Replace("</ul>", "");
    }

    /// <summary>
    /// Removes div html tags
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveOthersDivs(string text)
    {
        return text.Replace("<div>", "").Replace("</div>", "");
    }

    /// <summary>
    /// Removes div html tag about class 
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveDivClassNiceTexttile(string text)
    {
        return text.Replace("<div class=\"nice-textile\">", "");
    }

    /// <summary>
    /// Removes multiple spaces
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveMultipleSpaces(string text)
    {
        RegexOptions options = RegexOptions.None;
        Regex regex = new Regex("[ ]{2,}", options);
        return regex.Replace(text, " ");
    }

    /// <summary>
    /// Removes a text between two other texts
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <param name="startedText">begin of text reference to remove</param>
    /// <param name="endText">the end of the text to remove</param>
    /// <returns>fixed text</returns>
    string RemoveText(string text, string startedText, string endText)
    {
        int indexStart = text.IndexOf(startedText);
        while (indexStart > -1)
        {
            int indexEnd = text.IndexOf(endText, indexStart);
            text = text.Substring(0, indexStart) + text.Substring(indexEnd + endText.Length);
            indexStart = text.IndexOf(startedText);
        }
        return text;
    }

    /// <summary>
    /// Removes abbr html tag
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string RemoveAbbrToBold(string text)
    {
        return text.Replace("</abbr>", "").Replace("<abbr title=\"Material\">", "")
            .Replace("<abbr title=\"Somatic\">", "").Replace("<abbr title=\"Verbal\">", "")
            .Replace("<abbr title=\"Divine Focus\">", "").Replace("<abbr title=\"Experience\">", "")
            .Replace("<abbr title=\"Experience\">", "").Replace("<abbr title=\"Meta Breath\">", "")
            .Replace("<abbr title=\"Arcane Focus\">", "");
    }

    /// <summary>
    /// Replaces some undesired texts with another text, like: "&#8212" to " — "
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceCodeToSinal(string text)
    {
        return text.Replace("&#39;", "'").Replace("Ã»", "u").Replace("Â· ", "-").Replace("&#215", "x")
            .Replace("&#215;", "x").Replace("&#quot;", "\"").Replace("&#8212", " — ").Replace("&quot;", "\"")
            .Replace("&#8211;", "–");
    }

    /// <summary>
    /// Replaces br html tag to line break
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceBrToLineBroken(string text)
    {
        return text.Replace("<br>", "\n").Replace("</br>", "").Replace("<br/>", "\n").Replace("<br />", "\n")
            .Replace("<br/ >", "\n");
    }

    /// <summary>
    /// Replaces p html tag to line break
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceParagraphToLineBroken(string text)
    {
        return text.Replace("<p>", "\n").Replace("</p>", "\n");
    }

    /// <summary>
    /// Replaces bold html tag to unity text bold
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceEmToBold(string text)
    {
        return text.Replace("<em>", "<b>").Replace("</em>", "</b>");
    }

    /// <summary>
    /// Replaces h html tag to unity text bold
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceH2ToBold(string text)
    {
        text = text.Replace("<h4>", "<b>").Replace("</h4>", "</b>");
        text = text.Replace("<h3>", "<b>").Replace("</h3>", "</b>");
        text = text.Replace("<h2>", "<b>").Replace("</h2>", "</b>");
        return text.Replace("<h1>", "<b>").Replace("</h1>", "</b>");
    }

    /// <summary>
    /// Replaces strong html tag to unity text bold
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceStrongToBold(string text)
    {
        return text.Replace("<strong>", "<b>").Replace("</strong>", "</b>");
    }

    /// <summary>
    /// Replaces span html tag to unity text bold
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceSpanToBold(string text)
    {
        return text.Replace("<span class=\"caps\">", "<b>").Replace("</span>", "</b>");
    }

    /// <summary>
    /// Replaces "a href" html tag to unity link
    /// </summary>
    /// <param name="text">origem text to be fixed</param>
    /// <returns>fixed text</returns>
    string ReplaceHrefToLink(string text)
    {
        return text.Replace("<a href=", "<link=").Replace("</a>", "</link>");
    }
}