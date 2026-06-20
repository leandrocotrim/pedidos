# TASKS — Pedidos API

Tarefas derivadas do [PRD](PRD.md) e [SDD](SDD.md). Marque com `[x]` ao concluir.

---

## Fase 1 — Setup do Projeto

- [x] **T01** Criar solution `pedidos.sln` com projetos: `Api`, `Application`, `Domain`, `Infrastructure`, `Application.Tests`
- [x] **T02** Configurar `Dockerfile` multi-stage (build + runtime)
- [x] **T03** Configurar `docker-compose.yml` com serviços `api` e `db` (PostgreSQL 16)
- [x] **T04** Adicionar pacotes NuGet:
  - `Npgsql.EntityFrameworkCore.PostgreSQL`
  - `Microsoft.EntityFrameworkCore.Design`
  - `xUnit` + `FluentAssertions` + `Moq`

---

## Fase 2 — Domínio

- [x] **T05** Criar enum `OrderType` (`Standard`, `Express`, `Subscription`)
- [x] **T06** Criar entidade `OrderItem` com propriedades `Id`, `Product`, `Quantity`, `UnitPrice`, `Subtotal`
- [x] **T07** Criar entidade `Order` com:
  - Propriedades `Id`, `Type`, `Items`
  - Cálculo de `Subtotal`, `Total` e `DiscountOrSurcharge` por tipo
  - Métodos `AddItem`, `UpdateItem`, `RemoveItem`

---

## Fase 3 — Infraestrutura

- [x] **T08** Criar `AppDbContext` com mapeamento Fluent API para `Order` e `OrderItem`
- [x] **T09** Criar interface `IOrderRepository` com métodos `AddAsync`, `GetByIdAsync`, `UpdateAsync`
- [x] **T10** Implementar `OrderRepository` com EF Core
- [x] **T11** Gerar migration inicial (`InitialCreate`)
- [x] **T12** Configurar `appsettings.json` e `appsettings.Development.json` com connection string

---

## Fase 4 — Aplicação (Casos de Uso)

- [x] **T13** Implementar `CreateOrderHandler` — valida request, cria `Order`, persiste
- [x] **T14** Implementar `GetOrderHandler` — carrega `Order`, projeta em `OrderResponse`
- [x] **T15** Implementar `UpdateOrderItemHandler` — localiza item, atualiza, persiste
- [x] **T16** Implementar `DeleteOrderItemHandler` — localiza item, remove, persiste
- [x] **T17** Criar DTOs de request/response: `CreateOrderRequest`, `OrderResponse`, `OrderItemResponse`

---

## Fase 5 — API (Endpoints)

- [x] **T18** Configurar `Program.cs` com DI, EF Core e roteamento
- [x] **T19** Implementar `POST /orders` — chama `CreateOrderHandler`, retorna `201` com ID
- [x] **T20** Implementar `GET /orders/{orderId}` — chama `GetOrderHandler`, retorna `200` com resumo
- [x] **T21** Implementar `PUT /orders/{orderId}/items/{itemId}` — chama `UpdateOrderItemHandler`
- [x] **T22** Implementar `DELETE /orders/{orderId}/items/{itemId}` — chama `DeleteOrderItemHandler`, retorna `204`
- [x] **T23** Configurar middleware de tratamento de erros (400/404)

---

## Fase 6 — Testes Unitários

- [x] **T24** Testar criação de pedido `standard` — total igual ao subtotal
- [x] **T25** Testar criação de pedido `express` — total = subtotal × 1,15
- [x] **T26** Testar criação de pedido `subscription` — total = subtotal × 0,90
- [x] **T27** Testar cálculo com múltiplos itens
- [x] **T28** Testar `GetOrderHandler` com pedido inexistente — deve retornar erro 404
- [x] **T29** Testar `UpdateOrderItemHandler` com item inexistente — deve retornar erro 404
- [x] **T30** Testar `DeleteOrderItemHandler` com item inexistente — deve retornar erro 404

---

## Fase 7 — Entrega

- [x] **T31** Criar repositório público no GitHub
- [x] **T32** Preencher `README.md` com instruções de execução e diagrama de solução
- [x] **T33** Garantir que `docker compose up` sobe o ambiente completo
- [x] **T34** Validar todos os endpoints via cURL ou Postman/Bruno
- [x] **T35** Confirmar cobertura de testes para os 3 tipos de pedido

---

## Fase 8 — Swagger / OpenAPI

- [x] **T36** Adicionar pacote `Scalar.AspNetCore` (UI moderna) ao projeto `Api`
- [x] **T37** Configurar `AddOpenApi()` e `MapOpenApi()` no `Program.cs`
- [x] **T38** Montar `MapScalarApiReference()` com título, versão e descrição da API
- [x] **T39** Decorar endpoints com `.WithName()`, `.WithSummary()` e `.WithTags()` para documentação gerada
- [x] **T40** Validar UI acessível em `http://localhost:8080/scalar/v1`

---

## Prioridade para o Live Code

> Foque primeiro em **T13 → T14 → T19 → T20** (criar pedido + consultar resumo).
> Esses são os endpoints avaliados na sessão ao vivo.
