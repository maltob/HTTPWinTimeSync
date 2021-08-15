# HTTPWinTimeSync
Windows Service that will sync your machine with the time from one or more webservers

## Usage
1. Download the MSI
1. Create a urls.txt in the same directory as the MSI with a URL per line. This will overwrite the default urls.txt.
1. Install the MSI on one or more machines
1. The service will automatically start after a reboot or if you start it manually through services/sc/Start-Service
