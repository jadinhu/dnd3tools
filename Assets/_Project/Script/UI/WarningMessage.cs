using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class WarningMessage{

	public static GameObject warning;
	public static Text messageText;
	public static Animator animator;
	public static Button button;

	public static void SendMessage(string message){
		SetVariables ();
		messageText.text = message;
		animator.SetTrigger ("Go");
	}

	public static void SendWelcomeMessage(string message){
		SetVariables ();
		button.gameObject.SetActive (true);
		messageText.text = message;
		animator.SetTrigger ("In");
	}

	static void SetVariables(){
		if (warning == null) {
			warning = GameObject.FindGameObjectWithTag ("Warning");
			messageText = warning.GetComponentInChildren<Text> ();
			animator = warning.GetComponent<Animator> ();
			button = warning.GetComponentInChildren<Button> ();
			button.gameObject.SetActive (false);
		}
	}
}
