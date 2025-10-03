## Introdução
Olá e seja bem-vindo(a) ao curso **Dominando Postgres com Entity Framework**! Eu sou André Baltieri, ou balta, 13x Microsoft MVP, e vou te acompanhar nesta jornada prática para integrar **Postgres** de ponta a ponta em aplicações **.NET** usando **Entity Framework Core (Npgsql)** — com mapeamentos eficientes, migrações seguras e recursos modernos como **JSONB**, **Value Objects** e **Identity**.

## O problema
Ligar o mundo relacional do Postgres ao modelo de domínio em .NET nem sempre é trivial. Sem uma boa estratégia de mapeamento, **migrações podem quebrar**, **consultas ficam lentas**, **value objects viram gambiarra** e recursos poderosos como **JSONB** ficam subutilizados. Além disso, integrar **ASP.NET Identity** ao Postgres exige atenção a chaves, nomes e convenções — e reproduzir tudo isso em times e ambientes requer uma base sólida e **reprodutível com Docker**.

## O que vamos aprender?
Neste curso, você vai dominar o uso do **Postgres com EF Core** do zero ao avançado, sempre com foco em **boas práticas**, **performance** e **manutenibilidade**:

* **Instalação e configuração do Postgres + EF Core (Npgsql)** no seu ambiente, com convenções úteis (snake\_case, schemas, etc.).
* **Modelagem e criação dos modelos** de domínio com foco em clareza, relacionamentos e consistência.
* **Migrations na vida real**: criar, versionar, aplicar, revisar e reverter com segurança (inclusive scripts idempotentes).
* **Mapeamento via Fluent API**: chaves, composições, relacionamentos, restrições e conversores.
* **Mapeamento avançado**: owned types para **Value Objects**, conversões personalizadas, colunas geradas e defaults.
* **JSON/JSONB na prática**: armazenar documentos, mapear para tipos .NET e **indexar para performance**.
* **Filtragem de dados em JSON** usando operadores do Postgres (e projeções LINQ) com foco em velocidade.
* **Localização**: estratégias para dados multi-idioma (tabelas auxiliares/JSON), ordenação e buscas insensíveis a acentos.
* **ASP.NET Identity + Postgres**: estrutura de tabelas, chaves e **mapeamento do Identity** com EF Core.
* **Ambiente reprodutível com Docker Compose**: subir Postgres/pgAdmin, aplicar migrations e padronizar o setup do time.

## Para quem é este curso?
Este curso é para **desenvolvedores .NET** que desejam:
* **Sair do básico** e usar Postgres com **EF Core de forma profissional**.
* **Aproveitar recursos modernos do Postgres** (JSONB, índices, convenções) sem abrir mão da produtividade do EF.
* **Modelar domínios ricos** com **Value Objects** e mapeamentos avançados.
* **Integrar Identity** e preparar um **ambiente reprodutível** para times e CI/CD com **Docker Compose**.
