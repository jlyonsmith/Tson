#!/bin/bash

SVCNAME=$1
SSHCONFIG=$2
APPCONFIG=$3
BUILDCONFIG=$4

if [[ -z "$BUILDCONFIG" ]]; then
    BUILDCONFIG=Release
fi

SCRIPTDIR=$(cd $(dirname $0); pwd -P)
SCRIPTNAME=$(basename $0)
SLNPATH=$(${SCRIPTDIR}/upfind.sh *.sln)
SLNDIR=$(dirname $SLNPATH)
SLNFILE=$(basename $SLNPATH)
APPNAME="${SLNFILE%.*}"
LAPPNAME=$(echo $APPNAME | tr [:upper:] [:lower:])
LSVCNAME=$(echo $SVCNAME | tr [:upper:] [:lower:])
SCRATCHDIR=$SLNDIR/Scratch
SVCPROJDIR=${SLNDIR}/${SVCNAME}/${SVCNAME}Service

if [[ -z "$SVCNAME" || -z "$SSHCONFIG" || -z "$APPCONFIG" ]]; then 
    echo "Usage: $SCRIPTNAME SVCNAME SSHCONFIG APPCONFIG BUILDCONFIG"
    echo "SVCNAME is Api"
    echo "SSHCONFIG is a name listed in the ~/.ssh/config file"
    if [[ -n "$SVCNAME" ]]; then
        echo "APPCONFIG's available for '${SVCNAME}':"
        find "${SVCPROJDIR}" -depth 1 -name app.\*.config -exec basename {} \; | sed 's/app.//;s/.config//'
    else
        echo "APPCONFIG's be discovered by giving SVCNAME"
    fi
    echo "BUILDCONFIG is Debug or Release (the default)"
    exit 1
fi

if [[ -z "$SLNPATH" ]]; then
    echo "Unable to find an .sln file in root directories"
    exit 1
fi

if [[ ! -d $SCRATCHDIR ]]; then mkdir $SCRATCHDIR; fi

# Read in version
SVCVERSION=v$(expr $(cat ${SLNDIR}/Scratch/$APPNAME.version.txt) : '\([0-9]*\)\.[0-9]*')
SVCVERSION=${SVCVERSION/\./-}
SVCLIBDIR=v$(expr $(cat ${SLNDIR}/Scratch/$APPNAME.version.txt) : '\([0-9]*\.[0-9]*\)')
SVCLIBDIR=lib/${APPNAME}.${SVCNAME}.${SVCLIBDIR}

# Delete the BUILDCONFIG build directories
rm -rf $SLNDIR/${APPNAME}Service/bin/${BUILDCONFIG}

# Do a release build, and stop if it fails
bash -c "cd $SLNDIR; xbuild /property:Configuration=${BUILDCONFIG} ${APPNAME}.sln"
if [[ $? -ne 0 ]]; then exit 1; fi

# Do a release build of the web site
bash -c "cd $SLNDIR/Website; gulp clean; gulp --minify --config=release"
if [[ $? -ne 0 ]]; then exit 1; fi

# TODO: Enable when/if there are unit tests
#Run the service interface unit tests
# bash -c "mono ${SLNDIR}/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe ${SLNDIR}/Tests/${SVCNAME}.ServiceInterface.Tests/bin/${BUILDCONFIG}/${SVCNAME}.ServiceInterface.Tests.dll"
# if [[ $? -ne 0 ]]; then exit 1; fi

# TODO: Run the service feature tests (locally)

# Stop the remote services
echo Stopping remote service...
ssh $SSHCONFIG "sudo service ${LAPPNAME}-${LSVCNAME}-${SVCVERSION} stop"

# Create remote bin & lib directories
ssh $SSHCONFIG "if [[ ! -d bin ]]; then mkdir -p bin; fi"
ssh $SSHCONFIG "if [[ ! -d ${SVCLIBDIR} ]]; then mkdir -p ${SVCLIBDIR}; fi"

# Synchronize the local and remote directories
rsync -rtvzp --delete --progress --rsh=ssh --exclude='logs' ${SVCPROJDIR}/bin/${BUILDCONFIG}/* ubuntu@${SSHCONFIG}:${SVCLIBDIR}

# Put correct app.config in place
ssh $SSHCONFIG "cd ${SVCLIBDIR}; cp app.${APPCONFIG}.config ${SVCNAME}Service.exe.config"
if [[ $? -ne 0 ]]; then exit 1; fi

# Create new symbolic links
ssh $SSHCONFIG "find ~/${SVCLIBDIR}/Scripts -name \*.sh | while read -r FILENAME; do ln -sf \$FILENAME ~/bin/\$(basename \$FILENAME); done"

# Start the service
ssh $SSHCONFIG "sudo service ${LAPPNAME}-${LSVCNAME}-${SVCVERSION} start"

# Update S3 website
aws s3 rm s3://tsonspec.org/ --recursive --region us-east-1 --profile jamoki
aws s3 cp $SLNDIR/Website/build/ s3://tsonspec.org/ --region us-east-1 --profile jamoki --recursive --acl public-read


