# Bark Nibbles Inventory

Bark Nibbles Inventory helps Bark Nibbles keep track of products, stock on hand, restock levels, and stock movements. You can use it from a computer, phone, or tablet browser.

## Getting started

First, decide which situation applies:

- **The app is already running on a host PC:** connect to that app using the instructions below. Do not install another copy on your device.
- **You are setting up the app for the first time:** install it on the Windows PC that will host the inventory, then connect other devices to that PC.

The host PC stores the inventory database and must stay on, connected to the network, and running the app whenever others need access. Devices connected to the same host share the same inventory.

## Connect to an existing app

On the host PC, open `http://localhost:5128`. On a phone or another computer connected to the same trusted Wi-Fi, open `http://<host-PC-IPv4>:5128` in a browser. Replace the placeholder with the host PC's IPv4 address; find it by running `ipconfig` on the host.

Sign in with the administrator account for that app. If you do not have the credentials, ask the inventory administrator. There is one administrator account and no public staff registration.

For access from a different Wi-Fi, the host and authorized devices need to connect to the same private VPN, such as Tailscale. See [USER-GUIDELINES.md](./USER-GUIDELINES.md) for VPN and firewall setup.

## Set up a new host PC

1. Download the latest **Windows x64, No Database** package from the [project repository](https://github.com/JeramiAbad/Bark-Nibbles-Inventory). It creates a new, separate inventory; it does not connect to or copy data from another installation.
2. In File Explorer, right-click the ZIP and choose **Extract All...**. Extract all files into a writable folder. Do not run the app from inside the ZIP.
3. In the extracted folder, double-click **00 - START HERE.bat**. You can also run **Start-Bark-Nibbles-Inventory.bat**.
4. Keep the command window open while the app is in use.
5. On the host PC, open **01 - OPEN APP IN BROWSER.url**, or browse to `http://localhost:5128`.
6. On first run, create the administrator account with a strong password of at least 12 characters. If an account already exists in this installation, sign in instead.
7. To use the app from another device on the same trusted Wi-Fi, open `http://<host-PC-IPv4>:5128` in that device's browser and sign in.

If Windows Firewall prompts for access, allow it on **Private networks only**. If the startup script cannot find `BarkNibblesInventory.exe`, make sure you extracted the entire ZIP and run the script from the extracted folder.

## What you can do

- Add, edit, search, and organize products.
- Record stock coming in and going out.
- Review inventory levels, low-stock alerts, and transaction history.
- Upload product photos and update account settings.

For step-by-step usage, troubleshooting, backups, and security guidance, see [USER-GUIDELINES.md](./USER-GUIDELINES.md).

## Important data and security notes

- Each installation has its own `inventory.db`. Separate installations do not synchronize automatically.
- The public **No Database** package starts a fresh inventory. The app seeds its initial catalog on first run.
- Inventory data and product photos are business records. Back up `inventory.db` and `wwwroot\uploads\products` only after stopping the app, and keep backups private.
- The app uses HTTP, not HTTPS. Use it only on a trusted local network or inside an authenticated private VPN. Never expose port 5128 to the public internet.

## For developers

The source project targets .NET 8 and uses Entity Framework Core with SQLite. With the .NET 8 SDK installed, run:

```powershell
dotnet run --project .\Bark-Nibbles-Inventory.csproj --launch-profile http
```
