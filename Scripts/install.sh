#! /bin/sh
# exit this script if any commmand fails
set -e

echo "Downloading from $UNITY_DOWNLOAD_URL: "
curl -o Unity.pkg "$UNITY_DOWNLOAD_URL"

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /
