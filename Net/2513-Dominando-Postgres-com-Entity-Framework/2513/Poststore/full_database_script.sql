CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE categories (
    id serial NOT NULL,
    "Title" varchar NOT NULL,
    CONSTRAINT pk_category PRIMARY KEY (id)
);

CREATE TABLE products (
    id serial NOT NULL,
    "Title" varchar NOT NULL,
    "Slug" varchar NOT NULL,
    created_at_utc timestamp with time zone NOT NULL DEFAULT (now()),
    updated_at_utc timestamp with time zone NOT NULL DEFAULT (now()),
    is_active boolean NOT NULL,
    category_id integer NOT NULL,
    CONSTRAINT pk_product PRIMARY KEY (id),
    CONSTRAINT "FK_products_categories_category_id" FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX idx_products_slug ON products ("Slug");

CREATE INDEX "IX_products_category_id" ON products (category_id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250819234848_v1', '9.0.8');

COMMIT;

