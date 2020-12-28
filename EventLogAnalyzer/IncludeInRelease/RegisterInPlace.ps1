$filebase = Join-Path $PSScriptRoot EventLogAnalyzer.exe
echo $filebase

# https://gist.github.com/timabell/bc90e0808ec1cda173ca09225a16e194
# MIT license
$exts=@(
	"evtx")
echo "## setting up file associations"
foreach ($ext in $exts){
	$extfile=$ext+"file"
	$dotext="." + $ext
	cmd /c assoc $dotext=$extfile
	cmd /c "ftype $extfile=""$filebase"" ""%1"""
	echo ""
}

# see also:
# * https://superuser.com/q/406985/8271
# * https://gist.github.com/timabell/608fb680bfc920f372ac