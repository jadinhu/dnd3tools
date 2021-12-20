/**
* XmlPlayerPrefsContainer.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 30/10/18 (dd/mm/yy)
* Revised on: 30/10/18 (dd/mm/yy)
*/
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("PlayerPrefsCollection")]
public class XmlPlayerPrefsContainer
{
    [XmlArray("XmlPlayerPrefsList"), XmlArrayItem("XmlPlayerPrefs")]
    public XmlPlayerPrefsContainer[] XmlPlayerPrefsArray;
    static List<XmlPlayerPrefs> XmlPlayerPrefsList;

    public void Save(string path)
    {
        UpdatePlayerPrefsXML();
        var serializer = new XmlSerializer(typeof(XmlPlayerPrefsContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static XmlPlayerPrefsContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XmlPlayerPrefsContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as XmlPlayerPrefsContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static XmlPlayerPrefsContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(XmlPlayerPrefsContainer));
        return serializer.Deserialize(new StringReader(text)) as XmlPlayerPrefsContainer;
    }

    static void UpdatePlayerPrefsXML()
    {
        XmlPlayerPrefsList = new List<XmlPlayerPrefs>();
        // get all spells favorited
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("spellsAllNames", PlayerPrefs.GetString("spellsAllNames")));
        // get list of toggle attack (name, label, image, isOn)
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("List of Toggle Attack", PlayerPrefs.GetString("List of Toggle Attack")));
        string toggleAllNames = PlayerPrefs.GetString("List of Toggle Attack");
        if (!string.IsNullOrEmpty(toggleAllNames))
        {
            string[] toggleNameList = toggleAllNames.Split(',');
            for (int i = 0; i < toggleNameList.Length; i++)
            {
                XmlPlayerPrefsList.Add(new XmlPlayerPrefs("Toggle-" + toggleNameList[i], PlayerPrefs.GetString("Toggle-" + toggleNameList[i])));
                XmlPlayerPrefsList.Add(new XmlPlayerPrefs("ToggleImage-" + toggleNameList[i], PlayerPrefs.GetString("ToggleImage-" + toggleNameList[i])));
                XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttack-" + toggleNameList[i], PlayerPrefs.GetString("PanelAttack-" + toggleNameList[i])));
            }
        }
        // get panel attack values
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttackBBA", PlayerPrefs.GetString("PanelAttackBBA")));
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttackStr", PlayerPrefs.GetString("PanelAttackStr")));
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttackModAtk", PlayerPrefs.GetString("PanelAttackModAtk")));
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttackModDmg", PlayerPrefs.GetString("PanelAttackModDmg")));
        XmlPlayerPrefsList.Add(new XmlPlayerPrefs("PanelAttackTwohand", PlayerPrefs.GetString("PanelAttackTwohand")));
    }
}