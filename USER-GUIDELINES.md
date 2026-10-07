# Bark Nibbles Inventory — User Guidelines

This guide explains how to install the app on a Windows PC and use it from other devices over Wi-Fi.

## 1. Choose the host PC

Choose the PC that will keep the inventory database and run the app. It needs to stay powered on, connected to the network, and running Bark Nibbles Inventory whenever other devices need access.

The package is for **Windows x64** and includes its runtime. Visual Studio, .NET, and a separate database server are not required on the destination PC.

## 2. Install and create the administrator

1. Download `Bark-Nibbles-Inventory-Windows-x64.zip` from the project's GitHub Release and copy it to the host PC.
2. Extract the ZIP into a folder where the user can write files, such as `Documents\BarkNibbles`. Do not run the app from inside the ZIP.
3. Open the extracted folder and run `Start-Bark-Nibbles-Inventory.bat`.
4. Keep the command window open while the inventory is in use.
5. On the host PC, open [http://localhost:5128](http://localhost:5128).
6. The first-run setup page is only available from the host PC. Create the single admin account with a username and a strong password of at least 12 characters, then confirm the password.

There is no public registration or additional staff account. The administrator login is required to use the dashboard and inventory.

## 3. Connect a phone or another PC on Wi-Fi

1. Connect the host PC and the phone or other computer to the same trusted Wi-Fi network/router.
2. On the host PC, open **Command Prompt**, run `ipconfig`, and find the **IPv4 Address** under its active **Wireless LAN adapter Wi-Fi** section. Use the current address shown there; it may change when the PC reconnects to Wi-Fi.
3. On the other device, open a browser and enter `http://<host-PC-IPv4>:5128`, replacing the placeholder with the host address. For example: `http://192.168.1.25:5128`.
4. Sign in using the admin username and password created on the host PC.
5. If Windows Firewall prompts on the host PC, permit the app on **Private networks only**. Do not allow it on Public networks.

Use Chrome, Edge, Safari, or Firefox. The responsive pages adapt to phone screens. Sign out from the sidebar when finished on a shared device.

### Using a different Wi-Fi network

Devices on another Wi-Fi network normally cannot reach the host directly. The host and client need to share a trusted network with a route between them. If remote access is needed, use a properly secured private VPN and configure the host firewall for that private connection. Do **not** forward port 5128 on the router or expose the app directly to the internet.

## 4. Products and stock

- **Products** lists the catalog. Use search and the low-stock filter to find items. Add, edit, view, or delete products using the available actions.
- **Stock In** records received or added quantity.
- **Stock Out** records quantity removed or sold. The app prevents removing more than the recorded quantity on hand.
- **Transactions** shows the stock movement history.
- The dashboard summarizes product count, stock quantity/value, restock alerts, and recent movements.
- Set a product's reorder level to enable its low-stock alert. A blank/unset value is shown as “—”.
- Product prices are displayed in Philippine pesos (PHP). Confirm the price and stock values against your business records.

## 5. Database, changes, and backups

- The app creates `inventory.db` in the extracted application folder the first time it starts. It also creates the database tables and initial 30-product catalog.
- All browser sessions connected to the **same running host app** see and update that host's database.
- Installing the package on another PC creates a separate database. Data does not automatically copy, sync, or merge between installations.
- To back up the inventory, stop the app first by closing its command window, then copy `inventory.db` to a safe location. Keep backups private because they contain business data. Restart the app by running `Start-Bark-Nibbles-Inventory.bat`.
- Do not delete or replace `inventory.db` to troubleshoot without first making a backup.

## 6. Security

- There is one admin account. Passwords are stored as password hashes, and repeated login attempts are rate-limited.
- The package serves **HTTP, not HTTPS**. Credentials and session cookies are not encrypted in transit. Use this only on a trusted private Wi-Fi network—not public Wi-Fi.
- Do not expose the host through router port forwarding or use it as a public internet website. The app is intended for a trusted local network.
- Use a unique, strong admin password and keep it private. Sign out on shared devices.

## 7. Troubleshooting

**The page does not load on the host PC**

- Check that `Start-Bark-Nibbles-Inventory.bat` is still running and its window has not been closed.
- Try `http://localhost:5128` on the host PC.
- If the command window reports that port 5128 is already in use, close the other app using it or restart the host PC.

**The page does not load on a phone or another PC**

- Verify both devices are on the same Wi-Fi/router, and that the host app is still running.
- Use the host PC's current IPv4 address from `ipconfig`, not `localhost` and not the phone's IP address.
- Check that the URL uses `http://` and ends with `:5128`.
- Check Windows Firewall and allow the app on a trusted **Private** network.
- Some guest Wi-Fi networks block devices from seeing each other. Use the normal private Wi-Fi instead.

**The host PC's IP address changed**

Run `ipconfig` again on the host PC and use its current IPv4 address. The Wi-Fi router may assign a different address later.

**The app asks for admin setup again**

Check that the app is running from the same extracted folder as before and that its `inventory.db` file is still there. Every separate extracted installation keeps its own database and admin account.

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

GitHub may ask you to sign in through a browser or credential manager. Never put a password or access token in the remote URL. Publish the Windows installer as an asset on a GitHub Release; do not commit build outputs.
