# PRD — Product Requirements Document
## Pedidos API

---

## 1. Visão Geral
API REST para gerenciamento de pedidos de uma loja online. Mantém um catálogo de produtos e permite criar pedidos referenciando esses produtos. O snapshot de nome e preço é capturado no momento da criação do pedido, garantindo que alterações futuras no produto não afetem pedidos já registrados.

## 2. Objetivos
- Manter catálogo de produtos com nome e preço
- Criar pedidos referenciando produtos por ID com snapshot automático
- Calcular e armazenar subtotal, total e desconto/acréscimo no momento da criação
- Permitir atualização e remoção de itens de um pedido existente
- Validar todas as entradas com mensagens de erro claras

## 3. Tipos de Pedido e Regras de Desconto

| Tipo           | Regra                              | Impacto no Total |
|----------------|------------------------------------|------------------|
| `standard`     | Sem desconto                       | 100% do subtotal |
| `express`      | Acréscimo de 15% (entrega rápida)  | 115% do subtotal |
| `subscription` | Desconto de 10% (cliente assinante)| 90% do subtotal  |

## 4. Endpoints

### 4.1 Criar Produto
```
POST /products
```
**Body:**
```json
{ "name": "Nome do Produto", "unitPrice": 49.90 }
```
**Retorno:** `201 Created`
```json
{ "id": "uuid", "name": "Nome do Produto", "unitPrice": 49.90, "createdAt": "2026-06-20T00:00:00Z" }
```

### 4.2 Listar Produtos
```
GET /products
```
**Retorno:** `200 OK` — array de produtos ordenados por nome

### 4.3 Consultar Produto
```
GET /products/{id}
```
**Retorno:** `200 OK` com dados do produto ou `404 Not Found`

### 4.4 Atualizar Produto
```
PUT /products/{id}
```
**Body:**
```json
{ "name": "Nome Atualizado", "unitPrice": 59.90 }
```
**Retorno:** `200 OK` com dados atualizados

> Atualizar um produto não altera pedidos já criados (snapshot preservado).

### 4.5 Remover Produto
```
DELETE /products/{id}
```
**Retorno:** `204 No Content`

---

### 4.6 Criar Pedido
```
POST /orders
```
**Body:**
```json
{
  "type": "standard | express | subscription",
  "items": [
    { "productId": "uuid", "quantity": 2 }
  ]
}
```
**Retorno:** `201 Created`
```json
{ "id": "uuid" }
```

> O sistema busca o produto pelo `productId`, captura snapshot de nome e preço, e calcula e persiste os totais.

### 4.7 Consultar Resumo do Pedido
```
GET /orders/{orderId}
```
**Retorno:** `200 OK`
```json
{
  "id": "uuid",
  "type": "express",
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

> `subtotal`, `discountOrSurcharge` e `total` são valores armazenados — não recalculados na leitura.

### 4.8 Atualizar Item do Pedido
```
PUT /orders/{orderId}/items/{itemId}
```
**Body:**
```json
{ "productId": "uuid", "quantity": 3 }
```
**Retorno:** `200 OK` com item atualizado

> Novo snapshot de nome e preço é capturado do produto. Totais do pedido são recalculados e persistidos.

### 4.9 Remover Item do Pedido
```
DELETE /orders/{orderId}/items/{itemId}
```
**Retorno:** `204 No Content`

## 5. Regras de Validação

### Produtos
- `name` obrigatório, máximo 255 caracteres
- `unitPrice` maior que zero

### Pedidos
- `type` obrigatório — `standard`, `express` ou `subscription`
- `items` deve conter ao menos 1 item
- `productId` obrigatório por item
- `quantity` maior que zero por item

## 6. Cenários de Erro
| Situação                     | HTTP Status     |
|------------------------------|-----------------|
| Campos inválidos (validação) | 400 Bad Request |
| Tipo de pedido inválido      | 400 Bad Request |
| Lista de itens vazia         | 400 Bad Request |
| Produto não encontrado       | 404 Not Found   |
| Pedido não encontrado        | 404 Not Found   |
| Item não encontrado          | 404 Not Found   |

## 7. Critérios de Aceite
- [x] POST /products cria produto e retorna dados completos
- [x] GET /products lista produtos ordenados por nome
- [x] PUT /products/{id} atualiza sem afetar pedidos existentes
- [x] DELETE /products/{id} remove produto
- [x] POST /orders cria pedido com snapshot automático e retorna ID
- [x] GET /orders/{id} retorna totais armazenados corretamente para cada tipo
- [x] PUT /orders/{id}/items/{itemId} atualiza item com novo snapshot e recalcula totais
- [x] DELETE /orders/{id}/items/{itemId} remove item e recalcula totais
- [x] Validações retornam 400 com mensagem clara antes de chegar ao handler
- [x] Testes unitários cobrem os três tipos de pedido com cálculo correto
