#! /bin/sh
# exit this script if any commmand fails
set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "Get Unity version for $UNITY_VERSION"
url=$(python $DIR/unitydownloadurl.py -v $UNITY_VERSION --os mac)
realVersion=${url/*-/}
realVersion=${realVersion/.pkg*/}
realVersion=${realVersion/.dmg*/}
echo "Got Unity version:$realVersion"
echo "Got Unity url:$url"

dmg=${url/*.dmg*/}
pkgname="Unity.pkg"
echo "Downloading from $url: "
if [ -z $dmg ]; then
    curl -o Unity.dmg "$url"
    sudo hdiutil attach Unity.dmg
    ls /Volumes/Unity\ Download\ Assistant
    pkgname="/Volumes/Unity Download Assistant/Unity.pkg"
else
    curl -o Unity.pkg "$url"
fi

echo 'Installing Unity.pkg'
sudo installer -dumplog -package $pkgname -target /
