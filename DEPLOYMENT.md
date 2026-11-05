# Bark Nibbles Inventory deployment

## Connect to the current app

The app currently running on the host PC is at [http://localhost:5128](http://localhost:5128). On that PC, open the address and sign in with the existing admin account.

To connect another device to **that same app and database**, leave the host PC running and open one of these addresses in the other device's browser:

- Same trusted Wi-Fi: `http://<host-PC-IPv4>:5128` (get the host address using `ipconfig`).
- Different Wi-Fi: connect the host and authorized device to the same private Tailscale network, configure the host firewall by running `Configure-Bark-Nibbles-VPN-Firewall.ps1` as administrator, then open `http://<host-Tailscale-IP>:5128`.

Do not install a second copy on a client device; each installation has its own database. Do not forward port 5128 or expose the app directly to the public internet.

## Install a separate host

The public-safe Windows x64 package is `dist/Bark-Nibbles-Inventory-Windows-x64-No-Database.zip`. Extract it on the new host PC, double-click `00 - START HERE.bat` (the first file in the folder), and keep its window open. Then double-click `01 - OPEN APP IN BROWSER.url` to open `http://localhost:5128` on that PC. You can also run `Start-Bark-Nibbles-Inventory.bat` directly and type the address into a browser. This package has no database; it creates a fresh one and seeds the starter catalog on first run. Set up an admin account on the new host. This separate install does not automatically include or synchronize the current inventory.

Never commit or publicly distribute `inventory.db` or an archive containing it. A separate package containing business records must be shared only through an authorized, secure channel. For full installation, Wi-Fi, account, backup, security, and troubleshooting instructions, see the [User Guidelines](./USER-GUIDELINES.md). The project overview and development instructions are in the [README](./README.md).
