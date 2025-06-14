CREATE DATABASE litellmdb;
CREATE USER userLitellmDB WITH ENCRYPTED PASSWORD 'litellmdb';
GRANT ALL PRIVILEGES ON DATABASE litellmdb TO userLitellmDB;

\c postgres

CREATE TABLE "Regioes" (
    "IdRegiao" INT NOT NULL,
    "CodRegiao" VARCHAR(2) NOT NULL,
    "NomeRegiao" VARCHAR(20) NOT NULL,
    CONSTRAINT "PK_Regioes" PRIMARY KEY ("IdRegiao")
);

CREATE TABLE "Estados" (
    "SiglaEstado" CHAR(2) NOT NULL,
    "NomeEstado" VARCHAR(20) NOT NULL,
    "NomeCapital" VARCHAR(20) NOT NULL,
    "IdRegiao" INT NOT NULL,
    CONSTRAINT "PK_Estados" PRIMARY KEY ("SiglaEstado"),
    CONSTRAINT "FK_Estado_Regiao" FOREIGN KEY ("IdRegiao") REFERENCES "Regioes" ("IdRegiao")
);


\c postgres

-- Cadastramento de Regioes

INSERT INTO "Regioes"("IdRegiao", "CodRegiao", "NomeRegiao")
VALUES (1, 'CO', 'Centro-Oeste');

INSERT INTO "Regioes" ("IdRegiao", "CodRegiao", "NomeRegiao")
VALUES (2, 'NE', 'Nordeste');

INSERT INTO "Regioes" ("IdRegiao", "CodRegiao", "NomeRegiao")
VALUES (3, 'N', 'Norte');

INSERT INTO "Regioes" ("IdRegiao", "CodRegiao", "NomeRegiao")
VALUES (4, 'SE', 'Sudeste');

INSERT INTO "Regioes" ("IdRegiao", "CodRegiao", "NomeRegiao")
VALUES (5, 'S', 'Sul');


-- Cadastramento de Estados

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('AC', 'Acre', 'Rio Branco', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('AL', 'Alagoas', 'Maceió', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('AP', 'Amapá', 'Macapá', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('AM', 'Amazonas', 'Manaus', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('BA', 'Bahia', 'Salvador', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('CE', 'Ceará', 'Fortaleza', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('DF', 'Distrito Federal', 'Brasília', 1);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('ES', 'Espírito Santo', 'Vitória', 4);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('GO', 'Goiás', 'Goiânia', 1);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('MA', 'Maranhão', 'São Luís', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('MT', 'Mato Grosso', 'Cuiabá', 1);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('MS', 'Mato Grosso do Sul', 'Campo Grande', 1);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('MG', 'Minas Gerais', 'Belo Horizonte', 4);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('PA', 'Pará', 'Belém', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('PB', 'Paraíba', 'João Pessoa', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('PR', 'Paraná', 'Curitiba', 5);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('PE', 'Pernambuco', 'Recife', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('PI', 'Piauí', 'Teresina', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('RJ', 'Rio de Janeiro', 'Rio de Janeiro', 4);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('RN', 'Rio Grande do Norte', 'Natal', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('RS', 'Rio Grande do Sul', 'Porto Alegre', 5);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('RO', 'Rondônia', 'Porto Velho', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('RR', 'Roraima', 'Boa Vista', 3);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('SC', 'Santa Catarina', 'Florianópolis', 5);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('SP', 'São Paulo', 'São Paulo', 4);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('SE', 'Sergipe', 'Aracaju', 2);

INSERT INTO "Estados" ("SiglaEstado", "NomeEstado", "NomeCapital", "IdRegiao")
VALUES ('TO', 'Tocantins', 'Palmas', 3);