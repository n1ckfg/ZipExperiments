using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON; 
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class ZipTest : MonoBehaviour {

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
		if (ext == ".latk" || ext == ".zip") {
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
            jsonNode = getJsonFromZip(www.bytes);
		} else {
			jsonNode = JSON.Parse(www.text);
		}

        Debug.Log(jsonNode["grease_pencil"][0]["layers"][0]["frames"][0]["strokes"][0]["color"]);
    }

    JSONNode getJsonFromZip(byte[] bytes) {
        // https://gist.github.com/r2d2rigo/2bd3a1cafcee8995374f

        MemoryStream fileStream = new MemoryStream(bytes, 0, bytes.Length);
        ZipFile zipFile = new ZipFile(fileStream);

        foreach (ZipEntry entry in zipFile) {
            if (Path.GetExtension(entry.Name).ToLower() == ".json") {
                Stream zippedStream = zipFile.GetInputStream(entry);
                StreamReader read = new StreamReader(zippedStream, true);
                string json = read.ReadToEnd();
                Debug.Log(json);
                return JSON.Parse(json);
            }
        }

        return null;
    }

    void saveStringAsZip(string s) {
        // TODO
    }

}
