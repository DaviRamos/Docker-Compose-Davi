\c my_grafana_db

-- Criar a Tabela

CREATE TABLE "my_table" (
    "person" varchar(100) NOT NULL,
    "apples" smallint NOT NULL
);


\c my_grafana_db

-- Popular Dados

INSERT INTO my_table ("person", "apples") 
values('Anne', 10);

INSERT INTO my_table ("person", "apples") 
VALUES ('Jane', 15);

INSERT INTO my_table ("person", "apples") 
VALUES ('Jack', 25);

INSERT INTO my_table ("person", "apples") 
VALUES ('Linda', 35);

