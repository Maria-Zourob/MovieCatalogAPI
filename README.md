# MovieCatalogAPI

**MovieCatalogAPI** is a powerful RESTful API built using **ASP.NET Core** for managing a movie catalog. It allows you to perform CRUD operations (Create, Read, Update, Delete) on movie data, search movies by various filters, and export or import movie data using Excel files. This API also features role-based access control with user roles like **Admin** and **Reader**.

## Features

- **CRUD operations** for movies (Create, Read, Update, Delete).
- **Search functionality** for movies based on title or description.
- **Filter movies by category** and **release date range**.
- **Role-based authorization** for different levels of access (Admin, Reader).
- **Movie data import and export** in Excel format.
- **Category suggestion** based on movie descriptions.

## Endpoints

### Get all movies:
```http
GET /api/movies/all
Search movies by title/description:
GET /api/movies/search?query={query}

Get movies by category:
GET /api/movies/bycategory?category={category}

Get movies by release date range:
GET /api/movies/date-range?start={startDate}&end={endDate}

Get the latest movies:
GET /api/movies/latest

Get movie by ID:
GET /api/movies/{id}

Create a new movie:
POST /api/movies/create

Update an existing movie:
PUT /api/movies/update/{id}

Delete a movie:
DELETE /api/movies/delete/{id}

Installation

Clone the repository:

git clone https://github.com/Maria-Zourob/MovieCatalogAPI.git


Navigate into the project directory:

cd MovieCatalogAPI


Restore the project dependencies:

dotnet restore


Run the project:

dotnet run


Open the API in your browser or Postman to start interacting with the endpoints.

Authentication

This API uses JWT authentication. Ensure that you have a valid JWT token to access the endpoints.

Users can be assigned roles such as Admin and Reader. Admin users can perform all actions, while Reader users have limited access.

License

This project is licensed under the MIT License - see the LICENSE
 file for details.

Contributing

Feel free to fork the repository and submit issues and pull requests. Contributions are welcome!
