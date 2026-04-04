"Deploying the assets..."
foreach ($type in "Binary", "Text") {
	$file = "$($type.ToLower())-extensions"
	$path = "sindresorhus/$file/main/$file.json"
	Invoke-WebRequest "https://raw.githubusercontent.com/$path" -OutFile "res/Text/${type}Extensions.json"
}
