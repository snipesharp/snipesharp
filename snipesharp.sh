#!/bin/bash
# This script allows you to run multiple snipesharp instances by just adding account credentials,
# an offset and optionally extra parameters for snipesharp to use, like --prename if the account
# on the current line is a prename account.
#
# The only requirement for using this script is to have snipesharp installed on the system. If
# you have snipesharp downloaded as an executable but have it not installed on the system, you
# can install it by cd-ing into the directory of the executable and running this command:
# ./snipesharp --install
#
# It may also be worth mentioning that this script only works on unix systems and not Windows.

# Edit this value
export SPREAD=76

snipe() {
    PASS=$(printf "%q" "$3")
    screen -S ss-pop$1 -dm
    screen -S ss-pop$1 -X stuff "snipesharp --name=p --spread=$SPREAD --offset=$4 --pop-length=4 --pop-minsearches=120 --email=$2 --password=$PASS --debug $5 $6 $7 $8 $9 \n"
    sleep 25
    screen -S ss-3c$1 -dm
    screen -S ss-3c$1 -X stuff "snipesharp --name=3 --spread=$SPREAD --offset=$4 --email=$2 --password=$PASS --debug $5 $6 $7 $8 $9 \n"
}

gen_accs_txt() {
    echo -e "\x1b[0;31mYou must populate accs.txt with accounts you wish to use along with the offset to use for each and optionally extra arguments for snipesharp\x1b[0;37m"
    echo "An example accs.txt file has been written to $PWD/accs.txt"
    echo "# Hello,  lines  that  start  with   a  #  are  comments,  you  don't  have" >> accs.txt &&
    echo "# to    remove    them.    Insert    the    required    information   below" >> accs.txt &&
    echo "# separated    by     space(s)     and     run     snipesharp.sh     again." >> accs.txt &&
    echo "# " >> accs.txt &&
    echo "# " >> accs.txt &&
    echo "# EMAIL                   PASSWORD        OFFSET      EXTRA ARGS (OPTIONAL)" >> accs.txt &&
    echo "" >> accs.txt &&
    echo "example@email.com         password        57          --refresh-offset=12" >> accs.txt &&
    echo "example2@email.com        123pass!3       150         --prename" >> accs.txt &&
    exit
}

main() {
    [ -e accs.txt ] || gen_accs_txt

    # KILL EXISTING SNIPESHARP SCREENS
    for f in /run/screen*/S-$USER/*.ss*; do
        screen -X -S "${f##*/}" quit
    done
    sleep 1

    num=0
    grep -Ev '^#|^$' accs.txt | while read acc; do
        ((num++))
        snipe $num $acc
        sleep 25
    done
}

main
