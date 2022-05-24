TEMP_FOLDER=$HOME/$RANDOM$RANDOM$RANDOM-TEMP
ARCH=$(uname -m)
FAIL=FALSE

mkdir -p $TEMP_FOLDER

function installLatest() {
    echo -n "Finding latest snipesharp version."
    wget -q 'https://github.com/snipesharp/snipesharp/releases/latest' -O $TEMP_FOLDER/latest || FAIL=TRUE
    echo -n "."

    [ $FAIL = TRUE ] && echo -e "\nFailed to find latest version, check your internet connection" && exit 1

    [ $(uname -m) = 'x86_64' ] &&
        LINK=https://github.com$(grep -o '/snipe.*linux-x86-64-v[0-9]\.[0-9]\.[0-9]' $TEMP_FOLDER/latest)

    [ $(uname -m) = 'arm64' ] &&
        LINK=https://github.com$(grep -o '/snipe.*linux-arm64-v[0-9]\.[0-9]\.[0-9]' $TEMP_FOLDER/latest)
    echo "."

    echo -n "Downloading snipesharp $(grep -o 'v[0-9]\.[0-9]\.[0-9]' $TEMP_FOLDER/latest | head -n 1)."
    wget -q $LINK -O $TEMP_FOLDER/snipesharp && echo -n "."
    chmod +x $TEMP_FOLDER/snipesharp &&         echo "."
    sudo bash -c "exec '$TEMP_FOLDER/snipesharp' '--install'"
}

installLatest
rm -rf $TEMP_FOLDER
