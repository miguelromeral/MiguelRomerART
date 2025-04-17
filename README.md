# MiguelRomerART

Azure Web App to show art

### Run Tests

On MRA.UnitTests:

```
> dotnet test --collect:"XPlat Code Coverage"
> reportgenerator "-reports:**/TestResults/**/*.xml" "-targetdir:TestResults/CoverageReport" "-reporttypes:Html"
```