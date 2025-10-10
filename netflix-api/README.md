# Netflix API

## Overview
The Netflix API is a simple web API that provides weather forecast data. It allows users to retrieve weather forecasts for the upcoming days.

## Project Structure
The project is organized into the following directories and files:

- **Controllers**
  - `WeatherForecastController.cs`: Handles HTTP requests related to weather forecasts.
  
- **Models**
  - `WeatherForecast.cs`: Defines the `WeatherForecast` record with properties for date, temperature, and summary.
  
- **Services**
  - `WeatherService.cs`: Provides methods to generate and retrieve weather forecasts.
  
- **Properties**
  - `launchSettings.json`: Contains settings for launching the application in different environments.
  
- **Program.cs**: The entry point of the application, setting up the web application and configuring services.
  
- **appsettings.json**: Configuration settings for the application.
  
- **appsettings.Development.json**: Development-specific configuration settings.
  
- **netflix-api.csproj**: Project file containing information about dependencies and build settings.

## Setup Instructions
1. Clone the repository:
   ```
   git clone <repository-url>
   ```

2. Navigate to the project directory:
   ```
   cd netflix-api
   ```

3. Restore the dependencies:
   ```
   dotnet restore
   ```

4. Run the application:
   ```
   dotnet run
   ```

5. Access the API at `https://localhost:5001/weatherforecast` to get the weather forecast data.

## Usage
The API provides a single endpoint:
- `GET /weatherforecast`: Returns a list of weather forecasts for the next five days.

## License
This project is licensed under the MIT License.