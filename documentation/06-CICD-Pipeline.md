# 16 — CI/CD Pipeline

## Current Setup

The project uses a **Taskfile.yaml** as a local task runner that serves as the foundation for CI/CD. It provides scriptable commands for installation, linting, testing, and building.

### Available Tasks

```sh
task default          # Show available tasks
task install          # Install all dependencies (npm + dotnet restore)
task build            # Build frontend and backend
task test             # Run all tests
task lint             # Run linters
task ci               # Full CI pipeline entry point
```

The `ci` task runs sequentially:

```
task install → task lint → task test → task build
```

### CI Pipeline (to be configured)

Recommended GitHub Actions workflow (`.github/workflows/ci.yml`):

```yaml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  backend:
    runs-on: ubuntu-latest
    services:
      postgres:
        image: postgres:14-alpine
        env:
          POSTGRES_USER: d1111
          POSTGRES_PASSWORD: mypassword
          POSTGRES_DB: Saas-Dashboard
        ports:
          - 5431:5432
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0"
      - run: dotnet restore backend/APIs/APIs.sln
      - run: dotnet build backend/APIs/APIs.sln -c Release --no-restore
      - run: dotnet test backend/APIs/APIs.sln -c Release --no-build

  frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: "20"
      - run: npm ci
        working-directory: frontend
      - run: npm run lint
        working-directory: frontend
      - run: npm run build
        working-directory: frontend
      - run: npm test
        working-directory: frontend
```

### Docker Image Build & Push

For automated Docker image deployment:

```yaml
docker-build:
  runs-on: ubuntu-latest
  needs: [backend, frontend]
  steps:
    - uses: actions/checkout@v4
    - name: Login to DockerHub
      uses: docker/login-action@v3
      with:
        username: ${{ secrets.DOCKERHUB_USERNAME }}
        password: ${{ secrets.DOCKERHUB_TOKEN }}
    - name: Build & Push Backend
      run: |
        docker build -t ${{ secrets.DOCKERHUB_USERNAME }}/saas-backend:latest ./backend
        docker push ${{ secrets.DOCKERHUB_USERNAME }}/saas-backend:latest
    - name: Build & Push Frontend
      run: |
        docker build -t ${{ secrets.DOCKERHUB_USERNAME }}/saas-frontend:latest ./frontend
        docker push ${{ secrets.DOCKERHUB_USERNAME }}/saas-frontend:latest
```

## Deployment Pipeline

Triggered on push to `main`:

1. Run CI (lint → test → build)
2. Build Docker images for backend and frontend
3. Push images to DockerHub registry
4. Deploy to production server via SSH or orchestration platform

## Local Development Workflow

```sh
# Start infrastructure
docker compose up -d db

# Start backend
task backend:run

# Start frontend (separate terminal)
task frontend:dev
```
