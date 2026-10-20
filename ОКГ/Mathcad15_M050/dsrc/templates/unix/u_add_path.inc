// 17-Jan-05 J-01-68+     Bruno  Created.
// 24-Jan-05 K-03-18  $$1 MAZ    Submit to K-03
if ($?mc1) then
        set path = ( $path $prodir/$mc/obj $prodir/$mc/lib $prodir/$mc1/obj $prodir/$mc1/lib)
else
        set path = ( $path $prodir/$mc/obj $prodir/$mc/lib )
endif

