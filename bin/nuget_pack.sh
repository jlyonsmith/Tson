#!/bin/bash
function upfind() {
    pushd $PWD > /dev/null
    while [[ $PWD != / ]] ; do
        find "$PWD" -maxdepth 1 -name "$1"
        cd ..
    done
    popd > /dev/null
}

APPNAME=Tson
SLNDIR=$(dirname $(upfind $APPNAME.sln))
pushd $SLNDIR
rm *.nupkg
mono ~/lib/NuGet/NuGet.exe pack TsonLibrary.nuspec
popd

