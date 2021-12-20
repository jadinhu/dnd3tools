/**
* KeyboardManual.cs
* Created by: Jadson Almeida [jadson.sistemas@gmail.com]
* Created on: 14/03/19 (dd/mm/yy)
* Revised on: 14/03/19 (dd/mm/yy)
*/
using UnityEngine;
using UnityEngine.UI;

public class KeyboardManual : MonoBehaviour
{
    public InputField CurrentInputField { get; private set; }
    [SerializeField]
    GameObject keyboardPanel;
    [SerializeField]
    GameObject letterKeyboard;
    [SerializeField]
    GameObject letterUpperKeyboard;
    [SerializeField]
    GameObject numberKeyboard;
    [SerializeField]
    GameObject specialKeyboard;

    public void SetInputField(InputField inputField)
    {
        CurrentInputField = inputField;
        ShowKeyboardPanel(true);
    }

    public void InputCharacter(string character)
    {
        if (letterUpperKeyboard.activeSelf)
            CurrentInputField.text += character.ToUpper();
        else
            CurrentInputField.text += character;
    }

    public void RemoveCharacter()
    {
        CurrentInputField.text = CurrentInputField.text.Substring(0, CurrentInputField.text.Length - 1);
    }

    public void ChangeToLetter()
    {
        letterKeyboard.SetActive(true);
        letterUpperKeyboard.SetActive(false);
        numberKeyboard.SetActive(false);
        specialKeyboard.SetActive(false);
    }

    public void ChangeToLetterUpper()
    {
        letterKeyboard.SetActive(false);
        letterUpperKeyboard.SetActive(true);
        numberKeyboard.SetActive(false);
        specialKeyboard.SetActive(false);
    }

    public void ChangeToNumber()
    {
        letterKeyboard.SetActive(false);
        letterUpperKeyboard.SetActive(false);
        numberKeyboard.SetActive(true);
        specialKeyboard.SetActive(false);
    }

    public void ChangeToSpecial()
    {
        letterKeyboard.SetActive(false);
        letterUpperKeyboard.SetActive(false);
        numberKeyboard.SetActive(false);
        specialKeyboard.SetActive(true);
    }

    public void ShowKeyboardPanel(bool open)
    {
        keyboardPanel.SetActive(open);
    }
}