import gulp from "gulp"
import {spawn} from "node:child_process"
import {readdir, rm} from "node:fs/promises"
import {join} from "node:path"

# Builds the project.
export build = ->
	await npx "coffee", "--compile", "--no-header", "--output", "lib", "src"

# Deletes all generated files.
export clean = ->
	files = await readdir "lib", recursive: yes, withFileTypes: yes
	await rm join file.parentPath, file.name for file from files when file.isFile() and not file.name.endsWith ".d.ts"
	await rm join("var", file), recursive: yes for file from await readdir "var" when file isnt ".gitkeep"

# Performs the static analysis of source code.
export lint = ->
	await npx "coffeelint", "--file=etc/coffeelint.json", "gulpfile.coffee", "src"

# Publishes the package.
export publish = ->
	{default: {version}} = await import("./package.json", with: type: "json")
	await npx "gulp"
	await run "npm", "publish", "--registry=#{registry}" for registry from ["https://registry.npmjs.org", "https://npm.pkg.github.com"]
	await run "git", action..., "v#{version}" for action from [["tag"], ["push", "origin"]]

# Watches for file changes.
export watch = ->
	npx "coffee", "--compile", "--map", "--no-header", "--output", "lib", "--watch", "src"

# The default task.
export default gulp.series clean, build

# Executes a command from a local package.
npx = (command, args...) -> run "npm", "exec", "--", command, args...

# Spawns a new process using the specified command.
run = (command, args...) -> new Promise (resolve, reject) ->
	spawn command, args, shell: on, stdio: "inherit"
		.on "close", (code) -> if code then reject(Error [command, args...].join(" ")) else resolve()