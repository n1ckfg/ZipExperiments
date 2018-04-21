// https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class ZipTest : MonoBehaviour {

    public string url;

    private ZipOutputStream _zipOut;
    private byte[] _buffer = new byte[4096];

    private void Start() {
        url = Path.Combine(Application.streamingAssetsPath, url);
        char[] c = Encoding.UTF8.GetChars(unzipFileToMemory(url));
        foreach (char ch in c) {
            Debug.Log(ch);
        }
    }

    public byte[] unzipFileToMemory(string zipFileIn) { //, string password) {
        Stream inStream = File.OpenRead(zipFileIn);

        MemoryStream outputMemStream = new MemoryStream();
        _zipOut = new ZipOutputStream(outputMemStream);
        _zipOut.IsStreamOwner = false;  // False stops the Close also Closing the underlying stream.

        _zipOut.SetLevel(3);
        //_zipOut.Password = password;        // optional

        ZipFile zipFile = new ZipFile(inStream);
        zipFile.IsStreamOwner = false;

        foreach (ZipEntry zipEntry in zipFile) {
            if (!zipEntry.IsFile)
                continue;
            string entryFileName = zipEntry.Name; // or Path.GetFileName(zipEntry.Name) to omit folder
                                                  // Specify any other filtering here.

            Stream zipStream = zipFile.GetInputStream(zipEntry);

            ZipEntry newEntry = new ZipEntry(entryFileName);
            newEntry.DateTime = zipEntry.DateTime;
            newEntry.Size = zipEntry.Size;
            // Setting the Size will allow the zip to be unpacked by XP's built-in extractor and other older code.

            _zipOut.PutNextEntry(newEntry);

            StreamUtils.Copy(zipStream, _zipOut, _buffer);
            _zipOut.CloseEntry();
        }

        inStream.Close();

        // Must finish the ZipOutputStream to finalise output before using outputMemStream.
        _zipOut.Close();

        outputMemStream.Position = 0;

        // At this point the underlying output memory stream (outputMemStream) contains the zip.
        return outputMemStream.ToArray();
    }

    public void zipFileFromMemory(string zipFileIn) {

    }

}

 