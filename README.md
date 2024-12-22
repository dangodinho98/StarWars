# StarWars

A simple ASP.NET Core MVC project using Razor Pages to integrate with the **SWAPI.Tech API**. The application includes static user authentication, a paginated grid on the index page, and a dropdown search functionality.

## Features
- **Authentication**:
  - Static credentials: Username: `daniel`, Password: `123`
- **Integration with SWAPI.Tech API**:
  - Fetch and display data from the Star Wars API.
- **Paginated Grid**:
  - Displays fetched data with pagination on the index page.
- **Dropdown Search**:
  - Search functionality using a dropdown menu to filter results.

---

## Technology Stack
- **ASP.NET Core MVC**: Application framework.
- **Razor Pages**: For dynamic views.
- **SWAPI.Tech API**: Data source for Star Wars resources.
- **Bootstrap**: For responsive and styled UI.

## Getting Started
### Prerequisites
- **.NET SDK**: Version 9 or later.
- **SWAPI.Tech API**: No API key required.

### Installation Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/StarWars.git
   cd StarWars
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build and run the application:
   ```bash
   dotnet run
   ```

4. Access the application in your browser:
   ```
   http://localhost:<port>
   ```

---

## Authentication
- **Static User Credentials**:
  - **Username**: `daniel`
  - **Password**: `123`

- Authentication is hardcoded for simplicity and demonstration purposes.

---

## Features Overview

### Index Page
- Displays a **paginated grid** populated with data from the SWAPI.Tech API.
- Includes a **dropdown menu** for filtering data based on specific criteria (e.g., Star Wars categories like people, planets, or starships).

### Dropdown Search
- Dropdown allows the user to select a category (e.g., `People`, `Planets`).
- Triggers a search query to fetch and display results matching the selection.

### Pagination
- Pagination controls are implemented to navigate through API results.
- Supports a configurable number of items per page.

---

## Project Structure
```
StarWars/
│   ├── UrlShortener.Web/             # Web project
│   ├── UrlShortener.Application/     # Application services and interfaces
│   ├── UrlShortener.Domain/          # Domain entities and constants
│   ├── UrlShortener.Infrastructure/  # Database and caching implementations
```

---

## Example Workflow

### Login
1. Navigate to the login page (`/Home/Login`).
2. Enter the static credentials:
   - **Username**: `daniel`
   - **Password**: `123`
3. On successful login, you are redirected to the index page.

### Index Page
1. Use the dropdown to select a category (e.g., `Planets`).
2. View the results in a paginated grid.
3. Navigate between pages using pagination controls.

---

## API Integration
- **Base URL**: `https://www.swapi.tech/api`
- The application uses HTTP GET requests to fetch data dynamically based on the user's dropdown selection.
- Data is mapped to the `SwapiEntity` model for rendering in Razor Pages.

---

## Customization
1. **Modify Static Credentials**:
   - Update the `HomeController` to change the username and password.

2. **Expand Search Options**:
   - Add additional dropdown categories by extending the API integration logic in `HomeController`.

3. **Styling**:
   - Update `wwwroot/css/site.css` to modify the UI.

---

