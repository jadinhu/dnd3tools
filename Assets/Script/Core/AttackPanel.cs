/**
* AttackPanel.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 06/10/18 (dd/mm/yy)
* Revised on: 14/12/21 (dd/mm/yy)
*/
#if UNITY_ANDROID
using SimpleFileBrowser;
using System.Collections;
#else
using Crosstales.FB;
#endif
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the UI Panel about Attack Calculator Tool
/// </summary>
public class AttackPanel : MonoBehaviour
{
    /// <summary>
    /// List of buttons about keyboard workaround for WebGL
    /// </summary>
    [SerializeField]
    List<GameObject> webKeyboardButtons = new List<GameObject>();
    /// <summary>
    /// The index of this Attack Panel
    /// </summary>
    [SerializeField]
    int index;
    /// <summary>
    /// Panel with toggle buttons (buffs and debuffs)
    /// </summary>
    [SerializeField]
    GameObject panelToggles;
    /// <summary>
    /// Can't remember whats is it rsrs
    /// </summary>
    [SerializeField]
    GameObject panelTogglesTip;
    /// <summary>
    /// The field of Bonus Base Attack
    /// </summary>
    [SerializeField]
    InputField inputBBA;
    /// <summary>
    /// The field of base Strenght
    /// </summary>
    [SerializeField]
    InputField inputStr;
    /// <summary>
    /// If the character are using two-handed weapon (selected)
    /// </summary>
    [SerializeField]
    Toggle toggleStrTwohand;
    /// <summary>
    /// The field of miscellaneous attack modifiers
    /// </summary>
    [SerializeField]
    InputField inputModAtk;
    /// <summary>
    /// The field of miscellaneous damage modifiers
    /// </summary>
    [SerializeField]
    InputField inputModDmg;
    /// <summary>
    /// Result of the total attack modifier
    /// </summary>
    [SerializeField]
    Text textTotalAttack;
    /// <summary>
    /// Result of the total damage modifier
    /// </summary>
    [SerializeField]
    Text textTotalDamage;
    /// <summary>
    /// Toggle base to be cloned for each new buff/debuff toggle
    /// </summary>
    [SerializeField]
    Toggle toggleBase;
    /// <summary>
    /// List of UI elements for navegation in left-menu of attack calculator tool 
    /// </summary>
    [SerializeField]
    List<InputField> tabNagivation;
    /// <summary>
    /// The panel to create new toggles
    /// </summary>
    [SerializeField]
    GameObject panelAddToggle;
    /// <summary>
    /// Field to insert a new toggle's name
    /// </summary>
    [SerializeField]
    InputField inputToggleName;
    /// <summary>
    /// Field to insert a new toggle's attack modifier
    /// </summary>
    [SerializeField]
    InputField inputToggleAttack;
    /// <summary>
    /// Field to insert a new toggle's damage modifier
    /// </summary>
    [SerializeField]
    InputField inputToggleDamage;
    /// <summary>
    /// Field to insert a new toggle's strenght modifier
    /// </summary>
    [SerializeField]
    InputField inputToggleStr;
    /// <summary>
    /// If the new toggle modifier strenght
    /// </summary>
    [SerializeField]
    Toggle toggleStr;
    /// <summary>
    /// The image of new toggle
    /// </summary>
    [SerializeField]
    Image toggleImageButton;
    /// <summary>
    /// The panel with toogle's name to shows on mouse enter
    /// </summary>
    [SerializeField]
    GameObject panelToggleName;
    /// <summary>
    /// Panel about delete a toggle
    /// </summary>
    [SerializeField]
    GameObject panelDeleteToggle;
    /// <summary>
    /// Name of toggle to be deleted
    /// </summary>
    [SerializeField]
    Text deleteToggleName;
    /// <summary>
    /// Attack of toggle to be deleted
    /// </summary>
    [SerializeField]
    Text deleteToggleAttack;
    /// <summary>
    /// Damage of toggle to be deleted
    /// </summary>
    [SerializeField]
    Text deleteToggleDamage;
    /// <summary>
    /// Strenght of toggle to be deleted
    /// </summary>
    [SerializeField]
    Text deleteToggleStr;
    /// <summary>
    /// Image of toggle to be deleted
    /// </summary>
    [SerializeField]
    Image deleteToggleImage;
    /// <summary>
    /// List of UI elements for navegation in new toggle creation panel
    /// </summary>
    [SerializeField]
    List<InputField> addToggleNagivation;
    /// <summary>
    /// List of all toggles created
    /// </summary>
    List<Toggle> attackToggles = new List<Toggle>();
    /// <summary>
    /// The event system of this scene
    /// </summary>
    EventSystem system;
    /// <summary>
    /// The holded toggle (used to check if user wants delete it)
    /// </summary>
    Toggle toggleHolding;
    /// <summary>
    /// Sprite created from <see cref="textureNewToggle"/> to load a image for new toogle
    /// </summary>
    static Sprite spriteNewToggle;
    /// <summary>
    /// The texture loaded from a file, to create a image for new toggle
    /// </summary>
    static Texture2D textureNewToggle;
    /// <summary>
    /// The path of the image to new toggle
    /// </summary>
    static string pathImageToogle;
    /// <summary>
    /// Total attack modifier calculated
    /// </summary>
    int totalAttack;
    /// <summary>
    /// Total damage modifier calculated
    /// </summary>
    int totalDamage;
    /// <summary>
    /// Total strenght bonus calculated
    /// </summary>
    int strBonus;
    /// <summary>
    /// Counter time of holded toggle (to delete)
    /// </summary>
    float timeHoldingToggleAttack;
    /// <summary>
    /// If the initialization setup are finished
    /// </summary>
    bool initialized = false;
#if UNITY_ANDROID
    FileBrowser.OnSuccess loadImageSuccess;
    FileBrowser.OnCancel loadImageCancel;
#endif

