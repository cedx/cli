Write-Output "Deploying the assets..."
"Binary", "Text" | ForEach-Object {
	$file = "$($_.ToLowerInvariant())-extensions";
	$path = "sindresorhus/$file/refs/heads/main/$file.json";
	Invoke-WebRequest "https://raw.githubusercontent.com/$path" -OutFile "res/${item}Extensions.json"
}
