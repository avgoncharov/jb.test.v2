# jb.test.v2
This is a truncated version of the Nuget feed.  
The “nuget push” and “nuge install” commands are supported.  
Works by using nuget api v3 protocol.  
Provides viewing data of pushed packages and searching by id, version and description through web interface.    

During the startup and restart process, synchronization between the storage and the file system is performed:  
* All packages from input folder will saved in system and remove from input folder  
* Packages for which there are no images in the output folder will be removed from the system  