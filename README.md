<p align="center">
  <a href="https://snipesharp.xyz/">
    <img src="https://user-images.githubusercontent.com/93228501/155908335-803039d4-85bc-4407-9a59-5de88ec49d40.png" height="196" width="196">
  </a>
  <br/>
  
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/sha256sums.txt">
    <img src="https://img.shields.io/badge/sha256sums-%231a6eef?style=flat-square"</img>
  </a>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_linux-x86-64-v1.8.0">
    <img src="https://img.shields.io/badge/_linux-v1.8.0-%231a6eef?style=flat-square&logo=linux&logoWidth=20&logoColor=white"</img>
  </a>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_mac-os-x86-64-v1.8.0">
    <img src="https://img.shields.io/badge/_mac_os-v1.8.0-%231a6eef?style=flat-square&logo=apple&logoWidth=20&logoColor=white"</img>
  </a>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_win-x86-64-v1.8.0.exe">
    <img src="https://img.shields.io/badge/_windows-v1.8.0-%231a6eef?style=flat-square&logo=windows&logoWidth=20&logoColor=white"</img>
  </a>
  <br/>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_linux-arm64-v1.8.0">
    <img src="https://img.shields.io/badge/linux_arm64-v1.8.0-%23015fa1?style=flat-square&logo=linux&logoWidth=20&logoColor=white"</img>
  </a>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_mac-os-arm64-v1.8.0">
    <img src="https://img.shields.io/badge/mac_os_arm64-v1.8.0-%23015fa1?style=flat-square&logo=apple&logoWidth=20&logoColor=white"</img>
  </a>
  <a href="https://github.com/snipesharp/snipesharp/releases/download/v1.8.0/snipesharp_win-arm64-v1.8.0.exe">
    <img src="https://img.shields.io/badge/windows_arm64-v1.8.0-%23015fa1?style=flat-square&logo=windows&logoWidth=20&logoColor=white"</img>
  </a>
  <br>
  <a align="center" href="http://download.snipesharp.xyz/"><b>Not sure which to download?</b></a>
  <br><br>
  <a href="https://snipesharp.xyz/donate/">
    <img src="https://img.shields.io/badge/❤️_donate_❤️-%230b2a53?style=for-the-badge">
  </a>
  <br/><br/>
  <a href="https://snipesharp.xyz/discord">
    <img src="https://img.shields.io/discord/943483411597758494?color=567CFF&label=discord&logo=discord&logoColor=ffffff&style=for-the-badge">
  </a>
  <br/><br/>
  <a href="https://github.com/snipesharp/snipesharp/graphs/contributors"><img src="https://img.shields.io/github/contributors/snipesharp/snipesharp?style=flat&color=e17800&label=developers&logo=github"></a>
  <a href="https://github.com/snipesharp/snipesharp/releases"><img src="https://img.shields.io/github/downloads/snipesharp/snipesharp/total?color=1aaf19&logo=github&label=downloads&style=flat"></a>
  <a href="https://github.com/snipesharp/snipesharp/stargazers"><img src="https://img.shields.io/github/stars/snipesharp?color=d7b608&logo=github&style=flat"></a>
  <a href="https://github.com/snipesharp/snipesharp/issues"><img src="https://img.shields.io/github/issues/snipesharp/snipesharp?style=flat&color=0e3351&logo=github"></a>
</p>

<h1 align="center">About ❓</h1>

