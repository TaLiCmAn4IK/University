// 18-Jul-03  K-01-13 SVN   $$1  Created
//
//==================================================================
AUTOMATIC SERVER STARTUP CONFIGURATION FOR UNIX SYSTEMS
--------------------------------------------------------------

If you installed a PTC server process as a "non-root" user, your 
system was not configured to automatically restart the server upon 
reboot since root access is required for this.

Your system can be configured to start the server automatically upon
reboot, by creating or updating a system boot directory or file with 
the boot script.  This script is executed when the machine is
booted.


Follow the directions below logged-in as a user with root privilages.

1. For Sun, SGI, HP, and DEC Alpha Unix platforms, copy the file 
   specific to your platform from the <installation_dir>/startup
   directory into the appropriate system directory on your server.

   Hardware Platform     File name	System Directory      
   --------------------------------------------------------
   sun4_solaris         S99<name>       /etc/rc3.d/
   sgi_elf2            	S99<name>	/etc/rc2.d/          
   hp8k                 S999<name>	/sbin/rc2.d/         
   alpha_unix           S99<name>	/sbin/rc3.d/         


2. For IBM unix systems, please run the "ibm_rcptc" c-shell script
   located in the <installation_dir>/startup directory.  This
   will make a modification to your /etc/initab file.  You will also
   need to copy the rc.ptc file from <installation_dir>/startup
   directory to your /etc directory.


Your machine should now restart the server automatically upon reboot 
as the installed user.

