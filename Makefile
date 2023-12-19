DOCKER_COMPOSE_FILE := docker-compose.yml

build:
	docker compose $(DOCKER_COMPOSE_FILE) build

start:
	docker compose $(DOCKER_COMPOSE_FILE) up -d

up: start

stop:
	docker compose $(DOCKER_COMPOSE_FILE) down 

down: stop

logs:
	docker compose $(DOCKER_COMPOSE_FILE) logs --tail 100
