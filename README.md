# Dealmatcher

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Flutter](https://img.shields.io/badge/Flutter-3.x-02569B?style=for-the-badge&logo=flutter&logoColor=white)](https://flutter.dev/)
[![Microsoft SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=for-the-badge&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD-00599E?style=for-the-badge)](#system-architecture)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

**Dealmatcher** is a modern platform designed to connect offer creators and buyers, facilitating seamless deal matching, real-time activity tracking, offer exploration, and secure transactions. The project implements a Domain-Driven Design (DDD) & Clean Architecture backend using **.NET 9** alongside a responsive **Flutter** frontend.

This project was developed for the **"Software Engineering 2"** (SE 2) course.

## Core Features

- **Offer Matching & Discovery**: Browse, filter, and match relevant deals based on user preferences and activity.
- **Interactive Conversations**: Direct messaging and negotiation workflows between offer creators and interested buyers.
- **Purchase & Transaction Lifecycle**: Complete workflow for managing transactions, purchases, and deal status tracking.
- **Real-Time Activity Monitoring**: Comprehensive tracking of user interactions, deal views, and engagement metrics.
- **Secure Authentication**: Secure user management and session handling using JWT tokens.
- **State Management & Cloud Integrations**: 
  - **Redis Cart Store**: High-performance key-value persistence for managing active user shopping carts with automatic TTL expiration.
  - **Azure Blob Storage**: Secure document and media attachment storage.
- **Clean Architecture & DDD**: Strict separation of concerns (Domain, Use Cases, Infrastructure, API) adhering to SOLID principles.

## System Architecture

The platform consists of two primary components:

1. **Frontend**: A cross-platform web and mobile interface built with **Flutter** (Dart).
2. **Backend Services**: A **.NET 9** Web API applying Clean Architecture & Domain-Driven Design principles, backed by **SQL Server** for persistent relational storage and **Redis** as a fast key-value store for user cart state (`RedisCartRepository`).

## Tech Stack

- **Backend**: .NET 9, ASP.NET Core Web API, Entity Framework Core, Clean Architecture / DDD
- **Frontend**: Flutter 3.x, Dart, Material Design
- **Database**: Microsoft SQL Server
- **State & Cart Storage**: Redis 7
- **Infrastructure**: Docker, Docker Compose
- **Cloud Services**: Azure Blob Storage
- **Security**: JWT Authentication

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local backend development)
- [Flutter SDK](https://flutter.dev/docs/get-started/install) (for local frontend development)

### Quick Start with Docker

1. **Clone the repository**:
    ```bash
    git clone https://github.com/xxxDKGxxx/Dealmatcher.git
    cd Dealmatcher
    ```

2. **Configure environment variables**:
    Create a `.env` file in the root directory based on `.env.example`:
    ```bash
    cp .env.example .env
    ```
    Fill in the required values for database connections, JWT secrets, Azure Blob Storage, and Redis.

3. **Run the application**:
    ```bash
    docker-compose up -d
    ```

The services will be available at:
- **Frontend**: [http://localhost:8080](http://localhost:8080)
- **Backend API**: [http://localhost:5000](http://localhost:5000)
- **SQL Server**: `localhost:1433`
- **Redis**: `localhost:6379`

## Authors

- [Dominik Zieliński](https://github.com/xxxDKGxxx)
- [Jan Zaborski](https://github.com/janzaborski)
- [Paweł Szymański](https://github.com/szymanp135)
- [Frycek1](https://github.com/Frycek1)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
