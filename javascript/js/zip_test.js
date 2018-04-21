"use strict";

function main() {

    var url = "./untitled.zip"

    JSZipUtils.getBinaryContent(url, function(err, data) {
        if (err) {
            throw err; // or handle err
        }

        var zip = new JSZip();
        zip.loadAsync(data).then(function () {
            var fileNameOrig = url.split('\\').pop().split('/').pop();
            var fileName = fileNameOrig.split('.')[0] + ".json";
            zip.file(fileName).async("string").then(function(response) {
                console.log(response);
            });
        });
    });        

}

window.onload = main;