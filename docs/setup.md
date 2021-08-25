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

```

## Enable ssh

```bash
sudo raspi-config
```
