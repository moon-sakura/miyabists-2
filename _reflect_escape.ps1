$ErrorActionPreference = 'Continue'
$dataDir = 'C:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64'
$asm = [System.Reflection.Assembly]::LoadFrom((Join-Path $dataDir 'sts2.dll'))
try {
    $cmd = $asm.GetType('MegaCrit.Sts2.Core.Commands.CreatureCmd')
    if ($cmd) {
        $methods = $cmd.GetMethods() | Where-Object { $_.Name -eq 'Escape' }
        foreach ($m in $methods) {
            $ps = $m.GetParameters()
            $names = ($ps | ForEach-Object { $_.Name }) -join ', '
            $types = ($ps | ForEach-Object { $_.ParameterType.Name }) -join ', '
            "Escape($types)  params: [$names]"
        }
    } else {
        'CreatureCmd type not found'
    }
} catch {
    'ERR: ' + $_.Exception.Message
}
