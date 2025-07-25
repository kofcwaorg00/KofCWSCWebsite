﻿#*******************************************************************************************************************************
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

################################################################################################################
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$counter = 1
################################################################################################################
$uri = "https://localhost:7078/GetDistListForExchange/AGWM/14/0/0"
$response = Invoke-RestMethod -Uri $uri -Method Get
#$allContacts = Get-MailContact | Select-Object Name,Alias,PrimarySmtpAddress,CustomAttribute1,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,CustomAttribute10

$workingid = ""
foreach($res in $response)
{
    $total=$response.count
    "Working on $($counter) of $($total)"
    $allgroups = Get-DistributionGroup -ResultSize Unlimited | Select-Object Name,PrimarySmtpAddress #| Export-Csv -Path "C:\DistributionGroups.csv" -NoTypeInformation
    $existingGroup = $allgroups | Where-Object { $_.PrimarySmtpAddress -eq $res.GroupEmail}
    #---------------------------------------------------------------------------------------------------------------------------------
    ## PROCESS GROUPS
    if ($res.groupEmail -ne $workingid){
        #check to see if group exists
        if ($existingGroup){
            # DELETE IT
            Write-Output "Removing Group $($res.groupEmail)"
            Remove-DistributionGroup -Identity $res.groupEmail -Confirm:$false
            # ADD IT
            Write-Output "Adding Group After Removing $($res.groupEmail)"
            New-DistributionGroup -Name $res.groupName -PrimarySmtpAddress $res.groupEmail -Type "Distribution"
            # UPDATE IT Now Update the Managed By
            Set-DistributionGroup -Identity $res.groupEmail -ManagedBy $res.managedBy -CustomAttribute1 "KOFCWSCPROC"
            Set-DistributionGroup -Identity $res.groupEmail -RequireSenderAuthenticationEnabled $false
        }
        else {
            # CREATE IT
            Write-Output "Adding Group New $($res.groupEmail)"
            New-DistributionGroup -Name $res.groupName -PrimarySmtpAddress $res.groupEmail -Type "Distribution"
            # Now Update the Managed By
            Set-DistributionGroup -Identity $res.groupEmail -ManagedBy $res.managedBy -CustomAttribute1 "KOFCWSCPROC"
            Set-DistributionGroup -Identity $res.groupEmail -RequireSenderAuthenticationEnabled $false
        }
    }
    
    #---------------------------------------------------------------------------------------------------------------------------------
    # Check if the contact already exists
    # this may be obsolete as we are supposed to delete then add
    $existingContact = Get-MailContact -Identity $res.recipientEmail -ErrorAction SilentlyContinue # -Filter "EmailAddress -eq '$res.recipientEmail'" -ErrorAction SilentlyContinue
    #---------------------------------------------------------------------------------------------------------------------------------
    ## PROCESS CONTACTS
    if ($existingContact) {
        Write-Output "Mail Contact for $($res.recipientEmail) already exists. No need to add."
    } else {
        # Add the mail contact if it doesn't exist
        Write-Output "Adding Contact for $($res.recipientEmail)"   
        New-MailContact -Name $res.recipientName -ExternalEmailAddress $res.recipientEmail
        Start-Sleep -Seconds 5
        Set-MailContact -Identity $res.recipientemail -CustomAttribute1 "KOFCWSCPROC"

    }
     
     Write-Output "Adding $($res.recipientEmail) to $($res.groupEmail)"
     Add-DistributionGroupMember -Identity $res.groupEmail -Member $res.recipientEmail


    $workingid = $res.groupEmail
    $counter++
}
$stopwatch.Stop()

Write-Output "Elapsed Time: $($stopwatch.Elapsed.TotalSeconds) Seconds.  Worked on $($counter) Items"