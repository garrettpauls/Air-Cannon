properties {
	$baseDir = resolve-path ..
	$buildDir = "$baseDir\Build"
	$releaseDir = "$buildDir\Release"
	$sourceDir = "$baseDir\Src"
	$toolsDir = "$buildDir\Tools"
}

task default -depends Compile

$framework = '4.0'

task Compile -depends Clean {
	msbuild "/t:Clean;Rebuild" /p:Configuration=Release /p:OutputPath="$releaseDir\" "$sourceDir\AirCannon.sln"
}

task Clean {
	msbuild "$sourceDir\AirCannon.sln" /t:clean
}