/**
* Spell.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 04/10/18 (dd/mm/yy)
* Revised on: 14/12/21 (dd/mm/yy)
*/
using System.Collections.Generic;

/// <summary>
/// Struct of any spell in database
/// </summary>
public struct Spell
{
    /// <summary>
    /// ID of this spell
    /// </summary>
    public string id;
    /// <summary>
    /// Title ofthis  spell
    /// </summary>
    public string name;
    /// <summary>
    /// Source book of this spell
    /// </summary>
    public string sourcebook;
    /// <summary>
    /// Spell description
    /// </summary>
    public string description;
    /// <summary>
    /// School of this spell
    /// </summary>
    public string school;
    /// <summary>
    /// Spell level
    /// </summary>
    public string level;
    /// <summary>
    /// If this spell invokes a saving throw
    /// </summary>
    public bool save;
    /// <summary>
    /// If this spell invokes a spell resistence check
    /// </summary>
    public bool spellResistence;
    /// <summary>
    /// List of classes with this spell in theirs spell list (ex.: Wizard 4)
    /// </summary>
    public List<string> classLevel;

    public Spell(string id, string name, string sourcebook, string description, string school,
        bool save, bool spellResistence, List<string> classLevel)
    {
        this.id = id;
        this.name = name;
        this.sourcebook = sourcebook;
        this.description = description;
        this.school = school;
        this.save = save;
        this.spellResistence = spellResistence;
        this.classLevel = classLevel;
        level = null;
    }
}