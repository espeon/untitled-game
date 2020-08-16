#!/bin/bash
# buildEffects
# Compiles all .fx files found in the project's Content directory.
# Intended for usage with VS Code Build Tasks tooling.
# You may need to change the path to fxc.exe depending on your installation.

printf "Starting T4 processing...\n"

cd project_name

# create our output directory
mkdir -p T4Templates/Output

for file in `find ./T4Templates/** -name "*.tt"` ;
do
    # Build the template
    t4 -r=System.dll -r=mscorlib.dll -r=netstandard.dll -r=System.IO.FileSystem.dll -r=System.Linq.dll -r=System.Text.RegularExpressions.dll `dirname $file`/`basename $file` -o `dirname $file`/Output/`basename $file .tt`.cs

    echo "Built `basename $file`"

done
