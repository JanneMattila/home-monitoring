# Raspian setup

## Networking

Note: Rasbian default user account is `pi` and
default password is `raspberry`.

Note: If `wlan0` is in blocked state use `rfkill unclock` to resolve it.

Now follow these
[instructions](https://raspberrypihq.com/how-to-connect-your-raspberry-pi-to-wifi/).

## Keyboard

```bash
sudo dpkg-reconfigure keyboard-configuration
```

## Install

```bash
sudo apt update && sudo apt upgrade -y

# If issues with ssh
sudo apt-get install --reinstall libkrb5-3

# Finding serial ports
sudo apt install setserial
sudo setserial -g /dev/ttyACM0

ls -lF /sys/class/tty
# lrwxrwxrwx 1 root root 0 Aug 25 17:10 ttyACM0 -> ../../devices/platform/soc/3f980000.usb/usb1/1-1/1-1.2/1-1.2:1.0/tty/ttyACM0/
# lrwxrwxrwx 1 root root 0 Aug 25 16:23 ttyAMA0 -> ../../devices/platform/soc/3f201000.serial/tty/ttyAMA0/
```

## deCONZ

[Raspbian instructions](https://phoscon.de/en/conbee2/install#raspbian)

## Enable ssh

```bash
sudo raspi-config
```

## Home Assistant

[Home Assistant for Raspberry Pi](https://www.home-assistant.io/installation/raspberrypi)
