#!/bin/bash
# buildEffects
# Compiles all .fx files found in the project's Content directory.
# Intended for usage with VS Code Build Tasks tooling.
# You may need to change the path to fxc.exe depending on your installation.

printf "Starting build process...\n"

cd project_name
for file in `find ./Content/** -name "*.fx"` ;
do
    # Hush, wine...
    export WINEDEBUG=fixme-all,err-all

    # Build the effect
    wine64 ~/.wine/drive_c/Program\ Files\ \(x86\)/Microsoft\ DirectX\ SDK\ \(June\ 2010\)/Utilities/bin/x64/fxc.exe\
     /T fx_2_0 $file /Fo "`dirname $file`/`basename $file .fx`.fxb"

    echo ""

done
