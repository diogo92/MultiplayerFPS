using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class Util {
	public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

	public static void SetLayerRecursively (GameObject _obj, int _newLayer){
		if (_obj == null)
			return;

		_obj.layer = _newLayer;

		foreach (Transform _child in _obj.transform) {
			if (_child == null)
				continue;
			SetLayerRecursively (_child.gameObject, _newLayer);
		}
	}

	public static List<Sprite> GetAvatars(){
		List<Sprite> returnVal = new List<Sprite> ();
		string[] filePaths = Directory.GetFiles (Application.dataPath + "/Resources/Avatars");
		Texture2D tex;
		byte[] fileData;
		for (int i = 0; i < filePaths.Length; i++) {
			if (ImageExtensions.Contains (Path.GetExtension (filePaths [i]).ToUpperInvariant ())) {
				fileData = File.ReadAllBytes (filePaths [i]);
				tex = new Texture2D (1, 1);
				tex.LoadImage (fileData);
				Sprite sprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100.0f);
				sprite.name = Path.GetFileNameWithoutExtension (filePaths [i]);
				returnVal.Add (sprite);
			}
		}
		return returnVal;
	}

	public static Sprite GetCurrentPlayerAvatar(string _imageName){
		string[] filePaths = Directory.GetFiles (Application.dataPath + "/Resources/Avatars");
		for (int i = 0; i < filePaths.Length; i++) {
			if (Path.GetFileNameWithoutExtension (filePaths [i]) == _imageName) {
				Debug.Log (_imageName);
				Texture2D tex;
				byte[] fileData;
				fileData = File.ReadAllBytes (filePaths [i]);
				tex = new Texture2D (1, 1);
				tex.LoadImage (fileData);
				return Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f), 100.0f);
			}
		}
		return null;

	}

	//public static Sprite Get

}
