IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE TABLE [Games] (
        [Id] uniqueidentifier NOT NULL,
        [RoomCode] nvarchar(10) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [CurrentPlayerIndex] int NOT NULL,
        [RoundNumber] int NOT NULL,
        [RollNumber] int NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [UpdatedUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE TABLE [Dice] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [Position] int NOT NULL,
        [Value] int NOT NULL,
        [IsHeld] bit NOT NULL,
        CONSTRAINT [PK_Dice] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Dice_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE TABLE [Players] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [DisplayName] nvarchar(50) NOT NULL,
        [IsConnected] bit NOT NULL,
        [JoinOrder] int NOT NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Players_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Games] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE TABLE [ScoreEntries] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [Category] nvarchar(30) NOT NULL,
        [Points] int NOT NULL,
        CONSTRAINT [PK_ScoreEntries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScoreEntries_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Dice_GameId] ON [Dice] ([GameId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Games_RoomCode] ON [Games] ([RoomCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Players_GameId] ON [Players] ([GameId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ScoreEntries_PlayerId] ON [ScoreEntries] ([PlayerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417114805_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417114805_InitialCreate', N'9.0.0');
END;

COMMIT;
GO

