# jb.test.v2
The application is a truncated version of the Nuget feed with web interface.  
The “nuget push” and “nuge install” commands are supported.    
It runs over Nuget API v3 protocol.  
The application allows to review data of uploaded packages and execute search with id, version and description through the web interface.    

Repository synchronization with file system runs over at the application startup, restart:   
* All packages from input folder (for now it's ~/Pkgs/Input) are saved in system and deleted from input folder  
* Packages that were registered in system but deleted from output folder (for now it's ~/Pkgs/Output) are delete from the system  

##### Deploying and configuration
At deploying to IIS it is necessary to specify connection string to MSSQL database in which packages information will be saved.  
