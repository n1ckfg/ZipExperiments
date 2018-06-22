using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON; 
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class TestLoader : MonoBehaviour {

	public string readFileName;

	private float consoleUpdateInterval = 0f;
	private JSONNode jsonNode;
	private bool useZip = false;

	private void Start() {
		StartCoroutine(readBrushStrokes());
	}

	public IEnumerator readBrushStrokes() {
		Debug.Log ("*** Begin reading...");

		string ext = Path.GetExtension(readFileName).ToLower();
		Debug.Log("Found extension " + ext);
		if (ext == ".latk") {
			useZip = true;
		} else {
			useZip = false;
		}

		string url;

		#if UNITY_ANDROID
		url = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", readFileName);
		#endif

		#if UNITY_IOS
		url = Path.Combine("file://" + Application.dataPath + "/Raw", readFileName);
		#endif

		#if UNITY_EDITOR
		url = Path.Combine("file://" + Application.dataPath, readFileName);		
		#endif 

		#if UNITY_STANDALONE_WIN
		url = Path.Combine("file://" + Application.dataPath, readFileName);		
		#endif 

		#if UNITY_STANDALONE_OSX
		url = Path.Combine("file://" + Application.dataPath, readFileName);		
		#endif 

		WWW www = new WWW(url);
		yield return www;

		Debug.Log ("+++ File reading finished. Begin parsing...");
		yield return new WaitForSeconds (consoleUpdateInterval);

		if (useZip) {
			MemoryStream streamCompressed = new MemoryStream(www.bytes);
			ZipInputStream streamUncompressed = new ZipInputStream(streamCompressed);
			string json = streamUncompressed.ToString();
			Debug.Log(json);
			jsonNode = JSON.Parse(json);
		} else {
			jsonNode = JSON.Parse(www.text);
		}

		Debug.Log(jsonNode["grease_pencil"][0]["layers"][0]["frames"][0]["strokes"][0]["color"]);
	}

}
