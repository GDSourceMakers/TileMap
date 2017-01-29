#! /bin/sh

# Example build script for Unity3D project. See the entire example: https://github.com/JonathanPorta/ci-build

# Change this the name of your project. This will be the name of the final executables as well.
project="Tile-Map"
travisUnity="/Applications/Unity/Unity.app/Contents/MacOS/Unity"



unityPath="$travisUnity"

# csc /target:library /out:TileMap.DLL /Assets/TileMap/clipper.cs /Assets/TileMap/TileMap.cs

echo "Exporting Package"
"$unityPath" -batchmode -nographics -projectPath "$(pwd)" -exportPackage "Assets\TileMap" -logFile $(pwd)/unity.log 

echo 'Logs from build'
logFile="$(pwd)"/unity.log
echo cat "$logFile"