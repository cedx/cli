"Deploying the assets..."
"Binary", "Text" | ForEach-Object {
	$file = "$($_.ToLower())-extensions";
	$path = "sindresorhus/$file/main/$file.json";
	Invoke-WebRequest "https://raw.githubusercontent.com/$path" -OutFile "res/${_}Extensions.json"
}
