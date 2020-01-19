# Changelog & Version History

## Version 1.2 (2019-09-04)
* Don't download images whose hash is already seen. This prevents the downloading duplicate images from inactive grabbers.
* Default save file is now "grabber.txt" (not "graber.txt").
* If "grabber.txt" exists in the same folder as the EXE, it is loaded when the program starts.

## Version 1.1 (2019-09-03)
* Ignore exceptions during the download sequence. This was done so to prevent a hard crash if the user temporarily loses internet or the server doesn't respond.

## Version 1.0 (2019-09-02)
* Initial Release (fully functional)