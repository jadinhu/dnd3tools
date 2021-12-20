/**
* SpellHandler.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 04/10/18 (dd/mm/yy)
* Revised on: 19/12/21 (dd/mm/yy)
*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all features about Spells Panel
/// </summary>
public class SpellHandler : MonoBehaviour
{
    /// <summary>
    /// Singleton Instance
    /// </summary>
    public static SpellHandler Instance { get; private set; }
    /// <summary>
    /// List of buttons about keyboard workaround for WebGL
    /// </summary>
    [SerializeField]
    GameObject webKeyboardButton; 
    /// <summary>
    /// The scrollview with spell level filter to search
    /// </summary>
    [SerializeField]
    GameObject scrollLevel;
    /// <summary>
    /// The scrollview with buttons of spells in your favorite spell list
    /// </summary>
    [SerializeField]
    GameObject scrollSpells;
    /// <summary>
    /// The reference button to be cloned for each favored spell in <see cref="scrollSpells"/>
    /// </summary>
    [SerializeField]
    Button buttonSpellbook;
    /// <summary>
    /// The panel with spell description
    /// </summary>
    [SerializeField]
    GameObject panelDescription;
    /// <summary>
    /// The text of spell description
    /// </summary>
    [SerializeField]
    Text textSpellDescription;
    /// <summary>
    /// Toggle button to enable or disable the translation of <see cref="textSpellDescription"/>
    /// </summary>
    [SerializeField]
    Toggle toggleTranslateDescription;
    /// <summary>
    /// Turns a spell favorite or unfavorite it
    /// </summary>
    [SerializeField]
    Button buttonSpellFavorite;
    /// <summary>
    /// Panel to confirms if the user really wants unfavorite the spell
    /// </summary>
    [SerializeField]
    GameObject panelRemoveSpell;
    /// <summary>
    /// Panel to select the language to translate the <see cref="textSpellDescription"/>
    /// </summary>
    [SerializeField]
    GameObject panelLanguage;
    /// <summary>
    /// Panel about the error msg when trying translate <see cref="textSpellDescription"/>
    /// </summary>
    [SerializeField]
    GameObject panelLanguageBadRequest;
    /// <summary>
    /// Panel to search spells
    /// </summary>
    [SerializeField]
    GameObject panelSearchSpell;
    /// <summary>
    /// Panel with all source books to select
    /// </summary>
    [SerializeField]
    GameObject panelSourceBooks;
    /// <summary>
    /// The text with msg "not found" about the search
    /// </summary>
    [SerializeField]
    GameObject textSearchNotFound;
    /// <summary>
    /// The text to search a spell with this text on title or description
    /// </summary>
    [SerializeField]
    InputField inputSearchSpell;
    /// <summary>
    /// List of spell classes 
    /// </summary>
    [SerializeField]
    Dropdown dropSearchClassSpell;
    /// <summary>
    /// List of spell levels
    /// </summary>
    [SerializeField]
    Dropdown dropSearchLevelSpell;
    /// <summary>
    /// The reference button to be cloned for each spell founded on search
    /// </summary>
    [SerializeField]
    Button spellResultButton;
    /// <summary>
    /// Panel to select a level list to put the new favorited spell
    /// </summary>
    [SerializeField]
    GameObject panelClassLevel;
    /// <summary>
    /// If the spell filter options about saving throws
    /// </summary>
    [SerializeField]
    Dropdown dropSaveYes;
    /// <summary>
    /// If the spell filter options about spell resistance
    /// </summary>
    [SerializeField]
    Dropdown dropSpellResistence;
    /// <summary>
    /// (For developer usage) if must clean the local data on next initialization
    /// </summary>
    [SerializeField]
    bool cleanPlayerPrefs;
    /// <summary>
    /// Callback of finished translation proccess
    /// </summary>
    GoogleTranslate.FinishTranslate finishTranslate;
    /// <summary>
    /// Callback to get <see cref="languageList"/>
    /// </summary>
    GoogleTranslate.FinishGetEnglishCodes finishGetEnglishCodes;
    /// <summary>
    /// List of languages to translate spell descriptions
    /// </summary>
    List<Language> languageList;
    /// <summary>
    /// List of <see cref="buttonSpellbook"/> (favored spells)
    /// </summary>
    List<Button> spellsFavoredButtons = new List<Button>();
    /// <summary>
    /// List of <see cref="spellResultButton"/>
    /// </summary>
    List<Button> spellsResultButtons = new List<Button>();
    /// <summary>
    /// List of spells on result of search
    /// </summary>
    List<Spell> resultSpells;
    /// <summary>
    /// List of favored spells level 0
    /// </summary>
    List<Spell> spells0;
    /// <summary>
    /// List of favored spells level 1
    /// </summary>
    List<Spell> spells1;
    /// <summary>
    /// List of favored spells level 2
    /// </summary>
    List<Spell> spells2;
    /// <summary>
    /// List of favored spells level 3
    /// </summary>
    List<Spell> spells3;
    /// <summary>
    /// List of favored spells level 4
    /// </summary>
    List<Spell> spells4;
    /// <summary>
    /// List of favored spells level 5
    /// </summary>
    List<Spell> spells5;
    /// <summary>
    /// List of favored spells level 6
    /// </summary>
    List<Spell> spells6;
    /// <summary>
    /// List of favored spells level 7
    /// </summary>
    List<Spell> spells7;
    /// <summary>
    /// List of favored spells level 8
    /// </summary>
    List<Spell> spells8;
    /// <summary>
    /// List of favored spells level 9
    /// </summary>
    List<Spell> spells9;
    /// <summary>
    /// Selected spell to shows description
    /// </summary>
    Spell spellSelected;
    /// <summary>
    /// Color of the star when favorite is active
    /// </summary>
    Color starFavoriteColor;
    /// <summary>
    /// Color of the translate UI when translation is active
    /// </summary>
    Color translateOriginalColor;
    /// <summary>
    /// Original text of spell description
    /// </summary>
    string originalSpellDescription;
    /// <summary>
    /// Name of language to translate spell descriptions
    /// </summary>
    string languageToTranslate;
    /// <summary>
    /// Source book name of the displayed spell <see cref="spellSelected"/>
    /// </summary>
    string spellSourceBooks;
    /// <summary>
    /// If the search can be done (search is automatic when typing a text in <see cref="inputSearchSpell"/>, but must
    /// obey some rules for that)
    /// </summary>
    bool canSubmitSearch;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        CheckInputSearch();
    }

    /// <summary>
    /// Search a spells based on filters and text on <see cref="inputSearchSpell"/>
    /// </summary>
    public void SearchSpell()
    {
        // fix fields
        string spellClass = dropSearchClassSpell.options[dropSearchClassSpell.value].text;
        string spellLevel = dropSearchLevelSpell.options[dropSearchLevelSpell.value].text;
        int save = dropSaveYes.value;
        int spellResistence = dropSpellResistence.value;
        // fix values before search
        if (spellClass.Equals("All"))
            spellClass = null;
        if (spellLevel.Equals("All"))
            spellLevel = null;
        if (spellSourceBooks.Equals("All"))
            spellSourceBooks = null;
        // check if there is no filter
        if (spellClass == null && spellLevel == null && spellSourceBooks == null
            && string.IsNullOrEmpty(inputSearchSpell.text))
            return;
        // get result from HtmlToDatabase
        resultSpells = Dao.Instance.SearchSpell(inputSearchSpell.text, spellClass, spellLevel, 
            spellSourceBooks, save, spellResistence);
        // save fields
        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-SearchPanel-class", dropSearchClassSpell.value);
        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-SearchPanel-level", dropSearchLevelSpell.value);
        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-SearchPanel-save", dropSaveYes.value);
        PlayerPrefs.SetInt(Dao.Instance.ProfileName + "-SearchPanel-sr", dropSpellResistence.value);
        PlayerPrefs.SetString(Dao.Instance.ProfileName + "-SearchPanel-sourcebook", spellSourceBooks);
        // load UI with results
        LoadButtons(resultSpells, spellResultButton);
    }

    /// <summary>
    /// Loads a button for each spell on favored list by spell level
    /// </summary>
    /// <param name="level">level of spells to be loaded</param>
    public void LoadSpellsButtons(int level)
    {
        switch (level)
        {
            case 0:
                LoadButtons(spells0, buttonSpellbook);
                break;
            case 1:
                LoadButtons(spells1, buttonSpellbook);
                break;
            case 2:
                LoadButtons(spells2, buttonSpellbook);
                break;
            case 3:
                LoadButtons(spells3, buttonSpellbook);
                break;
            case 4:
                LoadButtons(spells4, buttonSpellbook);
                break;
            case 5:
                LoadButtons(spells5, buttonSpellbook);
                break;
            case 6:
                LoadButtons(spells6, buttonSpellbook);
                break;
            case 7:
                LoadButtons(spells7, buttonSpellbook);
                break;
            case 8:
                LoadButtons(spells8, buttonSpellbook);
                break;
            case 9:
                LoadButtons(spells9, buttonSpellbook);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Add the <see cref="spellSelected"/> to favored list
    /// </summary>
    public void AddSpellToMyBook()
    {
        Spell spell = FindSpellInBook(spellSelected.id, out int level);
        if (string.IsNullOrEmpty(spell.name))
            ShowFavoredClassLevel();
        else
            ShowRemoveSpell(true);
    }

    /// <summary>
    /// Closes <see cref="scrollSpells"/> and opens <see cref="scrollLevel"/>
    /// </summary>
    public void BackToScreenLevel()
    {
        scrollLevel.SetActive(true);
        scrollSpells.SetActive(false);
        AudioHandler.Instance.Play(AudioList.CloseBook);
    }

    /// <summary>
    /// Closes <see cref="panelDescription"/> of <see cref="spellSelected"/>
    /// </summary>
    public void ClosePanelDescription()
    {
        panelDescription.SetActive(false);
        AudioHandler.Instance.Play(AudioList.CloseSpellDescription);
        Admob.Instance.ShowInterstitial();
    }

    /// <summary>
    /// Opens <see cref="panelDescription"/> of <see cref="spellSelected"/>
    /// </summary>
    public void ShowPanelDescription(Button btn)
    {
        AudioHandler.Instance.Play(AudioList.OpenSpellDescription);
        spellSelected = btn.GetComponent<HolderReference>().Spell;
        int level;
        Spell spell = FindSpellInBook(spellSelected.id, out level);
        if (!string.IsNullOrEmpty(spell.name))
            buttonSpellFavorite.GetComponentInChildren<RawImage>().color = Color.blue;
        else
            buttonSpellFavorite.GetComponentInChildren<RawImage>().color = starFavoriteColor;
        originalSpellDescription = textSpellDescription.text = spellSelected.description + "\n\n\n";
        TranslateLanguage();
        panelDescription.SetActive(true);
        panelDescription.GetComponentInChildren<Scrollbar>().value = 0;
    }

    /// <summary>
    /// Translate the <see cref="spellSelected"/> description to <see cref="languageToTranslate"/> 
    /// (if <see cref="toggleTranslateDescription"/> is checked) or
    /// back the text to original (if <see cref="toggleTranslateDescription"/> is unchecked)
    /// </summary>
    public void TranslateLanguage()
    {
        if (toggleTranslateDescription.isOn && toggleTranslateDescription.interactable)
        {
            toggleTranslateDescription.interactable = false;
            originalSpellDescription = textSpellDescription.text;
            GoogleTranslate.Instance.Translate(languageToTranslate, textSpellDescription.text, finishTranslate);
            toggleTranslateDescription.targetGraphic.color = Color.blue;
            PlayerPrefs.SetInt("DescriptionPanel-toggleTranslate", 1);
        }
        else
        {
            textSpellDescription.text = originalSpellDescription + "\n\n\n";
            toggleTranslateDescription.targetGraphic.color = translateOriginalColor;
            PlayerPrefs.SetInt("DescriptionPanel-toggleTranslate", 0);
        }
    }

    /// <summary>
    /// Increases or decreases the size of <see cref="textSpellDescription"/>
    /// </summary>
    /// <param name="plus">number to be added to current size</param>
    public void PanelDescriptionZoom(bool plus)
    {
        if (plus)
            textSpellDescription.fontSize += 4;
        else
            textSpellDescription.fontSize -= 4;
        PlayerPrefs.SetInt("SpellDescriptionFontSize", textSpellDescription.fontSize);
    }

    /// <summary>
    /// Closes the <see cref="panelLanguage"/> without translate anything
    /// </summary>
    public void CancelPanelLanguage()
    {
        toggleTranslateDescription.isOn = false;
        ShowPanelLanguage(false);
    }

    /// <summary>
    /// Close the <see cref="panelLanguage"/>
    /// </summary>
    /// <param name="open"></param>
    public void ShowPanelLanguage(bool open)
    {
        panelLanguage.SetActive(open);
    }

    /// <summary>
    /// Select the language to translate <see cref="textSpellDescription"/>
    /// </summary>
    public void SetLanguage()
    {
        try
        {
            ShowPanelLanguage(false);
            Dropdown languageDropdown = panelLanguage.GetComponentInChildren<Dropdown>();
            string language = languageDropdown.options[languageDropdown.value].text;
            for (int i = 0; i < languageList.Count; i++)
            {
                if (languageList[i].language.Equals(language))
                {
                    languageToTranslate = "en-" + languageList[i].code;
                    break;
                }
            }
            PlayerPrefs.SetInt("DescriptionPanel-languageDrop", languageDropdown.value);
            PlayerPrefs.SetString("DescriptionPanel-language", languageToTranslate);
            TranslateLanguage();
        }
        catch (System.Exception e)
        {
            print("Error setting language:\n" + e.ToString());
        }
    }

    /// <summary>
    /// Exit the app
    /// </summary>
    public void ExitApp()
    {
        Application.Quit();
    }

    /// <summary>
    /// Shows the <see cref="panelSearchSpell"/> and focus on <see cref="inputSearchSpell"/>, or,
    /// closes the panel and shows again <see cref="scrollLevel"/>
    /// </summary>
    /// <param name="open"></param>
    public void ShowSearchSpell(bool open)
    {
        panelSearchSpell.SetActive(open);
        scrollLevel.SetActive(!open);
        if (open)
            inputSearchSpell.Select();
    }

    /// <summary>
    /// Shows the <see cref="panelRemoveSpell"/> or close it
    /// </summary>
    /// <param name="open"></param>
    public void ShowRemoveSpell(bool open)
    {
        panelRemoveSpell.SetActive(open);
    }

    /// <summary>
    /// Confirms the deletion of <see cref="spellSelected"/>
    /// </summary>
    public void RemoveSpellConfirm()
    {
        AudioHandler.Instance.Play(AudioList.RemoveFavoriteSpell);
        panelRemoveSpell.SetActive(false);
        int level = -1;
        spellSelected = FindSpellInBook(spellSelected.id, out level);
        RemoveSpellFromBook(spellSelected, level);
        Dao.Instance.RemoveFavoriteSpell(spellSelected, level);
        buttonSpellFavorite.GetComponentInChildren<RawImage>().color = Color.black;
        LoadSpellsButtons(level);
    }

    /// <summary>
    /// Updates the level of some spell already in favored list
    /// </summary>
    /// <param name="spell">the spell to be updated</param>
    /// <param name="level">new level of this spell</param>
    public void AddUpdateSpell(Spell spell, int level)
    {
        RemoveSpellFromBook(spell, level);
        AddSpell(spell, level + "");
    }

    /// <summary>
    /// Confirms the level of <see cref="spellSelected"/> to put on favored list or
    /// cancels it. And closes <see cref="panelClassLevel"/>
    /// </summary>
    /// <param name="confirm">Confirms if true</param>
    public void ConfirmClassLevelToFavoredSpell(bool confirm)
    {
        if (confirm)
        {
            Toggle selectedToggle = panelClassLevel.GetComponentInChildren<ToggleGroup>().ActiveToggles().FirstOrDefault();
            string level = selectedToggle.GetComponentInChildren<Text>().text;
            Dao.Instance.FavoriteSpell(spellSelected, level);
            AddSpell(spellSelected, level);
            buttonSpellFavorite.GetComponentInChildren<RawImage>().color = Color.blue;
            AudioHandler.Instance.Play(AudioList.FavoriteSpell);
        }
        panelClassLevel.SetActive(false);
    }

    /// <summary>
    /// Shows or hides the <see cref="panelSourceBooks"/>
    /// </summary>
    /// <param name="open"></param>
    public void ShowSourceBooks(bool open)
    {
        panelSourceBooks.SetActive(open);
    }

    /// <summary>
    /// Mark all source books toggles on <see cref="panelSourceBooks"/> as isOn true or false
    /// </summary>
    /// <param name="mark">value of toggle.isOn</param>
    public void MarkSourceBooks(bool mark)
    {
        List<Toggle> toggles = new List<Toggle>();
        toggles.AddRange(panelSourceBooks.GetComponentsInChildren<Toggle>());
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = mark;
        }
    }

    /// <summary>
    /// Confirms the all source books selected
    /// </summary>
    public void ConfirmSourceBooks()
    {
        List<Toggle> toggles = new List<Toggle>();
        toggles.AddRange(panelSourceBooks.GetComponentsInChildren<Toggle>());
        spellSourceBooks = "";
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
                spellSourceBooks += " " + toggles[i].name;
            PlayerPrefs.SetInt("Sourcebook Toggle " + toggles[i].name, toggles[i].isOn == false ? 0 : 1);
        }
    }

    /// <summary>
    /// Shows or hides the translator problem panel <see cref="panelLanguageBadRequest"/>
    /// </summary>
    /// <param name="open">Shows if true</param>
    public void ShowLanguageBadRequest(bool open)
    {
        panelLanguageBadRequest.SetActive(open);
        toggleTranslateDescription.isOn = false;
    }

    /// <summary>
    /// Loads the profile options
    /// </summary>
    public void UpdateProfile()
    {
        try
        {
            dropSearchClassSpell.value = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-SearchPanel-class");
        }
        catch (System.Exception)
        {
            dropSearchClassSpell.value = 0;
        }
        try
        {
            dropSearchLevelSpell.value = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-SearchPanel-level");
        }
        catch (System.Exception)
        {
            dropSearchLevelSpell.value = 0;
        }
        try
        {
            spellSourceBooks = PlayerPrefs.GetString(Dao.Instance.ProfileName + "-SearchPanel-sourcebook");
        }
        catch (System.Exception)
        {
            spellSourceBooks = "All";
        }
        try
        {
            dropSaveYes.value = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-SearchPanel-save");
        }
        catch (System.Exception)
        {
            dropSaveYes.value = 0;
        }
        try
        {
            dropSpellResistence.value = PlayerPrefs.GetInt(Dao.Instance.ProfileName + "-SearchPanel-sr");
        }
        catch (System.Exception)
        {
            dropSpellResistence.value = 0;
        }
        languageToTranslate = PlayerPrefs.GetString("DescriptionPanel-language");
        LoadSpellsDao();
    }

    /// <summary>
    /// Setup the spell data and UI
    /// </summary>
    void Initialize()
    {
        if (Instance == null)
            Instance = this;
        if (cleanPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            print("PlayerPrefs DELETED");
        }
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            webKeyboardButton.SetActive(false);
        starFavoriteColor = buttonSpellFavorite.GetComponentInChildren<RawImage>().color;
        translateOriginalColor = toggleTranslateDescription.targetGraphic.color;
        finishTranslate = FinishTranslate;
        finishGetEnglishCodes = LoadLanguageOptions;
        spellsFavoredButtons.Add(buttonSpellbook);
        spellsResultButtons.Add(spellResultButton);
        LoadClassesList();
        LoadSourceBooks();
        UpdateLanguageOptions();
        toggleTranslateDescription.isOn = PlayerPrefs.GetInt("DescriptionPanel-toggleTranslate") > 0;
        int fontSize = PlayerPrefs.GetInt("SpellDescriptionFontSize");
        if (fontSize >= 40)
            textSpellDescription.fontSize = fontSize;
        UpdateProfile();
        toggleTranslateDescription.onValueChanged.AddListener(delegate { TurnToogleTranslate(); });
    }

    /// <summary>
    /// Shows <see cref="panelLanguage"/> if <see cref="toggleTranslateDescription"/> is on
    /// or calls <see cref="TranslateLanguage"/> if not
    /// </summary>
    void TurnToogleTranslate()
    {
        if (toggleTranslateDescription.isOn)
            ShowPanelLanguage(true);
        else
            TranslateLanguage();
    }

    /// <summary>
    /// Finish the translate process, showing the translated text or a erro msg with <see cref="ShowLanguageBadRequest(bool)"/>
    /// </summary>
    /// <param name="text"></param>
    void FinishTranslate(string text)
    {
        if (text == null)
        {
            ShowLanguageBadRequest(true);
            return;
        }
        textSpellDescription.text = text + "\n\n\n";
        toggleTranslateDescription.interactable = true;
    }

    /// <summary>
    /// Check if the rules to do a spell search is right 
    /// (<see cref="canSubmitSearch"/> is true, <see cref="inputSearchSpell"/> is not empty etc.)
    /// </summary>
    void CheckInputSearch()
    {
        if (canSubmitSearch && inputSearchSpell.text.Trim().Length > 0 &&
            (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            SearchSpell();
            canSubmitSearch = false;
        }
        else
            canSubmitSearch = inputSearchSpell.isFocused;
    }

    /// <summary>
    /// Add a spell to favored list by level and order each level list by spell's name
    /// </summary>
    /// <param name="spell">spell to add</param>
    /// <param name="level">level of the spell</param>
    void AddSpell(Spell spell, string level)
    {
        spell.level = level;
        switch (level)
        {
            case "0":
                spells0.Add(spell);
                spells0 = spells0.OrderBy(si => si.name).ToList();
                break;
            case "1":
                spells1.Add(spell);
                spells1 = spells1.OrderBy(si => si.name).ToList();
                break;
            case "2":
                spells2.Add(spell);
                spells2 = spells2.OrderBy(si => si.name).ToList();
                break;
            case "3":
                spells3.Add(spell);
                spells3 = spells3.OrderBy(si => si.name).ToList();
                break;
            case "4":
                spells4.Add(spell);
                spells4 = spells4.OrderBy(si => si.name).ToList();
                break;
            case "5":
                spells5.Add(spell);
                spells5 = spells5.OrderBy(si => si.name).ToList();
                break;
            case "6":
                spells6.Add(spell);
                spells6 = spells6.OrderBy(si => si.name).ToList();
                break;
            case "7":
                spells7.Add(spell);
                spells7 = spells7.OrderBy(si => si.name).ToList();
                break;
            case "8":
                spells8.Add(spell);
                spells8 = spells8.OrderBy(si => si.name).ToList();
                break;
            case "9":
                spells9.Add(spell);
                spells9 = spells9.OrderBy(si => si.name).ToList();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Loads the list of classes
    /// </summary>
    void LoadClassesList()
    {
        string classesText = Resources.Load<TextAsset>("Classes").text;
        List<string> classesList = new List<string>();
        classesList.AddRange(classesText.Split("\n"[0]));
        for (int i = 0; i < classesList.Count; i++)
        {
            classesList[i] = classesList[i].TrimEnd();
        }
        dropSearchClassSpell.ClearOptions();
        dropSearchClassSpell.AddOptions(classesList);
        dropSearchClassSpell.RefreshShownValue();
    }

    /// <summary>
    /// Loads the source books
    /// </summary>
    void LoadSourceBooks()
    {
        string sourcesText = Resources.Load<TextAsset>("Sources").text;
        List<string> sourcesList = new List<string>();
        sourcesList.AddRange(sourcesText.Split("\n"[0]));
        for (int i = 0; i < sourcesList.Count; i++)
        {
            sourcesList[i] = sourcesList[i].TrimEnd();
        }
        List<Toggle> toggles = new List<Toggle>();
        toggles.AddRange(panelSourceBooks.GetComponentsInChildren<Toggle>());
        for (int i = 1; i < toggles.Count; i++)
        {
            Destroy(toggles[i].gameObject);
        }
        for (int i = 0; i < sourcesList.Count; i++)
        {
            Toggle newToggle = toggles[0];
            if (i > 0)
            {
                newToggle = Instantiate(toggles[0]);
                newToggle.transform.SetParent(toggles[0].transform.parent);
                newToggle.transform.localScale = toggles[0].transform.localScale;
                newToggle.transform.localPosition = toggles[0].transform.localPosition;
            }
            newToggle.GetComponentInChildren<Text>().text = newToggle.name = sourcesList[i];
            newToggle.isOn = PlayerPrefs.GetInt("Sourcebook Toggle " + newToggle.name, 0) == 0 ? false : true;
        }
    }

    /// <summary>
    /// Find the spell by id on favored list
    /// </summary>
    /// <param name="spellId">id of spell to find</param>
    /// <param name="level">level of favored spell</param>
    /// <returns>the spell founded</returns>
    Spell FindSpellInBook(string spellId, out int level)
    {
        foreach (Spell spell in spells0)
        {
            level = 0;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells1)
        {
            level = 1;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells2)
        {
            level = 2;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells3)
        {
            level = 3;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells4)
        {
            level = 4;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells5)
        {
            level = 5;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells6)
        {
            level = 6;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells7)
        {
            level = 7;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells8)
        {
            level = 8;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        foreach (Spell spell in spells9)
        {
            level = 9;
            if (spell.id.Equals(spellId, System.StringComparison.InvariantCultureIgnoreCase))
                return spell;
        }
        level = -1;
        return new Spell();
    }

    /// <summary>
    /// Shows <see cref="panelClassLevel"/> and calculate all levels with favored spells to show the buttons
    /// </summary>
    void ShowFavoredClassLevel()
    {
        panelClassLevel.SetActive(true);
        int minLevel = 9;
        foreach (string classLevel in spellSelected.classLevel)
        {
            int level = 9;
            int.TryParse(classLevel.Substring(classLevel.Length - 1), out level);
            if (level < minLevel)
                minLevel = level;
        }
        List<Toggle> toggles = panelClassLevel.GetComponentInChildren<ToggleGroup>().ActiveToggles().ToList();
        for (int i = 0; i < toggles.Count; i++)
        {
            print(toggles[i].GetComponentInChildren<Text>().text);
            if (toggles[i].GetComponentInChildren<Text>().text.Equals(minLevel))
            {
                toggles[i].isOn = true;
                return;
            }
        }
    }

    /// <summary>
    /// Removes a spell from favored list
    /// </summary>
    /// <param name="spell">spell to br removed</param>
    /// <param name="level">level of spell</param>
    void RemoveSpellFromBook(Spell spell, int level)
    {
        print("remove " + spell.name + " in level " + level);
        if (level == 0)
            spells0.Remove(spell);
        else if (level == 1)
            spells1.Remove(spell);
        else if (level == 2)
            spells2.Remove(spell);
        else if (level == 3)
            spells3.Remove(spell);
        else if (level == 4)
            spells4.Remove(spell);
        else if (level == 5)
            spells5.Remove(spell);
        else if (level == 6)
            spells6.Remove(spell);
        else if (level == 7)
            spells7.Remove(spell);
        else if (level == 8)
            spells8.Remove(spell);
        else if (level == 9)
            spells9.Remove(spell);
        if (scrollSpells.activeSelf)
        {
            if (spellsFavoredButtons.Count == 0)
                return;
            if (spellsFavoredButtons.Count == 1)
            {
                buttonSpellbook = spellsFavoredButtons[0];
                buttonSpellbook.gameObject.SetActive(false);
                return;
            }
            for (int i = 0; i < spellsFavoredButtons.Count; i++)
            {
                if (spellsFavoredButtons[i].GetComponent<HolderReference>().Spell.id.Equals(spell.id))
                {
                    print("removing " + spell.id + " at " + i);
                    Destroy(spellsFavoredButtons[i].gameObject);
                    spellsFavoredButtons.RemoveAt(i);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Loads the favored spell list of all levels and order it by spell's name
    /// </summary>
    void LoadSpellsDao()
    {
        spells0 = Dao.Instance.LoadFavoriteSpellsByLevel("0");
        spells1 = Dao.Instance.LoadFavoriteSpellsByLevel("1");
        spells2 = Dao.Instance.LoadFavoriteSpellsByLevel("2");
        spells3 = Dao.Instance.LoadFavoriteSpellsByLevel("3");
        spells4 = Dao.Instance.LoadFavoriteSpellsByLevel("4");
        spells5 = Dao.Instance.LoadFavoriteSpellsByLevel("5");
        spells6 = Dao.Instance.LoadFavoriteSpellsByLevel("6");
        spells7 = Dao.Instance.LoadFavoriteSpellsByLevel("7");
        spells8 = Dao.Instance.LoadFavoriteSpellsByLevel("8");
        spells9 = Dao.Instance.LoadFavoriteSpellsByLevel("9");
        spells0 = spells0.OrderBy(si => si.name).ToList();
        spells1 = spells1.OrderBy(si => si.name).ToList();
        spells2 = spells2.OrderBy(si => si.name).ToList();
        spells3 = spells3.OrderBy(si => si.name).ToList();
        spells4 = spells4.OrderBy(si => si.name).ToList();
        spells5 = spells5.OrderBy(si => si.name).ToList();
        spells6 = spells6.OrderBy(si => si.name).ToList();
        spells7 = spells7.OrderBy(si => si.name).ToList();
        spells8 = spells8.OrderBy(si => si.name).ToList();
        spells9 = spells9.OrderBy(si => si.name).ToList();
    }

    /// <summary>
    /// Create a list of buttons for favorite spellbook or search result
    /// </summary>
    /// <param name="spells">the spell list to create the buttons</param>
    /// <param name="spellBtn">the button reference to be cloned</param>
    void LoadButtons(List<Spell> spells, Button spellBtn)
    {
        if (spells == null || spells.Count == 0)
        {
            if (spellBtn == spellResultButton)
            {
                for (int i = 1; i < spellsResultButtons.Count; i++)
                {
                    Destroy(spellsResultButtons[i].gameObject);
                }
                spellsResultButtons.Clear();
                spellsResultButtons.Add(spellResultButton);
                spellResultButton.gameObject.SetActive(false);
                textSearchNotFound.SetActive(true);
            }
            return;
        }
        if (spellBtn == buttonSpellbook)
        {
            for (int i = 1; i < spellsFavoredButtons.Count; i++)
            {
                Destroy(spellsFavoredButtons[i].gameObject);
            }
            spellBtn = spellsFavoredButtons[0];
            spellsFavoredButtons.Clear();
        }
        if (spellBtn == spellResultButton)
        {
            for (int i = 1; i < spellsResultButtons.Count; i++)
            {
                Destroy(spellsResultButtons[i].gameObject);
            }
            spellBtn = spellsResultButtons[0];
            spellsResultButtons.Clear();
            textSearchNotFound.SetActive(false);
        }
        for (int i = 0; i < spells.Count; i++)
        {
            if (string.IsNullOrEmpty(spells[i].name))
                continue;
            Button btn = spellBtn;
            if (i > 0)
            {
                btn = Instantiate(spellBtn);
                btn.transform.SetParent(spellBtn.transform.parent);
                btn.transform.localScale = spellBtn.transform.localScale;
                btn.transform.localPosition = spellBtn.transform.localPosition;
            }
            btn.gameObject.SetActive(true);
            btn.name = btn.GetComponentInChildren<Text>().text = spells[i].name;
            btn.GetComponent<HolderReference>().Spell = spells[i];
            ChangeSquareColor(btn, spells[i].school);
            if (spellBtn == buttonSpellbook)
                spellsFavoredButtons.Add(btn);
            if (spellBtn == spellResultButton)
                spellsResultButtons.Add(btn);
        }
        if (spellBtn == buttonSpellbook)
        {
            scrollLevel.SetActive(false);
            if (!panelSearchSpell.activeSelf)
                scrollSpells.SetActive(true);
        }
        if (spellBtn == spellResultButton)
            spellResultButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Sets the spell school's color on button's circle
    /// </summary>
    /// <param name="btn">the button with circle</param>
    /// <param name="school">name of school</param>
    void ChangeSquareColor(Button btn, string school)
    {
        Color schoolColor = new Color();
        if (school.Contains("Abjuration"))
            ColorUtility.TryParseHtmlString("#fff700", out schoolColor);
        else if (school.Contains("Conjuration"))
            ColorUtility.TryParseHtmlString("#000066", out schoolColor);
        else if (school.Contains("Divination"))
            ColorUtility.TryParseHtmlString("#33cccc", out schoolColor);
        else if (school.Contains("Enchantment"))
            ColorUtility.TryParseHtmlString("#ff33cc", out schoolColor);
        else if (school.Contains("Evocation"))
            ColorUtility.TryParseHtmlString("#990000", out schoolColor);
        else if (school.Contains("Illusion"))
            ColorUtility.TryParseHtmlString("#00cc49", out schoolColor);
        else if (school.Contains("Necromancy"))
            ColorUtility.TryParseHtmlString("#9933ff", out schoolColor);
        else if (school.Contains("Transmutation"))
            ColorUtility.TryParseHtmlString("#996600", out schoolColor);
        else if (school.Contains("Universal"))
            ColorUtility.TryParseHtmlString("#b3b3b3", out schoolColor);
        btn.GetComponentInChildren<RawImage>().color = schoolColor;
    }

    /// <summary>
    /// Calls the <see cref="GoogleTranslate.FinishGetEnglishCodes"/> to updates the language used in last time
    /// </summary>
    void UpdateLanguageOptions()
    {
        GoogleTranslate.Instance.GetEnglishCodeTranslations(finishGetEnglishCodes);
    }

    /// <summary>
    /// Loads the menu options about language translator
    /// </summary>
    /// <param name="languages">list of avaliable languages</param>
    void LoadLanguageOptions(List<Language> languages)
    {
        if (languages == null)
        {
            ShowLanguageBadRequest(true);
            return;
        }
        languageList = languages;
        Dropdown languageDropdown = panelLanguage.GetComponentInChildren<Dropdown>();
        List<string> options = new List<string>();
        for (int i = 0; i < languages.Count; i++)
        {
            options.Add(languages[i].language);
        }
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(options);
        languageDropdown.value = PlayerPrefs.GetInt("DescriptionPanel-languageDrop");
        languageDropdown.RefreshShownValue();
    }
}