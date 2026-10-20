// 13-Apr-98 $$1  JJE   Created.
#!/bin/csh
###
### Convert cdrs images to SGI rgb images
### for interactive viewing
###

echo "************************************************"
echo "Converting CDRS animation images into rgb format"
echo "************************************************"


###
### Rename animation files to conform to Kaleidoscope
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

##foreach FILE ($IMAGE_4)
##    echo $FILE
##    mv $FILE `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9][0-9][0-9]\)\(.imf\)/\1\2\3/'`
##end

###
### Convert CDRS .imf files into rgb files, not altering gamma correction
### in the data.
###

echo "--------------------------------------"
echo "Converting .imf files into .rgb files "
echo "--------------------------------------"

set IMAGES = `/bin/ls *.[0-9][0-9][0-9][0-9].imf`
foreach FILE ($IMAGES)
    echo $FILE
    convert_image -I CDRS $FILE -O RGB `echo $FILE | sed -e 's/\([a-zA-Z_0-9\-\.]*\)\([0-9][0-9][0-9][0-9]\)\.imf/\1\2\.rgb/'`
end

echo "-----------------------------------------------"
echo "Final .rgb files:"
ls *.rgb
echo "-----------------------------------------------"
