CREATE TABLE "Users"
(
    "Id"           SERIAL PRIMARY KEY,
    "Email"        TEXT      NOT NULL,
    "FullName"     TEXT      NOT NULL,
    "PasswordSalt" BYTEA     NOT NULL,
    "PasswordHash" BYTEA     NOT NULL,
    "UserType"     SMALLINT  NOT NULL,
    "CreatedAt"    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"    TIMESTAMP,
    "IsDeleted"    BOOLEAN   NOT NULL DEFAULT FALSE,
    CONSTRAINT "uq_users_email" UNIQUE ("Email")
);

CREATE INDEX "idx_users_email" ON "Users" ("Email") WHERE "IsDeleted" = FALSE;

CREATE TABLE "UserTokens"
(
    "Token"      TEXT PRIMARY KEY,
    "TokenType"  SMALLINT  NOT NULL,
    "Expiration" TIMESTAMP NOT NULL,
    "UserId"     INT       NOT NULL,
    CONSTRAINT "fk_user" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id"),
    CONSTRAINT "uq_token" UNIQUE ("Token")
);

CREATE INDEX "idx_usertokens_userid" ON "UserTokens" ("UserId");
