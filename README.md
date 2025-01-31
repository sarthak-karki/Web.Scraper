# SEO Checker

## Overview

The SEO Checker is a C#.NET application designed to help automatically check the SEO ranking of a company website using specific keywords on Google. This application retrieves the search results and identifies the positions where the target URL appears within the first 100 results.

## Features

- Fetches search results from Google for specified keywords.
- Identifies and returns the positions of the target URL in the search results.
- Logs any errors that occur during the fetching process.
- Uses Dependency Injection for better testability and modularity.
- Includes unit tests to ensure the correctness of the functionality.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- An IDE such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/sarthak-karki/Web.Scraping.git
2. Restore the dependencies:
    ```sh
    dotnet restore
3. Build the project
    ```sh
    dotnet build

### Usage
The application accepts two paramenters: 
1. Keywords for the SEO search.
2. The target URL to check in the search results. 

These parameters are hardcoded in the `Main` method of the `Program` class for simplicity.

### Project Structure

* Web.Scraping: Contains the main application logic and entry point.
* Web.Scraping.Client: Contains the HttpClientService used to fetch HTML content.
* Web.Scraping.Tests: Contains unit tests for the application.
