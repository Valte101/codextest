# Sport Field Reservations (.NET + TypeScript + PostgreSQL)

This repo contains a full-stack sample for managing sport field reservations:

- **Backend:** ASP.NET Core Minimal API with EF Core + PostgreSQL
- **Frontend:** React + TypeScript (Vite)
- **Database:** PostgreSQL 16

## Project structure

- `backend/SportReservations.Api` - API + persistence logic
- `frontend` - TypeScript web app
- `docker-compose.yml` - local PostgreSQL and API runtime

## Run locally

### 1) Start PostgreSQL + API via Docker

```bash
docker compose up -d postgres api
```

API will be available at `http://localhost:5000`.

### 2) Start the frontend

```bash
cd frontend
npm install
npm run dev -- --host 0.0.0.0 --port 5173
```

Open `http://localhost:5173`.

## API endpoints

- `GET /api/fields` - list available sports fields
- `GET /api/reservations` - list reservations
- `POST /api/reservations` - create reservation (rejects overlaps)

### Create reservation payload

```json
{
  "fieldId": 1,
  "playerName": "City Strikers",
  "startTime": "2026-01-10T18:00:00Z",
  "endTime": "2026-01-10T19:00:00Z",
  "notes": "Bring extra bibs"
}
```
