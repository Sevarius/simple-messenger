```bash
dotnet ef migrations add <Name> --startup-project src/Host --project src/Data.Migrations
```

```bash
dotnet ef database update --startup-project src/Host --project src/Data.Migrations
```
