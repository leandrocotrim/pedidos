# Constitution — Pedidos API

## Propósito
Este documento define os princípios inegociáveis que governam todas as decisões de design, implementação e evolução deste projeto.

## Princípios Fundamentais

### 1. Simplicidade acima de tudo
Prefira a solução mais simples que atenda ao requisito. Não antecipe necessidades futuras sem evidência concreta.

### 2. Regras de negócio no domínio
Lógica de cálculo (desconto, acréscimo) vive em entidades ou serviços de domínio — nunca em controllers ou handlers de rota.

### 3. Testabilidade por design
Toda lógica de negócio deve ser testável sem dependências externas (banco, HTTP). Use injeção de dependência e interfaces.

### 4. Contratos explícitos
Requests e responses são representados por DTOs tipados. Nenhuma entidade de domínio exposta diretamente na API.

### 5. Falha rápida e clara
Validações estruturais (formato, obrigatoriedade, limites) são responsabilidade do FluentValidation e retornam `400 Bad Request` antes de chegar ao handler. Recursos não encontrados retornam `404 Not Found`. Nunca exponha stack traces.

### 6. Imutabilidade do pedido como regra de ouro
Apenas itens podem ser atualizados ou removidos após a criação. O tipo e a identidade do pedido são imutáveis.

### 7. Totais calculados e armazenados na criação
`Subtotal`, `Total` e `DiscountOrSurcharge` são calculados no domínio (`Order.RecalculateTotals()`) e persistidos. Leituras nunca recalculam — servem o valor armazenado.

### 8. Snapshot de produto no pedido
Ao criar ou atualizar um item, nome e preço são copiados do produto para o item. Alterações futuras no produto não afetam pedidos já criados.

### 9. Validação em duas camadas
- **FluentValidation (Api):** valida formato, obrigatoriedade e limites antes do handler
- **Domínio:** valida invariantes de negócio (ex: item não encontrado no pedido)

## Regras de Desconto (inegociáveis)
| Tipo           | Regra                         |
|----------------|-------------------------------|
| `standard`     | Sem desconto                  |
| `express`      | Acréscimo de 15% (taxa)       |
| `subscription` | Desconto de 10%               |

## O que NÃO fazer
- Não recalcular totais na leitura — use os valores armazenados
- Não expor entidades de domínio diretamente na API
- Não usar `dynamic` ou `object` como tipo de retorno
- Não colocar lógica de negócio em migrations ou stored procedures
- Não ignorar erros de concorrência no banco
- Não alterar preço do snapshot ao atualizar o produto — apenas novos pedidos/itens capturam o preço atual

## Evoluções Planejadas
- FluentValidation: ✅ implementado
- Catálogo de produtos: ✅ implementado
- Snapshot de produto no pedido: ✅ implementado
- Totais armazenados: ✅ implementado

## Relacionamentos
- Veja o [PRD](PRD.md) para requisitos de produto
- Veja o [SDD](SDD.md) para specs de comportamento e decisões técnicas
