/**
* XmlPlayerPrefs.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 30/10/18 (dd/mm/yy)
* Revised on: 30/10/18 (dd/mm/yy)
*/
using System.Xml;
using System.Xml.Serialization;

public class XmlPlayerPrefs
{
    [XmlAttribute("key")]
    public string key;
    public string value;

    public XmlPlayerPrefs(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}