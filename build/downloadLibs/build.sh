#!/usr/bin/env bash

# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
TEMP_DIR=$TOOLS_DIR/build
TEMP_PROJECT=$TEMP_DIR/build.csproj

# Define default arguments.
SCRIPT="build.cake"
CAKE_VERSION="0.37.0"
CAKE_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -s|--script) SCRIPT="$2"; shift ;;
        --cake-version) CAKE_VERSION="$2"; shift ;;
        --) shift; CAKE_ARGUMENTS+=("$@"); break ;;
        *) CAKE_ARGUMENTS+=("$1") ;;
    esac
    shift
done

CAKE_PATH="$TOOLS_DIR/cake.coreclr/$CAKE_VERSION/Cake.dll"

if [ ! -f "$CAKE_PATH" ]; then
    echo "Restoring Cake..."

    # Make sure the tools folder exists
    if [ ! -d "$TOOLS_DIR" ]; then
        mkdir "$TOOLS_DIR"
    fi

    # Build the temp project and restore Cake
    dotnet new classlib -o "$TEMP_DIR" --no-restore
    dotnet add "$TEMP_PROJECT" package Cake.CoreCLR --package-directory "$TOOLS_DIR" --version "$CAKE_VERSION"

    rm -rf "$TEMP_DIR"
fi


echo "> Running: exec dotnet $CAKE_PATH $SCRIPT ${CAKE_ARGUMENTS[@]}"
# Start Cake
exec dotnet "$CAKE_PATH" "$SCRIPT" "${CAKE_ARGUMENTS[@]}"
