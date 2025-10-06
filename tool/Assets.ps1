Write-Host "Deploying the assets..."
foreach ($item in "Binary", "Text") {
	$file = "$($item.ToLowerInvariant())-extensions";
	$path = "sindresorhus/$file/refs/heads/main/$file.json";
	Invoke-WebRequest "https://raw.githubusercontent.com/$path" -OutFile "res/${item}Extensions.json"
}
