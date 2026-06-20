# SDD — Spec-Driven Development
## Pedidos API

> As specs abaixo são a **fonte de verdade** do projeto. Implementação e testes derivam delas — não o contrário.

---

## Como usar este documento

1. Cada spec define um comportamento esperado do sistema
2. Os testes unitários/integração devem verificar exatamente o que está descrito aqui
3. Nenhum comportamento existe no código se não estiver especificado aqui primeiro
4. Alterações de comportamento começam com uma alteração neste documento

---

## Spec 0 — Catálogo de Produtos

**Endpoints:** `GET|POST /products` · `GET|PUT|DELETE /products/{id}`

### Contratos

**Entrada (criar/atualizar):**
```json
{ "name": "string (não vazio, max 255)", "unitPrice": "decimal > 0" }
```

**Saída:**
```json
{ "id": "uuid", "name": "string", "unitPrice": 49.90, "createdAt": "ISO8601" }
```

### Comportamentos esperados

| # | Dado | Quando | Então |
|---|------|--------|-------|
| 0.1 | Dados válidos | POST /products | 201 Created + produto criado |
| 0.2 | `name` vazio | POST /products | 400 Bad Request |
| 0.3 | `unitPrice` ≤ 0 | POST /products | 400 Bad Request |
| 0.4 | Produto existente | GET /products/{id} | 200 OK + dados |
| 0.5 | ID inexistente | GET /products/{id} | 404 Not Found |
| 0.6 | Dados válidos | PUT /products/{id} | 200 OK + dados atualizados |
| 0.7 | Atualizar produto | PUT /products/{id} | pedidos existentes mantêm snapshot anterior |
| 0.8 | Produto existente | DELETE /products/{id} | 204 No Content |
| 0.9 | ID inexistente | DELETE /products/{id} | 404 Not Found |

---

## Spec 1 — Criar Pedido

**Endpoint:** `POST /orders`

### Contrato de entrada
```json
{
  "type": "standard | express | subscription",
  "items": [
    { "productId": "uuid", "quantity": "int > 0" }
  ]
}
```

### Comportamentos esperados

| # | Dado | Quando | Então |
|---|------|--------|-------|
| 1.1 | Pedido válido com tipo e itens | POST /orders | 201 Created + `{ "id": "uuid" }` |
| 1.2 | Campo `type` ausente ou inválido | POST /orders | 400 Bad Request (FluentValidation) |
| 1.3 | Lista `items` vazia ou ausente | POST /orders | 400 Bad Request (FluentValidation) |
| 1.4 | Item com `quantity` ≤ 0 | POST /orders | 400 Bad Request (FluentValidation) |
| 1.5 | Item com `productId` inválido | POST /orders | 400 Bad Request (FluentValidation) |
| 1.6 | `productId` não existe no catálogo | POST /orders | 404 Not Found |
| 1.7 | Pedido criado com sucesso | POST /orders | snapshot de nome e preço copiados do produto |
| 1.8 | Pedido criado com sucesso | POST /orders | subtotal, total e discountOrSurcharge armazenados no banco |

---

## Spec 2 — Consultar Resumo do Pedido

**Endpoint:** `GET /orders/{orderId}`

### Contrato de saída
```json
{
  "id": "uuid",
  "type": "Express",
  "subtotal": 100.00,
  "discountOrSurcharge": 15.00,
  "total": 115.00,
  "items": [
    {
      "id": "uuid",
      "productId": "uuid",
      "productName": "Nome do Produto",
      "quantity": 2,
      "unitPrice": 49.90,
      "subtotal": 99.80
    }
  ]
}
```

### Comportamentos esperados

| # | Dado | Quando | Então |
|---|------|--------|-------|
| 2.1 | Pedido `standard` com subtotal R$ 100 | GET /orders/{id} | `total = 100`, `discountOrSurcharge = 0` |
| 2.2 | Pedido `express` com subtotal R$ 100 | GET /orders/{id} | `total = 115`, `discountOrSurcharge = 15` |
| 2.3 | Pedido `subscription` com subtotal R$ 100 | GET /orders/{id} | `total = 90`, `discountOrSurcharge = -10` |
| 2.4 | Pedido com múltiplos itens | GET /orders/{id} | `subtotal = Σ item.subtotal` |
| 2.5 | ID inexistente | GET /orders/{id} | 404 Not Found |
| 2.6 | Qualquer pedido | GET /orders/{id} | valores retornados são os armazenados — não recalculados |

### Fórmulas (inegociáveis — aplicadas em `Order.RecalculateTotals()`)
```
item.subtotal        = item.quantity × item.unitPrice      (armazenado em order_items)
order.subtotal       = Σ item.subtotal                     (armazenado em orders)
total (standard)     = order.subtotal × 1.00               (armazenado em orders)
total (express)      = order.subtotal × 1.15               (armazenado em orders)
total (subscription) = order.subtotal × 0.90               (armazenado em orders)
discountOrSurcharge  = total − subtotal                    (armazenado em orders)
```

---

## Spec 3 — Atualizar Item do Pedido

**Endpoint:** `PUT /orders/{orderId}/items/{itemId}`

### Contrato de entrada
```json
{ "productId": "uuid", "quantity": "int > 0" }
```

### Comportamentos esperados

| # | Dado | Quando | Então |
|---|------|--------|-------|
| 3.1 | Item existente + dados válidos | PUT /orders/{id}/items/{iid} | 200 OK + item atualizado |
| 3.2 | Pedido inexistente | PUT /orders/{id}/items/{iid} | 404 Not Found |
| 3.3 | Item inexistente no pedido | PUT /orders/{id}/items/{iid} | 404 Not Found |
| 3.4 | `productId` não existe no catálogo | PUT /orders/{id}/items/{iid} | 404 Not Found |
| 3.5 | Dados inválidos (`quantity` ≤ 0) | PUT /orders/{id}/items/{iid} | 400 Bad Request |
| 3.6 | Atualização bem-sucedida | PUT /orders/{id}/items/{iid} | novo snapshot de preço capturado + totais do pedido recalculados e persistidos |