    /// <summary>
    /// Check if user are holding a toggle to delete it and if they are using tab navegation
    /// </summary>
    void Update()
    {
        if (!initialized)
            return;
        TabNavigation();
        HoldingToogleAttack();
    }

    /// <summary>
    /// Return the list of attack toggles <see cref="attackToggles"/>
    /// </summary>
    public List<Toggle> GetAttackToggles()
    {
        return attackToggles;
    }

    /// <summary>
    /// Shows or hides the panel about new toggle <see cref="panelAddToggle"/>
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowAddToggle(bool open)
    {
        panelAddToggle.SetActive(open);
    }

    /// <summary>
    /// Change the bonus type of toggle, between strenght and explicit value
    /// </summary>
    public void ChangeBonusToggle()
    {
        inputToggleStr.interactable = toggleStr.isOn;
        inputToggleAttack.interactable = inputToggleDamage.interactable = !toggleStr.isOn;
    }

    /// <summary>
    /// Select a image for a new toggle from browser
    /// </summary>
    public void SelectImageToggle()
    {
        try
        {
#if UNITY_ANDROID
            FileBrowser.ShowLoadDialog(loadImageSuccess, loadImageCancel);
#else
            pathImageToogle = FileBrowser.OpenSingleFile("Open File", "",
                new ExtensionFilter[] { new ExtensionFilter("Image Files", new string[] { "jpg", "png" }) });
            SucessLoadingImage(pathImageToogle);
#endif
        }
        catch (System.Exception e)
        {
            print(e.Message);
            ErrorLoadingImage();
        }
    }

    /// <summary>
    /// Finish process to a new toggle to Attack Panel
    /// </summary>
    public void AddToggle()
    {
        GameObject toggleObj = Instantiate(toggleBase.gameObject);
        toggleObj.SetActive(true);
        toggleObj.name = inputToggleName.text;
        Toggle toggle = toggleObj.GetComponent<Toggle>();
        toggle.isOn = false;
        // next two lines fix unity bug
        toggle.GetComponentInChildren<RawImage>().enabled = true;
        toggle.GetComponentInChildren<RawImage>().enabled = false;
        try
        {
            ((Image)toggle.targetGraphic).sprite = ((Image)toggle.graphic).sprite = spriteNewToggle;
            if (toggleStr.isOn)
            {
                int str = int.Parse(inputToggleStr.text);
                if (str >= 0)
                    toggleObj.GetComponentInChildren<Text>().text = "STR +" + str;
                else
                    toggleObj.GetComponentInChildren<Text>().text = "STR " + str;
            }
            else
            {
                int atk = int.Parse(inputToggleAttack.text.Trim());
                int dmg = int.Parse(inputToggleDamage.text.Trim());
                if (atk >= 0)
                    toggleObj.GetComponentInChildren<Text>().text = "+" + atk;
                else
                    toggleObj.GetComponentInChildren<Text>().text = "" + atk;
                if (dmg >= 0)
                    toggleObj.GetComponentInChildren<Text>().text += "/+" + dmg;
                else
                    toggleObj.GetComponentInChildren<Text>().text += "/" + dmg;
            }
            // fix parent, position and size
            toggleObj.transform.SetParent(toggleBase.transform.parent);
            toggleObj.transform.localScale = toggleBase.transform.localScale;
            toggleObj.transform.localPosition = toggleBase.transform.localPosition;
            // destroy the first base toggle
            if (!toggleBase.gameObject.activeSelf)
            {
                print("toggleBase destroyed");
                Destroy(toggleBase.gameObject);
                toggleBase = toggle;
            }
            else
            {
                // destroy old toggle with same name
                for (int i = 0; i < attackToggles.Count; i++)
                {
                    if (attackToggles[i].name.Equals(toggle.name))
                    {
                        Destroy(attackToggles[i].gameObject);
                        attackToggles.RemoveAt(i);
                        print("toggleBase destroyed (same name)");
                        toggleBase = toggle;
                        break;
                    }
                }
            }
            toggle.onValueChanged.AddListener(delegate { UpdateValues(); });
            attackToggles.Add(toggle);
            Dao.Instance.SaveOrUpdateToggleAttack(toggle, textureNewToggle);
        }
        catch (System.Exception e)
        {
            Destroy(toggleObj);
            print(e.ToString());
        }
        ShowAddToggle(false);
    }

