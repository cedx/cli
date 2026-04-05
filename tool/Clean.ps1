"Deleting all generated files..."
Remove-Item bin, src/obj -ErrorAction Ignore -Force -Recurse
Remove-Item var/* -Exclude .gitkeep -Force -Recurse
