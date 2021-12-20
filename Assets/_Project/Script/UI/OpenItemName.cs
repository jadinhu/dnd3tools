using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenItemName : MonoBehaviour {

	public Image itemImageHolder;					//Image from the placeholder
	public Text itemNameHolder, itemNumberHolder;	//Text Name and Number from the placeholder

	/// <summary>
	/// Set the Item Name and Number on the placeholder
	/// </summary>
	/// <param name="itemParent">Item parent.</param>
	public void ButtonOpenItemName(GameObject itemParent){
		itemNameHolder.text = itemParent.name.Replace ("(Clone)", "");
		itemNumberHolder.text = itemParent.GetComponentInChildren<Text> ().text;
	}

	/// <summary>
	/// Set Item Image on the placeholder
	/// </summary>
	/// <param name="imageSelected">Image selected.</param>
	public void ButtonSetImage(Image imageSelected){
		itemImageHolder.sprite = imageSelected.sprite;
	}

}
