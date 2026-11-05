# Bark Nibbles Inventory — User Guidelines

This guide explains how to install the app on a Windows PC and use it from other devices over Wi-Fi.

## Connect to the currently running app

The app currently running on the host PC is available at [http://localhost:5128](http://localhost:5128). Open that address **on the host PC** and sign in with the administrator account already created for this inventory.

To use this same app from another device, leave the host PC and its app running and open the appropriate address in the other device's browser:

- **Same trusted Wi-Fi:** `http://<host-PC-IPv4>:5128`. Find the host PC's IPv4 address by running `ipconfig` on that PC.
- **Different Wi-Fi:** connect both devices to the same authorized Tailscale network and use `http://<host-Tailscale-IP>:5128`. Follow the VPN and firewall steps in section 3 below.

This connects to the host's existing app and database. Do not install or start another package to connect a client device; a separate installation creates a separate, initially empty database.

## 1. Choose the host PC

Choose the PC that will keep the inventory database and run the app. It needs to stay powered on, connected to the network, and running Bark Nibbles Inventory whenever other devices need access.

The package is for **Windows x64** and includes its runtime. Visual Studio, .NET, and a separate database server are not required on the destination PC.

## 2. Install and create the administrator

1. Download `Bark-Nibbles-Inventory-Windows-x64-No-Database.zip` from the project repository and copy it to the host PC. This package does not contain a database; the app creates a fresh database and seeds the initial catalog when started. This is a separate installation, not a way to connect to the app already running on another PC. If you were separately given an authorized database snapshot package, keep it private and share it only through a secure channel.
2. In File Explorer, right-click the ZIP, choose **Extract All...**, and extract **all** contents into a writable folder, such as `Documents\BarkNibbles`. Do not run the batch file from inside the ZIP or copy it out by itself.
3. Open the extracted folder and double-click **00 - START HERE.bat** (the first file in the folder). You can also run `Start-Bark-Nibbles-Inventory.bat` directly.
4. Keep the command window open while the inventory is in use.
5. On the host PC, double-click **01 - OPEN APP IN BROWSER.url** to open the app at `http://localhost:5128`. You can also type that address into a browser. If an admin account already exists, sign in with it. Otherwise, on first run, create the single admin account with a username and a strong password of at least 12 characters, then confirm the password. Initial setup is available only from the host PC.

There is no public registration or additional staff account. The administrator login is required to use the dashboard and inventory.

## 3. Connect a phone or another PC on Wi-Fi

1. Connect the host PC and the phone or other computer to the same trusted Wi-Fi network/router.
2. On the host PC, open **Command Prompt**, run `ipconfig`, and find the **IPv4 Address** under its active **Wireless LAN adapter Wi-Fi** section. Use the current address shown there; it may change when the PC reconnects to Wi-Fi.
3. On the other device, open a browser and enter `http://<host-PC-IPv4>:5128`, replacing the placeholder with the host address. For example: `http://192.168.1.25:5128`.
4. Sign in using the admin username and password created on the host PC.
5. If Windows Firewall prompts on the host PC, permit the app on **Private networks only**. Do not allow it on Public networks.

Use Chrome, Edge, Safari, or Firefox. The responsive pages adapt to phone screens. Sign out from the sidebar when finished on a shared device.

### Using a different Wi-Fi network

To connect from outside the host's Wi-Fi, use a private VPN such as Tailscale. Install it on the host PC and each authorized phone or computer, sign them in to the same private tailnet, and ensure the tailnet access policy permits only the devices or people who need the inventory. Keep the host PC running and connected to Tailscale.

On the host PC, open PowerShell **as administrator** in the extracted app folder and run:

```powershell
.\Configure-Bark-Nibbles-VPN-Firewall.ps1
```

This adds a Windows Firewall rule for TCP port 5128 restricted to Tailscale IPv4 addresses (`100.64.0.0/10`); it does not open the port to arbitrary internet clients. In Tailscale, find the host PC's Tailscale IPv4 address (or run `tailscale ip -4` on the host). On a device connected to the same authorized tailnet, open `http://<host-Tailscale-IP>:5128` and sign in. The VPN encrypts traffic between devices; do not use the app over untrusted networks without the VPN.

To remove the firewall rule later, run PowerShell as administrator and execute:

```powershell
Remove-NetFirewallRule -DisplayName "Bark Nibbles Inventory (Tailscale)"
```

Do **not** forward port 5128 on the router or expose the app directly to the internet. If you use another VPN, configure its firewall policy to allow only its authenticated private peers instead of using the Tailscale-specific script.

## 4. Products and stock

- **Products** lists the catalog. Use search and the low-stock filter to find items. Add, edit, view, or delete products using the available actions.
- Product photos appear in the horizontal carousel above the product list. Upload or replace a photo from a product’s **Edit** page; JPEG, PNG, and WebP files up to 5 MB are supported.
- **Stock In** records received or added quantity.
- **Stock Out** records quantity removed or sold. The app prevents removing more than the recorded quantity on hand.
- **Transactions** shows the stock movement history.
- **Account settings** in the sidebar lets the administrator update the username and password. Confirm the current password to save; leave the new password fields blank when only changing the username. New passwords must be at least 12 characters.
- The dashboard summarizes product count, stock quantity/value, restock alerts, and recent movements.
- Set a product's reorder level to enable its low-stock alert. A blank/unset value is shown as “—”.
- Product prices are displayed in Philippine pesos (PHP). Confirm the price and stock values against your business records.

## 5. Database, changes, and backups

- The public-safe `Bark-Nibbles-Inventory-Windows-x64-No-Database.zip` contains no `inventory.db`; the app creates one on first run and seeds the initial 30-product catalog. A separate snapshot package may include business records and must never be published publicly.
- All browser sessions connected to the **same running host app** see and update that host's database.
- Each extracted installation has its own database. Changes made on one PC do not automatically copy, sync, or merge into another installation.
- To back up the inventory, stop the app first by closing its command window, then copy `inventory.db` and the `wwwroot\uploads\products` folder to a safe location. Keep backups private because they contain business data and product photos. Restart the app by running `Start-Bark-Nibbles-Inventory.bat`.
- Do not delete or replace `inventory.db` to troubleshoot without first making a backup.

## 6. Security

- There is one admin account. Passwords are stored as password hashes, and repeated login attempts are rate-limited.
- If you forget the password, use **Forgot your password?** on the login page from the host PC at `http://localhost:5128`. Enter the admin username and set a new password of at least 12 characters. Password resets are blocked from phones and other network devices.
- The package serves **HTTP, not HTTPS**. Use it only on a trusted local network or inside an authenticated VPN, which encrypts traffic between devices. Do not visit it over public Wi-Fi without the VPN.
- Do not expose the host through router port forwarding or use it as a public internet website. For remote use, limit access through a private VPN and its device/user access policy.
- Use a unique, strong admin password and keep it private. Sign out on shared devices. Keep any database snapshot and product photos private; do not post them or `inventory.db` in a public repository.

## 7. Troubleshooting

**The page does not load on the host PC**

- Check that `Start-Bark-Nibbles-Inventory.bat` is still running and its window has not been closed.
- If the command window says `BarkNibblesInventory.exe is not recognized` or cannot be found, the package may not have been fully extracted or may be an older download. Use **Extract All...** on the current `Bark-Nibbles-Inventory-Windows-x64.zip`, then run `Start-Bark-Nibbles-Inventory.bat` from the extracted folder. The current launcher searches nested folders for the executable.
- If the app reports `http://localhost:5000`, stop it and start it with `Start-Bark-Nibbles-Inventory.bat` from the current package. The expected address is `http://localhost:5128`; the launcher also prints it in its command window. If you started the app directly, use the newer package or pass the intended URL explicitly.
- Use the current `Bark-Nibbles-Inventory-Windows-x64-No-Database.zip`. The older `BarkNibbles-Windows-x64.zip` is a legacy package.
- Try `http://localhost:5128` on the host PC.
- If the command window reports that port 5128 is already in use, close the other app using it or restart the host PC.

**The page does not load on a phone or another PC**

- Verify both devices are on the same Wi-Fi/router, and that the host app is still running.
- Use the host PC's current IPv4 address from `ipconfig`, not `localhost` and not the phone's IP address.
- Check that the URL uses `http://` and ends with `:5128`.
- Check Windows Firewall and allow the app on a trusted **Private** network.
- Some guest Wi-Fi networks block devices from seeing each other. Use the normal private Wi-Fi instead.

**The page does not load from another Wi-Fi network**

- Confirm Tailscale is installed and connected on both the host and client, and both are on the same authorized tailnet.
- On the host, run the included firewall configuration script as administrator.
- Use the host's Tailscale IPv4 address with port `5128`, not its local Wi-Fi address.
- Check the tailnet access policy and ensure the host app is still running.

**The host PC's IP address changed**

Run `ipconfig` again on the host PC and use its current IPv4 address. The Wi-Fi router may assign a different address later.

**The app asks for admin setup again**

Check that the app is running from the same extracted folder as before and that its `inventory.db` file is still there. If the database is missing, the app creates a fresh one and requires admin setup again.

## 8. Source code and GitHub

The project and recommended GitHub repository name are **Bark-Nibbles-Inventory**. The repository URL is `https://github.com/JeramiAbad/Bark-Nibbles-Inventory`.

If GitHub still shows the old repository name `BarkNibbles`, open the repository's **Settings**, change **Repository name** under **General** to `Bark-Nibbles-Inventory`, and save.

To publish the source from PowerShell opened in the project folder:

```powershell
git init -b main
git add .
git status
```

Review the staged files. `.gitignore` excludes the local database, build output, and generated installer. Never force-add `inventory.db`, which contains business data. Then run:

```powershell
git commit -m "Initial Bark Nibbles Inventory source"
git remote add origin https://github.com/JeramiAbad/Bark-Nibbles-Inventory.git
git push -u origin main
```

GitHub may ask you to sign in through a browser or credential manager. Never put a password or access token in the remote URL. The `dist/Bark-Nibbles-Inventory-Windows-x64-No-Database.zip` package is safe to publish because it contains no business database. Never commit or publicly release `inventory.db` or an archive containing it. Share any package with a database snapshot only through an authorized, secure channel.
