mkdir "%APPDATA%\JetBrains\dotCover\v2.7\Plugins" 2> NUL
mkdir "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec" 2> NUL
del "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec\*.*" 2> NUL
copy /y Machine.Specifications.dll "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec"
copy /y Machine.Specifications.pdb "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec" > NUL
copy /y Machine.Specifications.dotCover.Runner.2.7.dll "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec"
copy /y Machine.Specifications.dotCover.Runner.2.7.pdb "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec" > NUL
copy /y Machine.Specifications.dotCover.Provider.2.7.dll "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec"
copy /y Machine.Specifications.dotCover.Provider.2.7.pdb "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec" > NUL
copy /y Machine.Specifications.Runner.Utility.dll "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec"
copy /y Machine.Specifications.Runner.Utility.pdb "%APPDATA%\JetBrains\dotCover\v2.7\Plugins\mspec" > NUL
pause
