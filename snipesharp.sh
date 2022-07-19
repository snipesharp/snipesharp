#!/bin/bash
# This script allows you to run multiple snipesharp instances by just adding account credentials,
# an offset and optionally extra parameters for snipesharp to use, like --prename if the account
# on the current line is a prename account.
#
# The only requirement for using this script is to have snipesharp installed on the system. If
# you have snipesharp downloaded as an executable but have it not installed on the system, you
# can install it by cd-ing into the directory of the executable and running this command:
# sudo ./snipesharp_linux-x86-64-vx.x.x --install
#
# It may also be worth mentioning that this script only works on unix systems and not Windows.

# Edit this value
export SPREAD=76
export DIR=$(dirname -- "$0")

snipe() {
    PASS=$(printf "%q" "$3")
    echo "Starting popular name sniping as $2 in screen session 'ss-pop$1'"
    screen -S ss-pop$1 -dm
    screen -S ss-pop$1 -X stuff "snipesharp --name=p --spread=$SPREAD --offset=$4 --pop-length=4 --pop-minsearches=120 --email=$2 --password=$PASS --debug $5 $6 $7 $8 $9 \n"
    sleep 25
    echo "Starting 3 char name sniping as $2 in screen session 'ss-pop$1'"
    screen -S ss-3c$1 -dm
    screen -S ss-3c$1 -X stuff "snipesharp --name=3 --spread=$SPREAD --offset=$4 --email=$2 --password=$PASS --debug $5 $6 $7 $8 $9 \n"
}

gen_accs_txt() {
    echo -e "\x1b[0;31mYou must populate $DIR/accs.txt with accounts you wish to use along with the offset to use for each and optionally extra arguments for snipesharp\x1b[0;37m"
    echo "An example $DIR/accs.txt file has been written to $PWD/accs.txt"
    echo "# Hello,  lines  that  start  with   a  #  are  comments,  you  don't  have" >> $DIR/accs.txt &&
    echo "# to    remove    them.    Insert    the    required    information   below" >> $DIR/accs.txt &&
    echo "# separated    by     space(s)     and     run     snipesharp.sh     again." >> $DIR/accs.txt &&
    echo "# " >> $DIR/accs.txt &&
    echo "# " >> $DIR/accs.txt &&
    echo "# EMAIL                   PASSWORD        OFFSET      EXTRA ARGS (OPTIONAL)" >> $DIR/accs.txt &&
    echo "" >> $DIR/accs.txt &&
    echo "example@email.com         password        57          --refresh-offset=12" >> $DIR/accs.txt &&
    echo "example2@email.com        123pass!3       150         --prename" >> $DIR/accs.txt &&
    exit
}

killScreens() {
    echo -e "\x1b[0;90mKilling existing snipesharp screen sessions\x1b[0;37m"
    for f in /run/screen*/S-$USER/*.ss*; do
        screen -X -S "${f##*/}" quit
    done
}

noSnipesharp() {
    echo -e "\x1b[0;31mNo instance of snipesharp was found to be installed\x1b[0;37m"
    echo -e "\x1b[0;31mPlease install snipesharp to use this script\x1b[0;37m"
    echo -e "If you downloaded snipesharp as an executable, run it as a \x1b[0;31msuperuser\x1b[0;37m with the '--install' argument to install it"
    echo -e "Example: '\x1b[0;32msudo ./snipesharp_linux-x86-64-vx.x.x --install\x1b[0;37m'"
    exit
}

main() {
    [ -e /usr/bin/snipesharp ] || noSnipesharp
    [ -e $DIR/accs.txt ] || gen_accs_txt

    # KILL EXISTING SNIPESHARP SCREENS
    [ -z "$(ls /run/screen*/S-$USER/)" ] || killScreens
    sleep 1

    num=0
    grep -Ev '^#|^$' $DIR/accs.txt | while read acc; do
        ((num++))
        snipe $num $acc
        sleep 25
    done

    echo -e "\x1b[0;32mDone. View the launched screen sessions with 'screen -ls'\x1b[0;37m"
}

main
