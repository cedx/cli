"Deploying the assets..."
"Binary", "Text" | ForEach-Object {
	$file = "$($_.ToLower())-extensions";
	$path = "sindresorhus/$file/main/$file.json";
	Invoke-WebRequest "https://raw.githubusercontent.com/$path" -OutFile "res/ConvertTo-Encoding/${_}Extensions.json"
}
