// 30-Nov-98 $$1  TWH  Created.
// 28-Jan-99 $$2  TWH  Fixed temp_installed list
// -----------------------------------
// expects valid_catia_mc wordlist set
//
set temp_installed_mc = (`ls $prodir/`)
set installed_mc

foreach dir ($temp_installed_mc)
  foreach mc_list ($valid_catia_mc)
     if ("$dir" == "$mc_list") set installed_mc = ($installed_mc $dir)
     if ("$dir" == "$mc") set good_mc
  end
end

if (! $?good_mc) then
  echo "ERROR: Valid architectures are: "
  foreach vmc ($valid_catia_mc)
    echo "	${vmc}"
  end
  echo "You have the following valid architectures are installed:"
  if ("$installed_mc" == "") then
    echo "	No valid architectures installed"
  else
    foreach imc ($installed_mc)
      echo "	${imc}"
    end
  endif
  echo ""
  echo "Your machine type is $mc which is not in this list. Exiting..."
  exit(1)
endif
