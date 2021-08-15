# HTTPWinTimeSync
Windows Service that will sync your machine with the time from one or more webservers

## Usage
1. Download the MSI
1. Install the MSI on one or more machines
1. Edit the urls.txt in the install directory - Ex: C:\Program Files\maltob\HTTPWinTimeSync
1. The service will automatically start after a reboot or if you start it manually through services/sc/Start-Service

## FAQ
#### Why would I use this?
 If you have NTP or another time sync method, you wouldn't. 
 This allows you to sync time directly with webservers instead of NTP for areas like cafe's where HTTP and HTTPS might be the only allowed protocols.

#### Should I run this on my servers/systems?
It hasn't been thoroughly tested, I wouldn't recommend that.

#### How accurate will it keep my time?
It only aims to keep within 15 seconds of the server times. This isn't currently adjustable.

#### Is this related to htpdate?
No, that project has higher accuracy but is *NIX only.

#### How often does it sync?
 * Initially on service start it syncs
 * Then it will wait 2 hours, 4 hours, 8 hours, 16 hours, 32 hours; Once it reaches 32 hours it will continue syncing every 32 hours.
