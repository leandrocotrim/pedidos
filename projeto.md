# Contexto
Você deve implementar uma pequena API para gerenciar pedidos de uma loja online, permitindo criar pedidos com itens e consultar o resumo com o valor total calculado.

# Entrega
- Crie um repositório público no GitHub com o código desenvolvido.
- Compartilhe o link do repositório para avaliação.

# Requisitos técnicos

## Tipos de pedido e regras de desconto
Cada pedido possui um tipo que determina como o valor total é calculado:

| Tipo | Regra |
|---|---|
| `standard` | Sem desconto |
| `express` | Acréscimo de 15% (taxa de entrega rápida) |
| `subscription` | Desconto de 10% (cliente assinante) |

## Banco de dados
- Utilize banco de dados em memória ou via Docker.

## Testes
- Implemente testes unitários para a camada de serviço.
- Cubra os casos: criação de pedido `standard`, `express` e `subscription`, e cálculo do total com desconto correto.

# Durante o livecode
Inicie pela **criação do pedido** e pela **consulta do resumo** — esses endpoints serão avaliados na sessão ao vivo.

# Endpoints
POST /orders
    Corpo: tipo do pedido + lista de itens (produto, quantidade, preço)
    Retorno: ID do pedido criado

GET /orders/{orderId}
    Retorna o resumo do pedido com valor total e desconto aplicado

# Missão até 23:59
Implemente também os endpoints de atualização e remoção de itens:

PUT /orders/{orderId}/items/{itemId}
DELETE /orders/{orderId}/items/{itemId}