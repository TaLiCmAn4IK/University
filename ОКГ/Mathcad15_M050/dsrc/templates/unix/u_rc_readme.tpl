// 06-Aug-98  H-03-51+ TWH   $$1  Created
// 24-Sep-98  H-03-54+ TWH   $$2  chg flex dir to install dir
// 05-Dec-02  J-03-39  ECW   $$3  update for j03 platforms
//==================================================================
AUTOMATIC FLEXLM SERVER STARTUP CONFIGURATION FOR UNIX SYSTEMS
--------------------------------------------------------------

If you installed a PTC server process as a "non-root" user, your 
system was not configured to automatically restart the server upon 
reboot since root access is required for this.

Your system can be configured to start the server automatically upon
reboot, by creating or updating a system boot directory or file with 
a FLEXlm boot script.  This script is executed when the machine is
booted.


Follow the directions below logged-in as a user with root privilages.

1. For Sun, HP, SGI and Linux platforms, copy the file 
   specific to your platform from the <flexlm_installation_dir>/startup
   directory into the appropriate system directory on your server.

   Hardware Platform     File name	System Directory      
   --------------------------------------------------------
   Sun                  S99flexlm	/etc/rc2.d/          
   Linux                S99flexlm	/etc/rc5.d/         
   SGI          	S99flexlm	/etc/rc2.d/          
   HP                   S999flexlm	/sbin/rc2.d/         

Your machine should now restart the FLEXlm license server automatically
upon reboot as the installed user.

