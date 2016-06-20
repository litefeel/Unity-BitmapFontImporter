#! /bin/sh
# exit this script if any commmand fails
set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "Get Unity version for $UNITY_VERSION"
url=$(python $DIR/unitydownloadurl.py -v $UNITY_VERSION --os mac)
realVersion=${url/*Unity-/}
realVersion=${realVersion/.pkg*/}
echo "Got Unity version:$realVersion"
echo "Got Unity url:$url"

echo "Downloading from $url: "
curl -o Unity.pkg "$url"

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /
