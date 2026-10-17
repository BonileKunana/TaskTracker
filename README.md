\# TaskTracker — Full-Stack Task Management Application



A full-stack task management application built with \*\*ASP.NET Core, Entity Framework Core, PostgreSQL, and JavaScript\*\*. The project demonstrates REST API development, database integration, automated testing, containerised local development, and cloud deployment.



\*\*\[Live Demo](https://tasktracker-web-kjg6.onrender.com/)\*\* · \*\*\[REST API](https://tasktracker-0wki.onrender.com/tasks)\*\* · \*\*\[GitHub Repository](https://github.com/BonileKunana/TaskTracker)\*\*



\## Features



\- \*\*Task management:\*\* Create, view, edit, complete, and delete tasks.

\- \*\*RESTful API:\*\* ASP.NET Core API endpoints for task operations.

\- \*\*Database persistence:\*\* PostgreSQL integration using Entity Framework Core and database migrations.

\- \*\*Input validation:\*\* Data Transfer Objects (DTOs) and validation rules for incoming requests.

\- \*\*Maintainable backend:\*\* Dependency injection, a service layer, and centralised exception handling.

\- \*\*Health monitoring:\*\* Health endpoint for checking API availability.

\- \*\*Automated testing:\*\* Dedicated test project with 18 tests passing in the current verified run.

\- \*\*Continuous integration:\*\* GitHub Actions workflow that builds the project and runs automated tests on a self-hosted Windows runner.

\- \*\*Containerised development:\*\* Docker Compose for running the API and PostgreSQL locally.

\- \*\*Cloud deployment:\*\* Frontend hosted on Render, API hosted on Render, and PostgreSQL hosted on Neon.



\## Technology Stack



| Area | Technologies |

|---|---|

| Backend | C#, .NET 9, ASP.NET Core |

| Data access | Entity Framework Core, Npgsql |

| Database | PostgreSQL, Neon |

| Frontend | HTML, CSS, JavaScript |

| Containers | Docker, Docker Compose |

| CI/CD | GitHub Actions, self-hosted Windows runner |

| Hosting | Render |

| Testing | .NET test project |



\## Architecture



The application separates the frontend, API, and database responsibilities.



```text

User

&#x20; |

&#x20; v

Frontend (HTML, CSS, JavaScript)

&#x20; |

&#x20; | HTTP requests

&#x20; v

ASP.NET Core REST API

&#x20; |

&#x20; v

Task Service

&#x20; |

&#x20; v

Entity Framework Core

&#x20; |

&#x20; v

PostgreSQL Database

```



In production, the frontend and API are hosted on Render, with PostgreSQL hosted on Neon. Docker Compose is used for local development.



\## Live Application



\- \*\*Frontend:\*\* https://tasktracker-web-kjg6.onrender.com/

\- \*\*API health check:\*\* https://tasktracker-0wki.onrender.com/health

\- \*\*Retrieve tasks:\*\* https://tasktracker-0wki.onrender.com/tasks



The API exposes endpoints for creating, retrieving, updating, and deleting tasks.



\## Running Locally



\### Prerequisites



\- Git

\- .NET 9 SDK

\- Docker Desktop with Docker Compose

\- A compatible local database configuration



\### 1. Clone the repository



```bash

git clone https://github.com/BonileKunana/TaskTracker.git

cd TaskTracker

```



\### 2. Run the application with Docker Compose



From the directory containing the Compose file, run:



```bash

docker compose up --build

```



This builds and starts the services defined in the Compose configuration.



Use the port mappings and environment configuration in the Compose file to access the API. Configure database credentials through local environment settings rather than committing secrets to source control.



To stop the services, run:



```bash

docker compose down

```



\### 3. Run the automated tests



From the repository root, run:



```bash

dotnet test TaskTracker.Api.Tests/TaskTracker.Api.Tests.csproj

```



The tests verify the behaviours covered by the project's automated test suite.



\## Continuous Integration



The GitHub Actions workflow runs when changes are pushed to `main` or a pull request targets `main`.



The pipeline performs these steps:



1\. Checks out the repository.

2\. Verifies the installed .NET SDK.

3\. Restores dependencies.

4\. Builds the test project.

5\. Runs the automated tests.



\*\*Current verified result:\*\* 18 tests passed, with zero failures.



The workflow currently uses a self-hosted Windows runner, so the machine must be available and the runner process running for jobs to execute.



\## Engineering Practices Demonstrated



\- REST API design and HTTP methods

\- Separation of concerns and service-layer design

\- DTOs and request validation

\- Entity Framework Core migrations

\- PostgreSQL integration

\- Exception handling and health checks

\- Automated testing and continuous integration

\- Docker-based local development

\- Environment-based configuration and secret management

\- Deployment troubleshooting, including API connectivity and CORS



\## Future Improvements



Potential enhancements include:



\- Authentication and user-specific task lists

\- Pagination, filtering, and sorting

\- Improved automated integration testing

\- API documentation with Swagger/OpenAPI

\- Frontend UI improvements and accessibility enhancements

\- Monitoring and structured application logging



\## Author



\*\*Bonile Kunana\*\*



Software Developer | Full-Stack Development | Cloud Solutions



\[GitHub Profile](https://github.com/BonileKunana)





