#!/bin/bash

DOTNET_INSTALLER=dotnet-sdk-2.2.207-linux-arm.tar.gz
DOTNET_INSTALLER_URL=https://download.visualstudio.microsoft.com/download/pr/fca1c415-b70c-4134-8844-ea947f410aad/901a86c12be90a67ec37cd0cc59d5070/dotnet-sdk-2.2.207-linux-arm.tar.gz
DOTNET_LOCATION=$HOME/dotnet


red=`tput setaf 1`
green=`tput setaf 2`
yellow=`tput setaf 3`
reset=`tput sgr0`

clear

echo "${green}**** NetCoreRover Environment Installer ****${reset}"

echo
#echo "${green}Updating Raspberry Pi...${reset}"
#sudo apt-get update

#echo
#echo "${green}Upgrading Raspberry Pi...${reset}"
#sudo apt-get upgrade -y

#echo
#echo "${green}Upgrading Raspberry Pi distro...${reset}"
#sudo apt-get dist-upgrade -y

echo
echo "${green}Installing required dependencies...${reset}"
sudo apt-get install curl libunwind8 gettext


# Download dotnet only if needed...
if [ -f "$DOTNET_INSTALLER" ]; then
    echo
    echo "${green}netcore installer already exist.${reset}"
else 
    echo
    echo "${green}Downloading netcore installer...${reset}"
    wget "$DOTNET_INSTALLER_URL"
fi

# if dotnet is already installed, remove it first
if [ -d "$DOTNET_LOCATION" ]; then
    echo
    echo "${green}netcore already installed, removing it first.${reset}"
    rm -r "$DOTNET_LOCATION"
fi
mkdir -p "$DOTNET_LOCATION"

#echo
#echo "${green}Extracting netcore to $DOTNET_LOCATION...${reset}"
#tar zxf "$DOTNET_INSTALLER" -C "$DOTNET_LOCATION"

#echo
#echo "${green}removing dotnet installer...${reset}"
#rm "$DOTNET_INSTALLER"

echo
echo "${green}Configuring Environment variables...${reset}"
export DOTNET_ROOT=$HOME/dotnet 
export PATH=$PATH:$HOME/dotnet

if grep -q "$DOTNET_LOCATION" "$HOME/.bashrc"; then
    # bashrc already contains what we need so we dont need to add it again
    echo dotnet already in PATH environment variable
else
    # bashrc does not contain PATH configuration add it with dotnet path
    echo Adding PATH configuration...
    echo -e "\nPATH=\$PATH:$DOTNET_LOCATION" >> ~/.bashrc
fi  

echo
echo "${yellow}Remember to enable SSH to publish!${reset}"

