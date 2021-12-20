using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour {

	 //Use this script to Search, Load and Save Images

	public RawImage downloadingImage;
	public Text downloadingText;

	//Private Stuff
	private string finalPath;


	void Start(){
		//Create the Directory
		DirectoryInfo dirInfo = new DirectoryInfo (Application.persistentDataPath + "/" + "PoiImages");
		if(!dirInfo.Exists)
			Directory.CreateDirectory (Application.persistentDataPath + "/" + "PoiImages");

		//Set the Final
		finalPath = Application.persistentDataPath+ "/" + "PoiImages" + "/";
		downloadingText = downloadingImage.gameObject.GetComponentInChildren<Text> ();
	}

	/// <summary>
	/// Try to load a specific image
	/// </summary>
	/// <returns>Texture Image.</returns>
	/// <param name="urlName">Url name of the image.</param>
	public Texture2D LoadImage(string urlName){
		FileInfo tempFile = new FileInfo (finalPath + urlName.Replace("/","").Replace(":",""));
		if (tempFile.Exists) {
			Texture2D texture = new Texture2D(0, 0, TextureFormat.RGB24, false);
			texture.LoadImage(File.ReadAllBytes(tempFile.FullName));
			return texture;
		}
		return null;
	}

	/// <summary>
	/// Saves a image locally.
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="urlName">Url name of the image.</param>
	public void SaveImageLocally(Texture2D texture, string urlName, string itemName){
		downloadingImage.texture = texture;
		downloadingText.text = ("Loading Item:\n" + itemName);

		FileInfo tempFile = new FileInfo (finalPath + urlName.Replace("/","").Replace(":",""));
		File.WriteAllBytes(tempFile.FullName, texture.EncodeToPNG());
	}

	/// <summary>
	/// Searchs the image.
	/// </summary>
	/// <returns><c>true</c> Image file exist, <c>false</c> image file doesn't exist.</returns>
	/// <param name="urlName">Url name of the image.</param>
	public bool SearchImage(string urlName){
		FileInfo tempFile = new FileInfo (finalPath + urlName.Replace("/","").Replace(":",""));
		if (tempFile.Exists)
			return true;
		else
			return false;
	}

}
