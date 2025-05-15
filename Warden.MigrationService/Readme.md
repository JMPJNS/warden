# Documentation
https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations

# Common issues
- if it says `No migrations were found in assembly`, check if the entity framework version in all projects is the same
  - mine was 9.0.4 in some projects and 9.0.5 in others, didn't work