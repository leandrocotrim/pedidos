# Pedidos API

API REST para gerenciamento de pedidos de uma loja online, construída com .NET 10, Minimal APIs e PostgreSQL.

---

## Desenho da Solução

```
         Cliente HTTP  (cURL / Postman / Scalar UI :8080/scalar/v1)
                │
                │ HTTP :8080
                ▼
┌───────────────────────────────────────────────────────────────────┐
│  API Layer  —  .NET 10 Minimal APIs                               │
│                                                                   │
│   Produtos                  Pedidos              Itens            │
│   POST   /products          POST /orders         PUT  /orders/…   │
│   GET    /products          GET  /orders/{id}    DEL  /orders/…   │
│   GET    /products/{id}                                           │
│   PUT    /products/{id}                                           │
│   DELETE /products/{id}                                           │
│                                                                   │
│   ▼ ValidationFilter<T>  (FluentValidation — retorna 400 aqui)   │
└──────────────────────────────┬────────────────────────────────────┘
                               │
┌──────────────────────────────▼────────────────────────────────────┐
│  Application Layer  —  Handlers                                   │
│                                                                   │
│  Produtos                      Pedidos / Itens                    │
│  CreateProductHandler          CreateOrderHandler                 │
│  GetProductHandler               └─ busca produto → snapshot      │
│  GetAllProductsHandler         GetOrderHandler                    │
│  UpdateProductHandler          UpdateOrderItemHandler             │
│  DeleteProductHandler            └─ busca produto → snapshot      │
│                                DeleteOrderItemHandler             │
└───────────┬────────────────────────────┬──────────────────────────┘
            │                            │
┌───────────▼──────────────┐  ┌──────────▼───────────────────────┐
│  Domain — Product        │  │  Domain — Order / OrderItem      │
│                          │  │                                  │
│  id                      │  │  Order                           │
│  name                    │  │    type                          │
│  unitPrice               │  │    subtotal         (stored)     │
│  createdAt               │  │    total            (stored)     │
│                          │  │    discountOrSurcharge (stored)  │
│  Update(name, price)     │  │    RecalculateTotals()           │
└───────────┬──────────────┘  │                                  │
            │                 │  OrderItem  (snapshot)           │
            │                 │    productId                     │
            │                 │    productName  (snapshot)       │
            │                 │    unitPrice    (snapshot)       │
            │                 │    subtotal     (stored)         │
            │                 └──────────┬───────────────────────┘
            │                            │
┌───────────▼────────────────────────────▼──────────────────────────┐
│  Infrastructure  —  EF Core 10 + Npgsql                           │
│  ProductRepository          OrderRepository          AppDbContext  │
└───────────────────────────────┬───────────────────────────────────┘
                                │ TCP 5432
┌───────────────────────────────▼───────────────────────────────────┐
│  PostgreSQL 16                                                    │
│                                                                   │
│  products              orders                 order_items         │
│  ├─ id (PK)            ├─ id (PK)             ├─ id (PK)          │
│  ├─ name               ├─ type                ├─ order_id (FK)    │
│  ├─ unit_price         ├─ subtotal            ├─ product_id       │
│  └─ created_at         ├─ total               ├─ product_name ◄── snapshot
│                        ├─ discount_or_surch.  ├─ unit_price  ◄── snapshot
│                        └─ created_at          ├─ quantity         │
│                                               └─ subtotal         │
└───────────────────────────────────────────────────────────────────┘
```

## Regras de Negócio

| Tipo de Pedido  | Cálculo do Total       |
|-----------------|------------------------|
| `standard`      | subtotal × 1,00        |
| `express`       | subtotal × 1,15 (+15%) |
| `subscription`  | subtotal × 0,90 (-10%) |

**Snapshot de produto:** nome e preço são copiados do cadastro no momento de criar ou atualizar um item. Alterações futuras no produto não afetam pedidos existentes.

**Totais armazenados:** `subtotal`, `discountOrSurcharge` e `total` são calculados no domínio e persistidos no banco. A leitura serve os valores salvos sem recalcular.

## Pré-requisitos

