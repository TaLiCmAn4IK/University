// 17-Jan-01 $$1  TWH  Created.
// 06-Feb-01 $$2  TWH  fix mc check
// 25-OCT-05 K-03-34 $$3  MTP  sun_solaris_x64 support
//
if ($mc =~ sun*) then
    if (! $?TMPDIR) then
        setenv TMPDIR /tmp
    endif
endif

