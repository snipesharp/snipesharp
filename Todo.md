# List of ideas/bug fixes to be implemented:
## Giftcard sniping
Giftcard sniping
Create profile with giftcard: https://mojang-api-docs.netlify.app/needs-auth/create-profile.html
Redeem giftcard: https://mojang-api-docs.netlify.app/needs-auth/redeem-gc.html
## Check if account owns minecraft
https://mojang-api-docs.netlify.app/needs-auth/check-if-owns-minecraft.html
## account.json screen viewing protection
Hide account credentials below line 50 and fill the first 50 lines with a warning and empty space.
Example:
```
This file contains sensitive information below line 50,
Dont scroll if someone can see the contents of this file
.
.
.
```
## Automatic 24/7 sniping
Useful if you are not using bearers which expire every 24h
- Option to snipe a list of chosen names [DONE]
- Option to snipe every 3 letter name
- Option to snipe every english name
## Better navigation
Make it so that pressing `Esc` while in either `Mojang Account` or `Bearer Token` prompts; goes back to choosing one of the prompts again
## Discord RPC
Implement Discord RPC to work while using the program
## Mojang Login
Implement Mojang Authentication & sniping with Mojang credentials
