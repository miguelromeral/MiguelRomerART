# MiguelRomerART

Azure Web App to show art

### Run Tests

On MRA.UnitTests:

```
> dotnet test --collect:"XPlat Code Coverage"
> reportgenerator "-reports:**/MRA.TestingResults/**/*.xml" "-targetdir:MRA.TestingResults/CoverageReport" "-reporttypes:Html"
```