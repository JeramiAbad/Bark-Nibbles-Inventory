#Requires -RunAsAdministrator

$ruleName = "Bark Nibbles Inventory (Tailscale)"

Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue |
    Remove-NetFirewallRule

New-NetFirewallRule `
    -DisplayName $ruleName `
    -Description "Allows Bark Nibbles Inventory only from Tailscale IPv4 peers." `
    -Direction Inbound `
    -Action Allow `
    -Protocol TCP `
    -LocalPort 5128 `
    -RemoteAddress 100.64.0.0/10 `
    -Profile Any | Out-Null

Write-Host "Allowed TCP port 5128 from Tailscale IPv4 peers (100.64.0.0/10)."
