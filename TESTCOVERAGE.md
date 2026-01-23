## Generera testrapport

Från testprojektet

>> dotnet test --collect:"XPlat Code Coverage"

Efter körningen hamnar coverage-filen automatiskt i:

MyZombieProject.Tests/TestResults/<GUID>/coverage.cobertura.xml

VARJE NY KÖRNING SKAPAR NY RAPPORTMAPP!


## Läsbar testrapport med ReportGenerator

1.) Installera ReportGenerator (globalt)

>> dotnet tool install -g dotnet-reportgenerator-globaltool


2.) Generera rapporter:

Om det är första gången du genererar rapporter kommer en mapp TestResults att skapas i MyZombieProject.Test

Om mappen TestResults finns rensa allt innehåll i den innan du genererar ny rapport (gör livet enklare ;))

Från testprojektet

>> reportgenerator -reports:"TestResults/**/*.xml" -targetdir:"coverage-report" -reporttypes:Html

öppna filen index.html i din browser för att läsa rapporten


NOTERA!!! båda mapparna coverage-report samt TestResults är ignorerade i Github och kommer inte att checkas in



