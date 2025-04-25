$env:TEST_RESULTS_PATH = "./MRA.TestingResults"
$env:COVERAGE_HTML_PATH = "./MRA.TestingResults/CoverageReport"

if (Test-Path $env:TEST_RESULTS_PATH) {
    Write-Host "Deleting test results folder, $env:TEST_RESULTS_PATH"
    Remove-Item -Recurse -Force $env:TEST_RESULTS_PATH
}

dotnet test `
  --configuration Release `
  --collect:"XPlat Code Coverage" `
  --results-directory $env:TEST_RESULTS_PATH `
  $env:CSPROJ_PATH

reportgenerator `
  "-reports:$env:TEST_RESULTS_PATH/**/*.xml" `
  "-targetdir:$env:COVERAGE_HTML_PATH" `
  "-reporttypes:Html"

Write-Host "✅ Code Coverage report generated in $env:COVERAGE_HTML_PATH"
