# Get the current date
$currentDate = Get-Date -Format "yyyy.MM.dd"
# Define the base filename
$baseFilename = "ipfees-production"
# Construct the final filename
$filename = "{0}-{1}.mongodb" -f $baseFilename, $currentDate
$connectionString = "mongodb+srv://valerbocan:cxpAkYCALM15zC8j@ipfeescluster.ayqdiey.mongodb.net"

# Output the filename
Write-Host $filename
$command = "mongodump"
$arguments = "--archive=""$filename""", "--db=IPFees", "$connectionString"

Start-Process -FilePath $command -ArgumentList $arguments