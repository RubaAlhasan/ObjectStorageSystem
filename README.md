# Object Storage System

Object Storage System is a .NET 6 application that provides APIs to store and retrieve objects/files using an ID, name, or path.
The system supports multiple storage providers, including Amazon S3, database, local file system, and FTP. 

## Features

- Store and retrieve blobs of data using unique identifiers.
- Supports multiple storage backends:
  - Amazon S3 (Rest API Call)
  - Database
  - Local File System
  - FTP
- JWT authentication for secure access.

## Prerequisites

- .NET 6 SDK
- SQL Server
- AWS S3 account for S3 provider
- FTP server for FTP provider

## Getting Started

### Clone the Repository

```sh
git clone https://github.com/RubaAlhasan/ObjectStorageSystem.git
cd ObjectStorageSystem
```

### Configuration

Update the `appsettings.json` file with your specific configuration

### Database Migration

Run this command to update database:

```sh
dotnet ef database update
```

### Running the Application

```sh
dotnet run
```

## API Endpoints

### Store Blob

- **URL**: `/v1/blobs`
- **Method**: `POST`
- **Request Body**:
  ```json
  {
    "id": "any_valid_string_or_identifier",
    "data": "SGVsbG8gU2ltcGxlIFN0b3JhZ2UgV29ybGQh"
  }
  ```
- **Description**: Stores a blob of data. The data must be Base64 encoded and the id should be unique.

### Retrieve Blob

- **URL**: `/v1/blobs/{id}`
- **Method**: `GET`
- **Response Body**:
  ```json
  {
    "id": "any_valid_string_or_identifier",
    "data": "SGVsbG8gU2ltcGxlIFN0b3JhZ2UgV29ybGQh",
    "size": "234",
    "created_at": "2023-01-22T21:37:55Z"
  }
  ```
- **Description**: Retrieves a blob using Id.

### Authentication

endpoint to obtain a JWT token:

- **URL**: `/api/Auth/login`
- **Method**: `POST`
- **Request Body**:
  ```json
  {
    "username": "admin@gmail.com",
    "password": "P@ssw0rd123"
  }
  ```
- **Response Body**:
  ```json
  {
    "token": "jwt-token"
  }
  ```

## Integration Tests

1. Navigate to the test project directory.
2. Run the tests using the .NET CLI:

```sh
dotnet test
```

## License

This project is licensed under the MIT License.
