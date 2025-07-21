#*******************************************************************************************************************************
#BEGIN PREPARE ENV
# modules
Import-Module ExchangeOnlineManagement


# Parameters
$tenantId = "cba846cf-1683-4d63-8c9c-93e37f653c83"
$clientId = "a478a88d-31c3-4e02-b090-be3ea365cdb2"
$clientSecret = "PiL8Q~6C6sikh4MCrHj65sYruy44hr5wR8cWqdiH"
#$scope = "https://graph.microsoft.com/.default"
$scope = "https://outlook.office365.com/.default"

# Obtain token
$body = @{
    grant_type    = "client_credentials"
    client_id     = $clientId
    client_secret = $clientSecret
    scope         = $scope
}
$tokenResponse = Invoke-RestMethod -Uri "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token" -Method Post -Body $body
$accessToken = $tokenResponse.access_token

#$accessToken

# Connect to Exchange Online using app-only authentication
Connect-ExchangeOnline -AppId $clientId -Organization $tenantId -AccessToken $accessToken
#END PREPARE ENV
################################################################################################################

$groups = Get-DistributionGroup -ResultSize Unlimited
$results = @()

# Filter groups where the name starts with "Test"
$filteredGroups = $groups | Where-Object { $_.Name -like "DD*" }

$filteredGroups
foreach ($group in $filteredGroups){
    Write-Output "Removing Group $($group.Name)"
    Remove-DistributionGroup -Identity $group.Name -Confirm:$false
}



$groups
foreach ($group in $groups) {
    $members = Get-DistributionGroupMember -Identity $group.Identity
    foreach ($member in $members) {
        $results += [PSCustomObject]@{
            GroupDisplayName = $group.DisplayName
            GroupName = $group.Name
            MemberName = $member.Name
            MemberIdentity = $member.Identity
            MemberEmail = $member.PrimarySmtpAddress
        }
    }
}

$results | Export-Csv -Path "D:\Data\OneDrive\Development\KofCWSCData\Exchange\DistributionGroupMembers.csv" -NoTypeInformation






#To export the list of mail contacts to a CSV file for reporting or further analysis:
$AllContacts = Get-MailContact | Export-Csv -Path "D:\Data\OneDrive\Development\KofCWSCData\Exchange\MailContacts.csv" -NoTypeInformation
$AllContacts

Get-Recipient -ResultSize Unlimited | Export-Csv -Path "D:\Data\OneDrive\Development\KofCWSCData\Exchange\AllRecipients.csv"


Get-Mailbox -ResultSize Unlimited | Select-Object DisplayName, PrimarySmtpAddress,CustomAttribute1, CustomAttribute2
Get-MailContact -ResultSize Unlimited | Select-Object DisplayName, ExternalEmailAddress,CustomAttribute1, CustomAttribute2, CustomAttribute3, CustomAttribute4, CustomAttribute5, CustomAttribute6, CustomAttribute7, CustomAttribute8, CustomAttribute9, CustomAttribute10
(Get-MailContact -ResultSize Unlimited).Count
(Get-Mailbox -ResultSize Unlimited).Count



Get-MailContact -ResultSize Unlimited | ForEach-Object {
    Set-MailContact $_.Identity `
        -CustomAttribute1 "" `
        -CustomAttribute2 "" `
        -CustomAttribute3 "" `
        -CustomAttribute4 "" `
        -CustomAttribute5 "" `
        -CustomAttribute6 "" `
        -CustomAttribute7 "" `
        -CustomAttribute8 "" `
        -CustomAttribute9 "" `
        -CustomAttribute10 "" `
        -CustomAttribute11 "" `
        -CustomAttribute12 "" `
        -CustomAttribute13 "" `
        -CustomAttribute14 "" `
        -CustomAttribute15 ""
}

