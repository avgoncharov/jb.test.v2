# jb.test.v2
The application is a truncated version of the Nuget feed with web interface.  
The “nuget push” and “nuge install” commands are supported.    
It runs over Nuget API v3 protocol.  
The application allows to review data of uploaded packages and execute search with id, version and description through the web interface.    

Repository synchronization with file system runs over at the application startup, restart:   
* All packages from input folder (so far it's ~/Pkgs/Input) are saved in the system and deleted from input folder  
* Packages that were registered in the system but deleted from output folder (so far it's ~/Pkgs/Output) are deleted from the system  

##### Deploying and configuration
At deploying to IIS it is necessary to specify connection string to MSSQL database in which packages information will be saved to.  
