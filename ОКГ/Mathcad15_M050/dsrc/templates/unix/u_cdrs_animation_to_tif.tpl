// 13-Apr-98 $$1  JJE   Created.
#!/bin/csh
###
### Prepare cdrs images for mailing to Kaleidoscope Animation for
### recording on video tape
###

echo "********************************************"
echo "Preparing images for recording on video tape"
echo "********************************************"


###
### Rename animation (.imf) files to conform to Kaleidoscope
### naming conventions.
### Rename  file.nn.imf to file.00nn.imf
### (to have 4 digits of frame number)
###
echo "-------------------"
echo "Renaming .imf files"
echo "-------------------"

set IMAGE_1 = `/bin/ls *.[0-9].imf`
set IMAGE_2 = `/bin/ls *.[0-9][0-9].imf`
set IMAGE_3 = `/bin/ls *.[0-9][0-9][0-9].imf`
set IMAGE_4 = `/bin/ls *.[0-9][0-9][0-9][0-9].imf`

foreach FILE ($IMAGE_1)
    echo $FILE
    mv $FILE `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9]\)\(.imf\)/\1000\2\3/'`
end
foreach FILE ($IMAGE_2)
    echo $FILE
    mv $FILE `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9]\)\(.imf\)/\100\2\3/'`
end
foreach FILE ($IMAGE_3)
    echo $FILE
    mv $FILE `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9][0-9]\)\(.imf\)/\10\2\3/'`
end

##
##foreach FILE ($IMAGE_4)
##    echo $FILE
##    mv $FILE `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9][0-9][0-9]\)\(.imf\)/\1\2\3/'`
##end

###
### To convert CDRS .imf files into Kaleidoscope .tif files for
### processing, laying to tape as animation.
### Notes: 1) These images are assumed to be pre-processed to use
###           4 digits in each filename (done above in this script)
###        2) This version keeps the images in gamma corrected space.

echo "---------------------------------------------------------------------"
echo "Converting .imf files into .tif files (assumed to be of size 646x485)"
echo "---------------------------------------------------------------------"

set IMAGES = `/bin/ls *.[0-9][0-9][0-9][0-9].imf`
foreach FILE ($IMAGES)
    echo $FILE
    convert_image -I CDRS $FILE -O TIFF `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9][0-9][0-9]\)\.imf/\1\2\.tif/'`
end

echo "-----------------------------------------------"
echo ""
echo 'Please use the "tar" command to copy .tif files'
echo 'to tape (1/4" or 4mm).  On ESV:'
echo "    tar cvf /dev/rmt/m4 *.tif"
echo ""
