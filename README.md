# QRSS Plus Downloader
The **QRSS Plus Downloader** is a Windows application which makes it easy to automatically download QRSS grabber images from the [QRSS Plus website](http://swharden.com/qrss/plus/).

![](/doc/screenshot.jpg)

### Features
* simple click-to-run Windows application (nothing to configure)
* downloaded files are organized info folders according to callsign
* automatic mode downloads new grabs every every 10 minutes
* you can select which callsigns to download

### Download
* Click-to-run EXE: [QrssPlusDownloader-v1.2.zip](https://raw.githubusercontent.com/swharden/QRSSplus-Downloader/master/download/QrssPlusDownloader-v1.2.zip)
* Source code is in [/src](/src/)

### Changelog & Version History

#### Version 1.2 (2019-09-04)
* Don't download images whose hash is already seen. This prevents the downloading duplicate images from inactive grabbers.
* Default save file is now "grabber.txt" (not "graber.txt").
* If "grabber.txt" exists in the same folder as the EXE, it is loaded when the program starts.

#### Version 1.1 (2019-09-03)
* Ignore exceptions during the download sequence. This was done so to prevent a hard crash if the user temporarily loses internet or the server doesn't respond.

#### Version 1.0 (2019-09-02)
* Initial Release (fully functional)