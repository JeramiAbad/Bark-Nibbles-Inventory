# Bark Nibbles Inventory

A responsive ASP.NET Core MVC inventory app for Bark Nibbles. It tracks products, stock levels, restock thresholds, and stock-in/stock-out activity. The app uses a local SQLite database and supports one administrator account.

## GitHub repository

The project and recommended GitHub repository name are **Bark-Nibbles-Inventory**. The repository URL is:

`https://github.com/JeramiAbad/Bark-Nibbles-Inventory`

If the repository is currently named `BarkNibbles`, first rename it on GitHub: open the repository, choose **Settings**, update **Repository name** under **General** to `Bark-Nibbles-Inventory`, and save.

Then open PowerShell in the project folder and publish the source:

```powershell
git init -b main
git add .
git status
```

Review the staged files before committing. The local SQLite database, build output, and generated installer are excluded; never force-add `inventory.db`. Then commit and push:

```powershell
git commit -m "Initial Bark Nibbles Inventory source"
git remote add origin https://github.com/JeramiAbad/Bark-Nibbles-Inventory.git
git push -u origin main
```

GitHub may ask you to authenticate through a browser or credential manager. Do not put a password or access token in the remote URL. Never commit or publicly publish `inventory.db` or any archive containing it; it holds business records. The `dist/Bark-Nibbles-Inventory-Windows-x64-No-Database.zip` package is safe to publish and creates a fresh database on first run. Keep any database snapshot package separate and share it only through an authorized, secure channel.

## Install on another Windows PC

1. Obtain `Bark-Nibbles-Inventory-Windows-x64-No-Database.zip` from the project's GitHub repository and extract **all** files to a writable folder on the Windows x64 PC. This public-safe package has no database; the app creates a fresh database and seeds the initial catalog on first run. A separate authorized package may contain a private database snapshot—never publish or share it publicly.
2. Run `Start-Bark-Nibbles-Inventory.bat` and keep its window open while using the app.
3. On that PC, open [http://localhost:5128](http://localhost:5128) and complete the one-time admin setup using a strong password of at least 12 characters.
4. To use a phone or another computer on the same Wi-Fi, open `http://<host-PC-IPv4>:5128` and sign in. Find the host PC's address by running `ipconfig` and reading the IPv4 address for its active Wi-Fi adapter.
5. If Windows Firewall asks, allow access on **Private networks only**.

If the startup window says `BarkNibblesInventory.exe` is not recognized, the package may not have been fully extracted or may be an older download. Use **Extract All...** on the current ZIP, then run the included startup script; it searches nested folders for the executable. The older `BarkNibbles-Windows-x64.zip` is a legacy package and does not include the local database snapshot.
If the app reports port 5000 instead of 5128, run it with `Start-Bark-Nibbles-Inventory.bat`. The app now defaults to port 5128; explicit URL/port settings still take precedence.

For complete installation steps, everyday usage, backup guidance, troubleshooting, and network limitations, see [USER-GUIDELINES.md](./USER-GUIDELINES.md).

## Important network and database notes

- The host PC must keep the app running. All connected devices use the same database on that PC.
- A device needs a route to the host, usually by joining the same trusted Wi-Fi/router. A different Wi-Fi network normally cannot connect directly.
- The app uses HTTP, not HTTPS. Login credentials and session cookies are not encrypted in transit. Use only on a trusted private network; never use public Wi-Fi or expose port 5128 to the public internet.
- Each separate installation has its own `inventory.db`; databases do not synchronize automatically.
- The public-safe ZIP has no database and seeds a fresh catalog on first run. Any separate snapshot package contains business records; treat it as confidential and share it only with authorized people over a secure channel.
- Back up `inventory.db` only after stopping the app.

## Development

The source project targets .NET 8 and uses Entity Framework Core with SQLite. To run it from a development PC with the .NET 8 SDK installed:

```powershell
dotnet run --project .\Bark-Nibbles-Inventory.csproj --launch-profile http
```

The development database is `inventory.db` in the project folder. The Windows x64 install package is self-contained and does not require the .NET SDK on the destination PC.
