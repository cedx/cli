"Deleting all generated files..."
Remove-Item bin, obj -ErrorAction Ignore -Force -Recurse
Remove-Item var/* -Exclude .gitkeep -Force -Recurse
