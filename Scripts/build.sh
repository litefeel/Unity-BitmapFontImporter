#! /bin/sh
# exit this script if any commmand fails


echo "Run test "
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -runEditorTests \
  -quit

code=$?
cat $(pwd)/unity.log
if [ "$code" != "0" ]; then
  exit $code
fi


# project="ci-build"

# dobuild $plantform $buildcmd $outputpath
# dobuild() {
#   rm -rf "$(pwd)/unity.log"
#   echo "Attempting to build $project for $1"
#   set +e
#   /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#     -batchmode \
#     -nographics \
#     -silent-crashes \
#     -logFile $(pwd)/unity.log \
#     -projectPath $(pwd) \
#     -"$2" "$3" \
#     -quit


#   code=$?
#   cat "$(pwd)/unity.log"
#   if [ "$code" != "0" ]; then
#     echo "Build fail for $1"
#     # exit script without bash
#     # kill -"$code" $$
#     exit
#   fi
# }

# dobuild "Windows" buildWindowsPlayer        "$(pwd)/Build/windows/$project.exe"
# dobuild "0S X"    buildOSXUniversalPlayer   "$(pwd)/Build/osx/$project.app"
# dobuild "Linux"   buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe"




# function doWindows()
#   echo "Attempting to build $project for $2"
#   /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#     -batchmode \
#     -nographics \
#     -silent-crashes \
#     -logFile $(pwd)/unity.log \
#     -projectPath $(pwd) \
#     -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
#     -quit

# if [[ $? ]]; then
#   #statements
# fi
# cat "$(pwd)/unity.log"
# rm -rf "$(pwd)/unity.log"

# echo "Attempting to build $project for OS X"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#   -batchmode \
#   -nographics \
#   -silent-crashes \
#   -logFile $(pwd)/unity.log \
#   -projectPath $(pwd) \
#   -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
#   -quit

# cat "$(pwd)/unity.log"
# rm -rf "$(pwd)/unity.log"

# echo "Attempting to build $project for Linux"
# /Applications/Unity/Unity.app/Contents/MacOS/Unity \
#   -batchmode \
#   -nographics \
#   -silent-crashes \
#   -logFile $(pwd)/unity.log \
#   -projectPath $(pwd) \
#   -buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe" \
#   -quit

# cat "$(pwd)/unity.log"
# rm -rf "$(pwd)/unity.log"
