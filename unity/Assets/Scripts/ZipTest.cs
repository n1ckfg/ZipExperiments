using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON; 
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class ZipTest : MonoBehaviour {

	public string readFileName;
    public string writeFileName;

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

        saveJsonAsZip(jsonNode.ToString());
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

    void saveJsonAsZip(string s) {
        // https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string
        // https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples
        // https://stackoverflow.com/questions/8624071/save-and-load-memorystream-to-from-a-file

        MemoryStream memStreamIn = new MemoryStream();
        StreamWriter writer = new StreamWriter(memStreamIn);
        writer.Write(s);
        writer.Flush();
        memStreamIn.Position = 0;

        MemoryStream outputMemStream = new MemoryStream();
        ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

        zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

        string writeFileNameMinusExtension = "";
        string[] nameTemp = writeFileName.Split('.');
        for (int i=0; i<nameTemp.Length-1; i++) {
            writeFileNameMinusExtension += nameTemp[i];
        }

        ZipEntry newEntry = new ZipEntry(writeFileNameMinusExtension + ".json");
        newEntry.DateTime = System.DateTime.Now;

        zipStream.PutNextEntry(newEntry);

        StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
        zipStream.CloseEntry();

        zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
        zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.

        outputMemStream.Position = 0;

        using (FileStream file = new FileStream(writeFileName, FileMode.Create, System.IO.FileAccess.Write)) {
            byte[] bytes = new byte[outputMemStream.Length];
            outputMemStream.Read(bytes, 0, (int)outputMemStream.Length);
            file.Write(bytes, 0, bytes.Length);
            outputMemStream.Close();
        }

        /*
        // Alternative outputs:
        // ToArray is the cleaner and easiest to use correctly with the penalty of duplicating allocated memory.
        byte[] byteArrayOut = outputMemStream.ToArray();

        // GetBuffer returns a raw buffer raw and so you need to account for the true length yourself.
        byte[] byteArrayOut = outputMemStream.GetBuffer();
        long len = outputMemStream.Length;
        */
    }

}
