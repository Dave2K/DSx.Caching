#!/usr/bin/env pwsh
dotnet test --settings:./tests/coverlet.runsettings --results-directory:./coverage
dotnet tool install --global reportgenerator --version 5.2.0
reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"./coverage-report" -reporttypes:Html