# Content Manager
Provides Services to Manage Text Content stored in Windows Azure Table Storage to check Correctness and Manage Translations

## Description
This app was built to solve the need of managing resources associated with an application.  The first (current) version supports resourcess added to an RESX file.  Future versions will include support for translation resource created as part of an Angular application as well as image resources.

The PowerShell Script can be added to your build process, you will need to provide the name of the Azure Storage Account as well as the Storage Key.  The script will then process the contents of the RESX files and syncronize them with values stored in Table Storage.

Once the values have been updated, they can be reviewed and edited within this application.  A future version will also support managing translations.