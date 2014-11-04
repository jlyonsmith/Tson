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
find . -name \*.nupkg -maxdepth 1 | xargs -n 1 mono ~/lib/NuGet/NuGet.exe push
popd