---

## Spec 4 — Remover Item do Pedido

**Endpoint:** `DELETE /orders/{orderId}/items/{itemId}`

### Comportamentos esperados

| # | Dado | Quando | Então |
|---|------|--------|-------|
| 4.1 | Item existente | DELETE /orders/{id}/items/{iid} | 204 No Content + totais do pedido recalculados |
| 4.2 | Pedido inexistente | DELETE /orders/{id}/items/{iid} | 404 Not Found |
| 4.3 | Item inexistente no pedido | DELETE /orders/{id}/items/{iid} | 404 Not Found |

---

## Decisões Técnicas

### Stack
- **Runtime:** .NET 10 — Minimal APIs
- **Banco:** PostgreSQL 16 via Docker
- **ORM:** Entity Framework Core 10 (Npgsql)
- **Validação:** FluentValidation 12 com `ValidationFilter<T>` em endpoint filter
- **Documentação:** Scalar (`/scalar/v1`) + `Microsoft.AspNetCore.OpenApi`
- **Testes:** xUnit + FluentAssertions + Moq

### Estrutura de Diretórios
```
pedidos/
├── src/
│   ├── Api/
│   │   ├── Endpoints/
│   │   │   ├── OrderEndpoints.cs
│   │   │   ├── OrderItemEndpoints.cs
│   │   │   └── ProductEndpoints.cs
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   ├── Validators/
│   │   │   ├── Orders/
│   │   │   │   ├── CreateOrderRequestValidator.cs
│   │   │   │   └── UpdateOrderItemRequestValidator.cs
│   │   │   └── Products/
│   │   │       ├── CreateProductRequestValidator.cs
│   │   │       └── UpdateProductRequestValidator.cs
│   │   └── Program.cs
│   ├── Application/
│   │   ├── Interfaces/
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IProductRepository.cs
│   │   ├── Orders/
│   │   │   ├── CreateOrder/
│   │   │   ├── GetOrder/
│   │   │   ├── UpdateOrderItem/
│   │   │   └── DeleteOrderItem/
│   │   └── Products/
│   │       ├── CreateProduct/
│   │       ├── GetProduct/
│   │       ├── GetAllProducts/
│   │       ├── UpdateProduct/
│   │       └── DeleteProduct/
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── Product.cs
│   │   └── Enums/
│   │       └── OrderType.cs
│   └── Infrastructure/
│       └── Persistence/
│           ├── AppDbContext.cs
│           ├── Migrations/
│           └── Repositories/
│               ├── OrderRepository.cs
│               └── ProductRepository.cs
├── tests/
│   └── Application.Tests/
│       └── Orders/
│           ├── OrderCalculationTests.cs
│           ├── GetOrderHandlerTests.cs
│           ├── UpdateOrderItemHandlerTests.cs
│           └── DeleteOrderItemHandlerTests.cs
├── docker-compose.yml
├── Dockerfile
└── pedidos.sln
```

### Modelo de Domínio

`Order.RecalculateTotals()` é chamado em toda mutação — os valores são persistidos, não computados na leitura:

```csharp
private void RecalculateTotals()
{
    Subtotal = _items.Sum(i => i.Subtotal);       // stored
    Total = Type switch
    {
        OrderType.Express      => Subtotal * 1.15m,
        OrderType.Subscription => Subtotal * 0.90m,
        _                      => Subtotal
    };                                             // stored
    DiscountOrSurcharge = Total - Subtotal;        // stored
}
```

`OrderItem` captura snapshot no construtor e no `Update`:
```csharp
public OrderItem(Guid productId, string productName, int quantity, decimal unitPrice)
{
    // ...
    Subtotal = quantity * unitPrice;  // stored
}
```

### Tabelas (PostgreSQL)
```sql
products    (id UUID PK, name VARCHAR(255), unit_price DECIMAL(10,2), created_at TIMESTAMP)

orders      (id UUID PK, type VARCHAR(20), created_at TIMESTAMP,
             subtotal DECIMAL(10,2), discount_or_surcharge DECIMAL(10,2), total DECIMAL(10,2))

order_items (id UUID PK, order_id UUID FK, product_id UUID,
             product_name VARCHAR(255), quantity INT,
             unit_price DECIMAL(10,2), subtotal DECIMAL(10,2))
```

### Validação em duas camadas

| Camada | Responsabilidade | Retorno |
|---|---|---|
| `ValidationFilter<T>` (FluentValidation) | Formato, obrigatoriedade, limites numéricos | 400 antes do handler |
| Domínio / Handler | Invariantes de negócio (produto não existe, item não encontrado) | 404 via exceção |

### Mapeamento Spec → Teste

| Spec | Teste |
|------|-------|
| 2.1  | `Standard_Total_EqualsSubtotal` |
| 2.2  | `Express_Total_IsSubtotalPlus15Percent` |
| 2.3  | `Subscription_Total_IsSubtotalMinus10Percent` |
| 2.4  | `MultipleItems_Subtotal_IsSumOfAllItems` |
| 2.5  | `OrderNotFound_ThrowsKeyNotFoundException` |
| 3.3  | `ItemNotFound_ThrowsKeyNotFoundException` (UpdateOrderItemHandlerTests) |
| 4.3  | `ItemNotFound_ThrowsKeyNotFoundException` (DeleteOrderItemHandlerTests) |
