# DisqusThread
A web part for [Kentico](https://www.kentico.com) that adds [Disqus Engage](https://disqus.com/) platform to your web site.

## Installation
 1. Download the latest package from [Release](https://github.com/Kentico/DisqusThread/releases)
 2. In Kentico, go to the Sites application
 3. Select "Import sites or objects"
 4. Upload the package and import it
 5. Now you are ready to use it in the Pages application
 
## Contributing
  1. Read the [contribution guidelines](https://github.com/Kentico/DisqusThread/blob/master/CONTRIBUTING.md)
  2. Enable the [continuous integration](https://docs.kentico.com/display/K9/Setting+up+continuous+integration) module
  3. Serialize all objects to disk
  4. Open a command prompt
  5. Navigate to the root of your project (where the .sln file is)
  6. Init a git repo and fetch the web part
  
        git init
        git remote add origin https://github.com/Kentico/DisqusThread.git
        git fetch
        git checkout -t origin/master

  7. Restore DB data
  
        Kentico\CMS\bin\ContinuousIntegration.exe -r

  8. You are ready to start making changes
  
## Compatibility
Tested with Kentico 8.1, 8.2, 9.0.

## [Questions & Support](https://github.com/Kentico/Home/blob/master/README.md)

## [License](https://github.com/Kentico/DisqusThread/blob/master/LICENSE.txt)
  
