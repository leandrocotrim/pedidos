# CLAUDE.md

## Idioma
Sempre responda em **português**.

## Documentação do Projeto
- [Constitution](constitution.md) — princípios e diretrizes do projeto
- [PRD](PRD.md) — Product Requirements Document
- [SDD](SDD.md) — Spec-Driven Development
- [TASKS](TASKS.md) — lista de tarefas derivadas do PRD/SDD
- [README](README.md) — desenho da solução e instruções de execução

## Stack Técnica
- **Linguagem:** C# (.NET 8)
- **Padrão de API:** Minimal APIs
- **Banco de dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Conteinerização:** Docker + Docker Compose
- **Testes:** xUnit + FluentAssertions

## Convenções de Código
- Responder sempre em português nos comentários e comunicação
- Organização em camadas: `Api`, `Application`, `Domain`, `Infrastructure`
- Endpoints seguem o padrão RESTful definido no PRD
- Regras de negócio residem exclusivamente na camada `Application/Domain`

## Comandos Úteis
```bash
# Subir ambiente completo
docker compose up -d

# Rodar testes
dotnet test

# Aplicar migrations
dotnet ef database update --project src/Infrastructure
```
