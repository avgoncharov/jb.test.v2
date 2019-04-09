# jb.test.v2
This is a truncated version of the Nuget feed with web interface.  
The “nuget push” and “nuge install” commands are supported.  
Works by using nuget api v3 protocol.  
Provides viewing data of pushed packages and searching by id, version and description through the web interface.    

During the startup and restart process, synchronization between the storage and the file system is performed:  
* All packages from input folder (for now it's ~/Pkgs/Input) will saved in system and remove from input folder  
* Packages for which there are no images in the output folder (for now it's ~/Pkgs/Output)  will be removed from the system  

### Deploying and configuration
When deploying to iis, you must change the connection string to the actual mssql server and database.
