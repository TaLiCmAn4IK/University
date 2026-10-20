// 11-Dec-03  K-01-21  $$1  TWH  Created
// 18-Feb-04  K-01-23  $$2  TWH  Handle blank substitutions
// 19-Apr-04  K-01-26+ $$3  TWH  Chg w/ product interaction env var chg

if (! $?PTC_APPMGR ) then
  if ("__PTC_APPMGR_DIR__" != "" ) then
    setenv PTC_APPMGR_DIR __PTC_APPMGR_DIR__
    if ($PTC_APPMGR_DIR == "__!!PTC_APPMGR_DIR!!__") then
      unsetenv PTC_APPMGR_DIR
    endif
    if ($?PTC_APPMGR_DIR) then
      setenv PTC_APPMGR $PTC_APPMGR_DIR/bin/proappmgr
    endif
  endif
endif
