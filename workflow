
name: RaceDayAPI Build

on:
  push:
    branches:
      - main
  pull_request:
    branches:
      - main
  workflow_dispatch:

jobs:
  build:
    name: Build ASP.NET Core API
    runs-on: ubuntu-latest

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Install .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Restore dependencies
        run: dotnet restore RaceDayAPI.csproj

      - name: Build project
        run: dotnet build RaceDayAPI.csproj --configuration Release --no-restore
