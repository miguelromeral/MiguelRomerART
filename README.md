# MiguelRomerART

Azure Web App to show art

## Unit Test Coverage

You can see this project [Code Coverage report](https://miguelromeral.github.io/MiguelRomerART/MRA.TestingResults/CoverageReport/index.html). It is generated via **GitHub Actions** when pushing into ```main``` branch.

### Run Tests on Local

On MRA.UnitTests:

```
> dotnet test --collect:"XPlat Code Coverage"
> reportgenerator "-reports:**/MRA.TestingResults/**/*.xml" "-targetdir:MRA.TestingResults/CoverageReport" "-reporttypes:Html"
```