    /// <summary>
    /// Shows the toggle's name panel on mouse enter
    /// </summary>
    /// <param name="toggle"></param>
    public void ShowAttackToggleName(Toggle toggle)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return;
        panelToggleName.GetComponentInChildren<Text>().text = toggle.name;
        panelToggleName.SetActive(true);
    }

    /// <summary>
    /// Disable the toggle's name on mouse exit
    /// </summary>
    public void RemoveAttackToggleName()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            return;
        panelToggleName.SetActive(false);
    }

    /// <summary>
    /// Sets the toggle holded by pressed button
    /// </summary>
    /// <param name="toggle">the toggle holded</param>
    public void HoldToggle(Toggle toggle)
    {
        toggleHolding = toggle;
    }

    /// <summary>
    /// Leave the holded toggle in <see cref="HoldToggle(Toggle)"/>
    /// </summary>
    public void LeaveToggle()
    {
        toggleHolding = panelDeleteToggle.activeSelf ? toggleHolding : null;
    }

    /// <summary>
    /// Shows or hides the panel of toggle deletion <see cref="panelDeleteToggle"/>
    /// </summary>
    /// <param name="open">shows if true</param>
    public void ShowDeleteToggleAttack(bool open)
    {
        if (open && toggleHolding)
        {
            string label = toggleHolding.GetComponentInChildren<Text>().text;
            if (label.Contains("STR"))
                deleteToggleStr.text = label.Substring(4);
            else
            {
                deleteToggleAttack.text = label.Substring(0, label.IndexOf("/"));
                deleteToggleDamage.text = label.Substring(label.IndexOf("/") + 1);
            }
            deleteToggleName.text = toggleHolding.name;
            deleteToggleImage.sprite = ((Image)toggleHolding.targetGraphic).sprite;
            panelDeleteToggle.SetActive(true);
        }
        else
        {
            panelDeleteToggle.SetActive(false);
            toggleHolding = null;
        }
        timeHoldingToggleAttack = 0;
    }

    /// <summary>
    /// Confirms that <see cref="toggleHolding"/> must be deleted and turn off <see cref="panelDeleteToggle"/>
    /// </summary>
    public void ConfirmDeleteToggleAttack()
    {
        Dao.Instance.RemoveToggleAttack(toggleHolding);
        attackToggles.Remove(toggleHolding);
        if (toggleHolding == toggleBase)
        {
            toggleHolding.gameObject.SetActive(false);
        }
        else
            Destroy(toggleHolding.gameObject);
        toggleHolding = null;
        panelDeleteToggle.SetActive(false);
    }

    /// <summary>
    /// Calls a coroutine to shows <see cref="panelTogglesTip"/> with fade effect
    /// </summary>
    /// <param name="toggle"></param>
    public void ShowPanelAttackTip(Toggle toggle)
    {
        StartCoroutine(FadePanelAttackTip(toggle.name));
    }

    /// <summary>
    /// Reset the holding state check of <see cref="toggleHolding"/> and turn off <see cref="panelDeleteToggle"/>
    /// </summary>
    public void ResetHolding()
    {
        toggleHolding = null;
        timeHoldingToggleAttack = 0;
        ShowDeleteToggleAttack(false);
    }

    /// <summary>
    /// Updates de status of Attack Calculator Tool
    /// </summary>
    public void UpdateValues()
    {
        try
        {
            int bba = int.Parse(inputBBA.text);
            int str = int.Parse(inputStr.text);
            int modAtk = int.Parse(inputModAtk.text);
            int modDmg = int.Parse(inputModDmg.text);
            strBonus = totalAttack = totalDamage = 0;
            foreach (Toggle toggle in attackToggles)
            {
                if (toggle)
                {
                    toggle.GetComponentInChildren<RawImage>().enabled = toggle.isOn;
                    if (toggle.isOn)
                    {
                        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttack-" + index + "-" + toggle.name, 1);
                        ChangeToggle(toggle);
                    }
                    else
                        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttack-" + index + "-" + toggle.name, 0);
                }
            }
            str += strBonus;
            int strMod = Mathf.RoundToInt((str - 10) / 2);
            totalAttack += strMod + bba + modAtk;
            if (strMod > 0)
            {
                if (toggleStrTwohand.isOn)
                    totalDamage += (int)(strMod * 1.5f) + modDmg;
                else
                    totalDamage += strMod + modDmg;
            }
            else
                totalDamage += modDmg;
            textTotalAttack.text = totalAttack + "";
            textTotalDamage.text = totalDamage + "";
            PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttackBBA-" + index + "-", int.Parse(inputBBA.text));
            PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttackStr-" + index + "-", str - strBonus);
            PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttackModAtk-" + index + "-", int.Parse(inputModAtk.text));
            PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttackModDmg-" + index + "-", int.Parse(inputModDmg.text));
            PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-PanelAttackTwohand-" + index + "-", toggleStrTwohand.isOn ? 1 : 0);
        }
        catch (System.Exception e)
        {
            textTotalAttack.text = "";
            textTotalDamage.text = "";
            print(e.ToString());
        }
    }

    /// <summary>
    /// Updates spellbook data with selected profile
    /// </summary>
    public void UpdateProfile()
    {
        inputBBA.text = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttackBBA-" + index + "-") + "";
        inputStr.text = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttackStr-" + index + "-") + "";
        inputModAtk.text = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttackModAtk-" + index + "-") + "";
        inputModDmg.text = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttackModDmg-" + index + "-") + "";
        toggleStrTwohand.isOn = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttackTwohand-" + index + "-") > 0;
        foreach (Toggle toggle in attackToggles)
        {
            if (toggle && toggle != toggleBase)
                Destroy(toggle.gameObject);
        }
        attackToggles.Clear();
        attackToggles = Dao.Instance.LoadToggleAttack(toggleBase, this);
        if (attackToggles.Count > 0)
        {
            Destroy(toggleBase.gameObject);
            toggleBase = attackToggles[0];
        }
        foreach (Toggle toggle in attackToggles)
        {
            if (toggle)
            {
                toggle.isOn = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-PanelAttack-" + index + "-" + toggle.name) > 0;
                toggle.onValueChanged.AddListener(delegate { UpdateValues(); });
            }
        }
        UpdateValues();
        Support.Instance.UpdateProfileList();
    }

    /// <summary>
    /// Reads the image file on path and updates the toggle image with it
    /// </summary>
    /// <param name="path">the path of image</param>
    public void UpdateToggleImage(string path)
    {
        pathImageToogle = path;
        byte[] data = File.ReadAllBytes(pathImageToogle);
        textureNewToggle = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        textureNewToggle.LoadImage(data);
        textureNewToggle.name = Path.GetFileNameWithoutExtension(pathImageToogle);
        spriteNewToggle = Sprite.Create(textureNewToggle, new Rect(0.0f, 0.0f, textureNewToggle.width, textureNewToggle.height), new Vector2(0.5f, 0.5f), 100);
    }

    /// <summary>
    /// Setup default states and values: default image for new toggles, current data from started profile and 
    /// add delegates for UI options
    /// </summary>
    public void Initialize()
    {
        // event system to tab inputFields
        system = EventSystem.current;
#if UNITY_ANDROID
        // set delegate events about load image file
        loadImageSuccess = SucessLoadingImage;
        loadImageCancel = ErrorLoadingImage;
#endif
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            for (int i = 0; i < webKeyboardButtons.Count; i++)
            {
                webKeyboardButtons[i].SetActive(false);
            }
        }
        // set default image for Toggle Attack
        spriteNewToggle = Resources.Load<Sprite>("bless");
        textureNewToggle = spriteNewToggle.texture;
        textureNewToggle.name = "bless";
        // set playerprefs of panel attack
        UpdateProfile();
        // delegate UpdateValues for inputs (panel attack)
        inputBBA.onValueChanged.AddListener(delegate { UpdateValues(); });
        inputStr.onValueChanged.AddListener(delegate { UpdateValues(); });
        inputModAtk.onValueChanged.AddListener(delegate { UpdateValues(); });
        inputModDmg.onValueChanged.AddListener(delegate { UpdateValues(); });
        toggleStrTwohand.onValueChanged.AddListener(delegate { UpdateValues(); });
        initialized = true;
    }

    /// <summary>
    /// Called when a image file is successful readed from <see cref="SelectImageToggle"/>, 
    /// calls <see cref="UpdateToggleImage(string)"/> and sets <see cref="toggleImageButton"/>
    /// with <see cref="spriteNewToggle"/>
    /// </summary>
    /// <param name="path"></param>
    void SucessLoadingImage(string path)
    {
        UpdateToggleImage(path);
        toggleImageButton.sprite = spriteNewToggle;
    }

    /// <summary>
    /// Called when <see cref="SelectImageToggle"/> got a problem, so prints a error msg on console
    /// </summary>
    void ErrorLoadingImage()
    {
        print("Error loading image :(");
    }

    /// <summary>
    /// Changes the attack toggle between bonus to strenght or explicit integer
    /// </summary>
    /// <param name="toggle">the toggle's bonus to be changed</param>
    void ChangeToggle(Toggle toggle)
    {
        string text = toggle.GetComponentInChildren<Text>().text;
        if (text.Contains("STR"))
        {
            int bonus = int.Parse(text.Substring(5));
            if (text.Contains("-"))
                bonus *= -1;
            strBonus += bonus;
        }
        else
        {
            string textAttack = text.Substring(0, text.IndexOf("/"));
            string textDamage = text.Substring(text.IndexOf("/") + 1);
            if (string.IsNullOrEmpty(textAttack) || string.IsNullOrEmpty(textDamage) ||
                textAttack.Trim().Equals("") || textAttack.Trim().Equals(""))
                return;
            int attack = int.Parse(textAttack);
            int damage = int.Parse(textDamage);
            totalAttack += attack;
            totalDamage += damage;
        }
    }

    /// <summary>
    /// Calculate if user are holding a toggle for long time enought to shows <see cref="panelDeleteToggle"/> or 
    /// resets the time <see cref="timeHoldingToggleAttack"/>
    /// </summary>
    void HoldingToogleAttack()
    {
        if (toggleHolding)
        {
            timeHoldingToggleAttack += Time.deltaTime;
            if (timeHoldingToggleAttack > 1 && !panelDeleteToggle.activeSelf)
                ShowDeleteToggleAttack(true);
        }
        else
            timeHoldingToggleAttack = 0;
    }

    /// <summary>
    /// Check if user are using tab navegation to translate to next UI element
    /// </summary>
    void TabNavigation()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject obj = system.currentSelectedGameObject;
            if (!obj)
                return;
            Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
            if (!current)
                return;
            if (current.GetType() == typeof(InputField))
            {
                InputField inputfield = (InputField)current;
                if (tabNagivation.Contains(inputfield))
                {
                    int index = tabNagivation.IndexOf(inputfield) + 1;
                    if (index == tabNagivation.Count)
                        tabNagivation[0].OnPointerClick(new PointerEventData(system));
                    else
                        tabNagivation[index].OnPointerClick(new PointerEventData(system));
                }
                else if (addToggleNagivation.Contains(inputfield))
                {
                    int index = addToggleNagivation.IndexOf(inputfield) + 1;
                    while (!addToggleNagivation[index].interactable)
                    {
                        index++;
                        if (index == addToggleNagivation.Count)
                            index = 0;
                    }
                    if (index == addToggleNagivation.Count)
                        addToggleNagivation[0].OnPointerClick(new PointerEventData(system));
                    else
                        addToggleNagivation[index].OnPointerClick(new PointerEventData(system));
                }
            }
        }
    }

    /// <summary>
    /// Makes fading effect with <see cref="panelTogglesTip"/> 
    /// </summary>
    /// <param name="name">name to put on text of <see cref="panelTogglesTip"/></param>
    /// <returns></returns>
    System.Collections.IEnumerator FadePanelAttackTip(string name)
    {
        panelTogglesTip.SetActive(true);
        panelTogglesTip.GetComponentInChildren<Text>().text = name;
        yield return new WaitForSeconds(1.5f);
        panelTogglesTip.SetActive(false);
    }
}