/**
* Language.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 08/11/18 (dd/mm/yy)
* Revised on: 14/12/21 (dd/mm/yy)
*/

/// <summary>
/// The language used for translator API
/// </summary>
public struct Language
{
    /// <summary>
    /// ID of language
    /// </summary>
    public string code;
    /// <summary>
    /// Name of language
    /// </summary>
    public string language;

    public Language(string code, string language)
    {
        this.code = code;
        this.language = language;
    }
}