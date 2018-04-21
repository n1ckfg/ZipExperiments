# https://stackoverflow.com/questions/10908877/extracting-a-zipfile-to-memory
# https://stackoverflow.com/questions/11914472/stringio-in-python3
# https://docs.python.org/2/library/zipfile.html

import zipfile
from io import BytesIO

class InMemoryZip(object):

    def __init__(self):
        # Create the in-memory file-like object for working w/imz
        self.in_memory_zip = BytesIO()
        self.files = []

    # Just zip it, zip it
    def append(self, filename_in_zip, file_contents):
        # Appends a file with name filename_in_zip and contents of
        # file_contents to the in-memory zip.
        # Get a handle to the in-memory zip in append mode
        zf = zipfile.ZipFile(self.in_memory_zip, "a", zipfile.ZIP_DEFLATED, False)

        # Write the file to the in-memory zip
        zf.writestr(filename_in_zip, file_contents)

        # Mark the files as having been created on Windows so that
        # Unix permissions are not inferred as 0000
        for zfile in zf.filelist:
             zfile.create_system = 0         

        return self

    def readFromMemory(self):
        # Returns a string with the contents of the in-memory zip.
        self.in_memory_zip.seek(0)
        return self.in_memory_zip.read()

    def readFromDisk(self, url):
        zf = zipfile.ZipFile(url, 'r')
        for file in zf.infolist():
            self.files.append(zf.read(file.filename))

    # Zip it, zip it, zip it
    def writetofile(self, filename):
        # Writes the in-memory zip to a file.
        f = open(filename, "wb")
        f.write(self.readFromMemory())
        f.close()

if __name__ == "__main__":
# Run a test
    # write
    imz = InMemoryZip()
    imz.append("testfile.txt", "Make a test") #.append("testfile2.txt", "And another one")
    imz.writetofile("testfile.zip")

    # read
    imz.readFromDisk("testfile.zip")
    print(imz.files[0])