- [Docker](https://www.docker.com/) e Docker Compose
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (apenas para desenvolvimento local)

## Como Executar

### Com Docker Compose (recomendado)

```bash
docker compose up -d
```

A API estará disponível em `http://localhost:8080`.  
Documentação interativa (Scalar): `http://localhost:8080/scalar/v1`

### Desenvolvimento Local

```bash
# Subir apenas o banco de dados
docker compose up db -d

# Rodar a API localmente
dotnet run --project src/Api
```

### Testes

```bash
dotnet test
```

## Exemplos de Uso

### 1. Criar produto

```bash
curl -X POST http://localhost:8080/products \
  -H "Content-Type: application/json" \
  -d '{ "name": "Teclado Mecânico", "unitPrice": 299.90 }'
# → { "id": "uuid", "name": "Teclado Mecânico", "unitPrice": 299.90, "createdAt": "..." }
```

### 2. Criar pedido (referencia produtos por ID)

```bash
curl -X POST http://localhost:8080/orders \
  -H "Content-Type: application/json" \
  -d '{
    "type": "express",
    "items": [
      { "productId": "<uuid-produto>", "quantity": 2 }
    ]
  }'
# → { "id": "<uuid-pedido>" }
```

O snapshot de nome e preço é capturado automaticamente do produto cadastrado.

### 3. Consultar pedido

```bash
curl http://localhost:8080/orders/<uuid-pedido>
# → { "id": "...", "type": "Express", "subtotal": 599.80,
#     "discountOrSurcharge": 89.97, "total": 689.77, "items": [...] }
```

### 4. Atualizar item

```bash
curl -X PUT http://localhost:8080/orders/<orderId>/items/<itemId> \
  -H "Content-Type: application/json" \
  -d '{ "productId": "<uuid-novo-produto>", "quantity": 1 }'
```

### 5. Remover item

```bash
curl -X DELETE http://localhost:8080/orders/<orderId>/items/<itemId>
```

### 6. CRUD de produtos

```bash
curl http://localhost:8080/products                        # lista todos
curl http://localhost:8080/products/<id>                  # consulta um
curl -X PUT http://localhost:8080/products/<id> \
  -H "Content-Type: application/json" \
  -d '{ "name": "Nome Atualizado", "unitPrice": 349.90 }' # atualiza
curl -X DELETE http://localhost:8080/products/<id>        # remove
```

## Estrutura do Projeto

```
pedidos/
├── src/
│   ├── Api/
│   │   ├── Endpoints/          # OrderEndpoints, OrderItemEndpoints, ProductEndpoints
│   │   ├── Filters/            # ValidationFilter<T>
│   │   ├── Validators/
│   │   │   ├── Orders/         # CreateOrderRequestValidator, UpdateOrderItemRequestValidator
│   │   │   └── Products/       # CreateProductRequestValidator, UpdateProductRequestValidator
│   │   └── Program.cs
│   ├── Application/
│   │   ├── Interfaces/         # IOrderRepository, IProductRepository
│   │   ├── Orders/             # CreateOrder, GetOrder, UpdateOrderItem, DeleteOrderItem
│   │   └── Products/           # CreateProduct, GetProduct, GetAllProducts, UpdateProduct, DeleteProduct
│   ├── Domain/
│   │   ├── Entities/           # Order, OrderItem, Product
│   │   └── Enums/              # OrderType
│   └── Infrastructure/
│       └── Persistence/
│           ├── AppDbContext.cs
│           ├── Migrations/
│           └── Repositories/   # OrderRepository, ProductRepository
├── tests/
│   └── Application.Tests/
│       └── Orders/             # OrderCalculationTests, GetOrderHandlerTests, ...
├── docker-compose.yml
├── Dockerfile
└── pedidos.sln
```

## Postman

Importe a collection para testar todos os endpoints com variáveis encadeadas:

1. Abra o Postman → **Import**
2. Selecione o arquivo [`pedidos.postman_collection.json`](pedidos.postman_collection.json)
3. Execute as pastas **na ordem**: 📦 Produtos → 🛒 Pedidos → ✏️ Itens → ⚠️ Validações

Os scripts de teste capturam `productId`, `orderId` e `itemId` automaticamente a cada resposta — não é necessário copiar nenhum ID manualmente.

## Documentação
- [PRD](PRD.md) — Requisitos do produto
- [SDD](SDD.md) — Spec-Driven Development (specs de comportamento + decisões técnicas)
- [TASKS](TASKS.md) — Lista de tarefas de implementação
- [Constitution](constitution.md) — Princípios do projeto
