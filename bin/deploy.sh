#!/bin/bash

# Find a file in a parent directory
function upfind() 
{
    pushd $PWD > /dev/null
    while [[ $PWD != / ]]; 
    do
        find "$PWD" -maxdepth 1 -name "$1"
        cd ..
    done
    popd > /dev/null
}

APPNAME=Tson
LAPPNAME=$(echo $APPNAME | tr [:upper:] [:lower:])
SCRIPTNAME=$(basename $0)
SSH_CONFIG_NAME=$1

if [[ -z "$SSH_CONFIG_NAME" ]]; then 
    echo $SCRIPTNAME '$SSH_CONFIG_NAME'
    exit 1
fi

SLNDIR=$(dirname $(upfind $APPNAME.sln))
SCRATCHDIR=$SLNDIR/Scratch

if [[ ! -d $SCRATCHDIR ]]; then mkdir $SCRATCHDIR; fi

# Increment versions
$(dirname $0)/bumper.sh
if [[ $? -ne 0 ]]; then exit 1; fi

# Read in version
VERSION=$(cat Scratch/$APPNAME.version.txt)
VERSION=v$(expr $VERSION : '\([0-9]*\.[0-9]*\)')
DASH_VERSION=${VERSION/\./-}

# Delete the Release build directories
rm -rf $SLNDIR/${APPNAME}Service/bin/Release

# Do a release build of the service
bash -c "cd $SLNDIR; xbuild /property:Configuration=Release /property:Platform='Any CPU' ${APPNAME}.sln"
if [[ $? -ne 0 ]]; then exit 1; fi

# Do a release build of the web site
bash -c "cd Website; gulp clean; gulp --release"
if [[ $? -ne 0 ]]; then exit 1; fi

# Stop the services
echo Stopping remote service...
ssh $SSH_CONFIG_NAME "sudo service ${LAPPNAME}-${DASH_VERSION} stop"

# Create bin & lib directories
ssh $SSH_CONFIG_NAME "if [[ ! -d bin ]]; then mkdir -p bin; fi"
ssh $SSH_CONFIG_NAME "if [[ ! -d lib/${APPNAME}.${VERSION} ]]; then mkdir -p lib/${APPNAME}.${VERSION}; fi"

# Synchronize the application directories
rsync -rtvzp --delete --progress --rsh=ssh --exclude='logs' ${SLNDIR}/${APPNAME}Service/bin/Release/* ubuntu@${SSH_CONFIG_NAME}:lib/${APPNAME}.${VERSION}

# Create new symbolic links
ssh $SSH_CONFIG_NAME "find ~/lib/${APPNAME}.${VERSION}/Scripts -name \*.sh | while read -r FILENAME; do ln -sf \$FILENAME ~/bin/\$(basename \$FILENAME); done"

# Start the services
ssh $SSH_CONFIG_NAME "sudo service ${LAPPNAME}-${DASH_VERSION} start"

# Update S3 website
aws s3 rm s3://tsonspec.org/ --recursive --region us-east-1 --profile jamoki
aws s3 cp Website/build/ s3://tsonspec.org/ --region us-east-1 --profile jamoki --recursive --acl public-read

