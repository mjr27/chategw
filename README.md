## Data preparation

### Dotnet data preparation
```shell
$ cd src/ChatEgw.UI.Indexer
export EGW_SEARCH_DSN="Server=localhost;Database=search;Port=15432;Username=postgres;Password=password"
```
#### Create database

```shell
$ dotnet run -- migrate
```
#### Import base data

```shell
$ dotnet run -- import egw -f "Host=localhost;Username=user;Password=password;Database=database"
```

#### Export data to file for python postprocessing
```shell
$ dotnet run -- export tsv paragraphs-raw.tsv
```

### Extract tagging from raw data
```shell
$ cd cuda-backend