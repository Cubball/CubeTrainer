# Cube Trainer

Cube Trainer is a web application designed to help speedcubers improve their solving skills by practicing and tracking their progress with OLL and PLL algorithms

## Getting Started

### Running Locally with Docker

#### Prerequisites

- Git installed
- Docker and Docker Compose installed

#### Steps

1. Clone the repository:

```bash
git clone https://github.com/Cubball/CubeTrainer.git
```

2. Navigate to the project directory:

```bash
cd CubeTrainer
```

3. Build and run the Docker containers:

```bash
docker compose up --build
```

4. Open your browser and go to [http://localhost:3000](http://localhost:3000)

## Components

The project consists of three main components:

### 1. Frontend (CubeTrainer.Frontend)

- Built with React and TypeScript
- Uses React Router for navigation
- TanStack Query for state management
- Styled with TailwindCSS
- Uses the `cubing.js` library to render cube visualizations

### 2. API (CubeTrainer.API)

- ASP.NET Core Web API (.NET 8.0)
- Entity Framework Core
- PostgreSQL database
- User authentication and authorization

### 3. Cube Library (CubeTrainer.Cube)

- A .NET library for Rubik's cube operations
- Has a basic implementation of the Kociemba algorithm for cube solving