<a href="https://snipesharp.xyz/">Snipesharp</a> is an Open Source & easy to use Minecraft name Sniper featuring 24/7 giftcard (prename) & normal sniping, Discord RPC & more! Snipesharp is coded in [.NET](https://dotnet.microsoft.com/en-us/) C# and is focused on both speed and user friendliness.

### Created by:

<p align="center">
<a href="https://snipesharp.xyz/demented/">demented<img src="https://mc-heads.net/head/a5aee899-2d82-4594-aed1-f547178db6c0/100"></img></a><a href="https://snipesharp.xyz/stiliyan/"><img src="https://i.imgur.com/lMWqAlH.png">StiliyanKushev</img></a>
</p>

<h1 align="center">Features ⚡</h1>

### General
- Discord RPC
    - <img height="128px" src="https://user-images.githubusercontent.com/93228501/155626988-fed009c7-9e79-47a9-9d9d-22b86e3295eb.png" align="center"/>
- Auto NameMC follow on success
- Ease of use
- Automatic updates
- 24/7 sniping support
### Logging in
- Features
  - Completely possible through the Console Interface
  - Re-use of previous credentials
  - Giftcode redeeming when logging in with accounts which don't own Minecraft
- Methods
  - Microsoft Login
  - Mojang Login
  - Bearer Token Login
  - Using previous session credentials/bearer
- Configuration ([account.json](#configuration-file-locations-%EF%B8%8F))
  - All credentials (including Bearer Token) can be edited through account.json
### Sniping
- Features
  - Giftcard (prename) sniping
  - Seamless bearer reauthentication for 24/7 sniping support (Only works when using Microsoft & Mojang accounts)
  - Snipe custom name
  - Snipe from a [custom name list](#configuration-file-locations-%EF%B8%8F)
    - Automatically clean [name list of names](#configuration-file-locations-%EF%B8%8F) which were already sniped, can be disabled from [config.json](#configuration-file-locations-%EF%B8%8F)
    - Supports sniping 24/7 (Bearer gets updated using Microsoft/Mojang Account credentials)
    - Snipes in top to bottom order
  - Automatic (suggested) offset based on your ping and more math
- Configuration ([config.json](#configuration-file-locations-%EF%B8%8F) & In App)
  - Custom offset configured in app
  - Custom name configured in app
  - Custom name list configured in [names.json](#configuration-file-locations-%EF%B8%8F)
  - Custom packets sent amount configured in [config.json](#configuration-file-locations-%EF%B8%8F)
  - Custom delay between packets sent configured in [config.json](#configuration-file-locations-%EF%B8%8F)
### Post sniping
- Discord webhooks
  - Features
    - Webhooks contain your desired username & the name you sniped
    - Webhooks contain Minecraft character head of the account which sniped the name
  - Configuration ([config.json](#configuration-file-locations-%EF%B8%8F))
    - Custom username contained in webhook
    - Enable/disable webhook to snipesharp Discord server
    - Enable/disable webhook to custom Discord server
- Automatic skin change
  - Configuration ([config.json](#configuration-file-locations-%EF%B8%8F))
    - Custom skin
    - Custom skin type (classic/slim)

<h1 align="center">Installing ✅</h1>

To use snipesharp you can just download an executable from [here](http://download.snipesharp.xyz), through our [website](https://snipesharp.xyz) or from a button at the top of this document

Optionally, you can compile & install snipesharp using the install scripts that come with the repository. The only **dependency/requirement** for this is to have **[.NET](https://dotnet.microsoft.com/en-us/download/)** installed.

### Windows
To use the **Windows** install script, all you have to do is double click the `install_win.bat` file.

### Linux
On **Linux** you will have to make the `install_linux.sh` file executable by running `chmod +x install_linux.sh`, if it's not already executable. After that you run it with superuser privileges by running `sudo install_linux.sh`.

Similarly, if you have an **arm64/aarch64 architecture CPU**, you will have to make the file executable the same way you would normally, but run it without superuser privileges by running `./install_linux.sh`

<h1 align="center"><a href="https://snipesharp.xyz/donate/">Donate ❤️</a></h1>

### Donate to us through our website ❤️ https://snipesharp.xyz/donate/

<h1 align="center">Troubleshooting & FAQ 🪛</h1>

If youre having trouble while using snipesharp, check for solutions of frequently asked questions on the [Wiki](https://github.com/snipesharp/snipesharp/wiki).
If you still can't fix your issue or you just want to hang out, you can join [our Discord server](https://snipesharp.xyz/discord) and ask for help in the [#help](https://discord.com/channels/943483411597758494/943583091878932491) channel

<a href="https://discord.gg/ptFvZ8AYuU">
    <img src="https://img.shields.io/discord/943483411597758494?color=567CFF&label=discord&logo=discord&logoColor=ffffff&style=for-the-badge">
</a>

<h1 align="center">Configuration file locations ⚙️</h1>

### Windows
- `%appdata%` = WindowsPartition:\\Users\\{user}\\AppData\\Roaming
- `account.json` = %appdata%\\.snipesharp\\account.json
- `config.json` = %appdata%\\.snipesharp\\config.json
- `names.json` = %appdata%\\.snipesharp\\names.json
- `latest.log` = %appdata%\\.snipesharp\\logs\\latest.log
### Linux
- `~/` = `echo $HOME`
- `account.json` = ~/.snipesharp/account.json
- `config.json` = ~/.snipesharp/config.json
- `names.json` = ~/.snipesharp/names.json
- `latest.log` = ~/.snipesharp/logs/latest.log

<h1><h1/>
  
<a href="https://snipesharp.xyz/">![Untitled](https://user-images.githubusercontent.com/93228501/155002588-ab6d285d-2a5d-4ba1-86ac-85a35a253289.gif)</a>
