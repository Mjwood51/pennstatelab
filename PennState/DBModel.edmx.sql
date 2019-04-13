
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/05/2019 14:40:29
-- Generated from EDMX file: C:\Users\Mark W\source\repos\PennState\PennState\Models\DBModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PennStateDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_CatagoryLocations_dbo_Tbl_CatagoryLocations_Pid]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_CatagoryLocations] DROP CONSTRAINT [FK_dbo_Tbl_CatagoryLocations_dbo_Tbl_CatagoryLocations_Pid];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_CatagoryOwners_dbo_Tbl_CatagoryOwners_Pid]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_CatagoryOwners] DROP CONSTRAINT [FK_dbo_Tbl_CatagoryOwners_dbo_Tbl_CatagoryOwners_Pid];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_CatagoryTypes_dbo_Tbl_CatagoryTypes_Pid]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_CatagoryTypes] DROP CONSTRAINT [FK_dbo_Tbl_CatagoryTypes_dbo_Tbl_CatagoryTypes_Pid];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_CatagoryVendors_dbo_Tbl_CatagoryVendors_Pid]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_CatagoryVendors] DROP CONSTRAINT [FK_dbo_Tbl_CatagoryVendors_dbo_Tbl_CatagoryVendors_Pid];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_Items_dbo_Tbl_Locations_LocId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Items] DROP CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_Locations_LocId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_Items_dbo_Tbl_SubLocations_SubId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Items] DROP CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_SubLocations_SubId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_Items_dbo_Tbl_Users_UsrId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Items] DROP CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_Users_UsrId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_SubLocations_dbo_Tbl_Locations_LocId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_SubLocations] DROP CONSTRAINT [FK_dbo_Tbl_SubLocations_dbo_Tbl_Locations_LocId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Tbl_Users_dbo_Tbl_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_Users] DROP CONSTRAINT [FK_dbo_Tbl_Users_dbo_Tbl_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_Tbl_ItemFiles_Tbl_File]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_ItemFiles] DROP CONSTRAINT [FK_Tbl_ItemFiles_Tbl_File];
GO
IF OBJECT_ID(N'[dbo].[FK_Tbl_ItemFiles_Tbl_Items]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_ItemFiles] DROP CONSTRAINT [FK_Tbl_ItemFiles_Tbl_Items];
GO
IF OBJECT_ID(N'[dbo].[FK_Tbl_PhotoItems_Tbl_Items]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_PhotoItems] DROP CONSTRAINT [FK_Tbl_PhotoItems_Tbl_Items];
GO
IF OBJECT_ID(N'[dbo].[FK_Tbl_PhotoItems_Tbl_Photo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tbl_PhotoItems] DROP CONSTRAINT [FK_Tbl_PhotoItems_Tbl_Photo];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Tbl_CatagoryLocations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_CatagoryLocations];
GO
IF OBJECT_ID(N'[dbo].[Tbl_CatagoryOwners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_CatagoryOwners];
GO
IF OBJECT_ID(N'[dbo].[Tbl_CatagoryTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_CatagoryTypes];
GO
IF OBJECT_ID(N'[dbo].[Tbl_CatagoryVendors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_CatagoryVendors];
GO
IF OBJECT_ID(N'[dbo].[Tbl_File]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_File];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Items]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Items];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Locations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Locations];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Photo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Photo];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Roles];
GO
IF OBJECT_ID(N'[dbo].[Tbl_SubLocations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_SubLocations];
GO
IF OBJECT_ID(N'[dbo].[Tbl_Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_Users];
GO
IF OBJECT_ID(N'[dbo].[Tbl_ItemFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_ItemFiles];
GO
IF OBJECT_ID(N'[dbo].[Tbl_PhotoItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tbl_PhotoItems];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Tbl_CatagoryLocations'
CREATE TABLE [dbo].[Tbl_CatagoryLocations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LocationName] nvarchar(50)  NULL,
    [Pid] int  NULL,
    [HasChildren] bit  NOT NULL
);
GO

-- Creating table 'Tbl_CatagoryOwners'
CREATE TABLE [dbo].[Tbl_CatagoryOwners] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OwnerName] nvarchar(50)  NULL,
    [Pid] int  NULL,
    [HasChildren] bit  NOT NULL
);
GO

-- Creating table 'Tbl_CatagoryTypes'
CREATE TABLE [dbo].[Tbl_CatagoryTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TypeName] nvarchar(50)  NULL,
    [Pid] int  NULL,
    [HasChildren] bit  NOT NULL
);
GO

-- Creating table 'Tbl_CatagoryVendors'
CREATE TABLE [dbo].[Tbl_CatagoryVendors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VendorName] nvarchar(50)  NULL,
    [Pid] int  NULL,
    [HasChildren] bit  NOT NULL
);
GO

-- Creating table 'Tbl_File'
CREATE TABLE [dbo].[Tbl_File] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemFileName] varchar(50)  NULL,
    [DataStream] varbinary(max)  NULL
);
GO

-- Creating table 'Tbl_Items'
CREATE TABLE [dbo].[Tbl_Items] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ItemName] nvarchar(50)  NOT NULL,
    [AmountInStock] int  NULL,
    [LocationComments] nvarchar(255)  NULL,
    [Manufacturer] nvarchar(50)  NULL,
    [CatalogNumber] nvarchar(25)  NULL,
    [WebAddress] nvarchar(350)  NULL,
    [Vendor] nvarchar(50)  NULL,
    [ContactInfo] nvarchar(200)  NULL,
    [PurchaseDate] datetime  NULL,
    [Added] datetime  NULL,
    [Updated] datetime  NULL,
    [PurchasePrice] decimal(18,2)  NULL,
    [ItemType] nvarchar(50)  NOT NULL,
    [ItemNotes] nvarchar(255)  NULL,
    [UpdatedBy] nvarchar(255)  NULL,
    [IsDeleted] bit  NOT NULL,
    [UsrId] int  NULL,
    [LocId] int  NULL,
    [SubId] int  NULL
);
GO

-- Creating table 'Tbl_Locations'
CREATE TABLE [dbo].[Tbl_Locations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LocationName] nvarchar(50)  NULL
);
GO

-- Creating table 'Tbl_Photo'
CREATE TABLE [dbo].[Tbl_Photo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PhotoName] varchar(50)  NULL,
    [DataStream] varbinary(max)  NULL
);
GO

-- Creating table 'Tbl_Roles'
CREATE TABLE [dbo].[Tbl_Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleName] nvarchar(10)  NULL
);
GO

-- Creating table 'Tbl_SubLocations'
CREATE TABLE [dbo].[Tbl_SubLocations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SubLocationName] nvarchar(50) NULL,
    [LocId] int  NULL
);
GO

-- Creating table 'Tbl_Users'
CREATE TABLE [dbo].[Tbl_Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(50)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(30)  NOT NULL,
    [PasswordHashed] nvarchar(60)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [ActivationCode] uniqueidentifier  NOT NULL,
    [RoleId] int  NOT NULL
);
GO

-- Creating table 'Tbl_ItemFiles'
CREATE TABLE [dbo].[Tbl_ItemFiles] (
    [Files_Id] int  NOT NULL,
    [Items_Id] int  NOT NULL
);
GO

-- Creating table 'Tbl_PhotoItems'
CREATE TABLE [dbo].[Tbl_PhotoItems] (
    [Items_Id] int  NOT NULL,
    [Photos_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Tbl_CatagoryLocations'
ALTER TABLE [dbo].[Tbl_CatagoryLocations]
ADD CONSTRAINT [PK_Tbl_CatagoryLocations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_CatagoryOwners'
ALTER TABLE [dbo].[Tbl_CatagoryOwners]
ADD CONSTRAINT [PK_Tbl_CatagoryOwners]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_CatagoryTypes'
ALTER TABLE [dbo].[Tbl_CatagoryTypes]
ADD CONSTRAINT [PK_Tbl_CatagoryTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_CatagoryVendors'
ALTER TABLE [dbo].[Tbl_CatagoryVendors]
ADD CONSTRAINT [PK_Tbl_CatagoryVendors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_File'
ALTER TABLE [dbo].[Tbl_File]
ADD CONSTRAINT [PK_Tbl_File]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_Items'
ALTER TABLE [dbo].[Tbl_Items]
ADD CONSTRAINT [PK_Tbl_Items]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_Locations'
ALTER TABLE [dbo].[Tbl_Locations]
ADD CONSTRAINT [PK_Tbl_Locations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_Photo'
ALTER TABLE [dbo].[Tbl_Photo]
ADD CONSTRAINT [PK_Tbl_Photo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_Roles'
ALTER TABLE [dbo].[Tbl_Roles]
ADD CONSTRAINT [PK_Tbl_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_SubLocations'
ALTER TABLE [dbo].[Tbl_SubLocations]
ADD CONSTRAINT [PK_Tbl_SubLocations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tbl_Users'
ALTER TABLE [dbo].[Tbl_Users]
ADD CONSTRAINT [PK_Tbl_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Files_Id], [Items_Id] in table 'Tbl_ItemFiles'
ALTER TABLE [dbo].[Tbl_ItemFiles]
ADD CONSTRAINT [PK_Tbl_ItemFiles]
    PRIMARY KEY CLUSTERED ([Files_Id], [Items_Id] ASC);
GO

-- Creating primary key on [Items_Id], [Photos_Id] in table 'Tbl_PhotoItems'
ALTER TABLE [dbo].[Tbl_PhotoItems]
ADD CONSTRAINT [PK_Tbl_PhotoItems]
    PRIMARY KEY CLUSTERED ([Items_Id], [Photos_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Pid] in table 'Tbl_CatagoryLocations'
ALTER TABLE [dbo].[Tbl_CatagoryLocations]
ADD CONSTRAINT [FK_dbo_Tbl_CatagoryLocations_dbo_Tbl_CatagoryLocations_Pid]
    FOREIGN KEY ([Pid])
    REFERENCES [dbo].[Tbl_CatagoryLocations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_CatagoryLocations_dbo_Tbl_CatagoryLocations_Pid'
CREATE INDEX [IX_FK_dbo_Tbl_CatagoryLocations_dbo_Tbl_CatagoryLocations_Pid]
ON [dbo].[Tbl_CatagoryLocations]
    ([Pid]);
GO

-- Creating foreign key on [Pid] in table 'Tbl_CatagoryOwners'
ALTER TABLE [dbo].[Tbl_CatagoryOwners]
ADD CONSTRAINT [FK_dbo_Tbl_CatagoryOwners_dbo_Tbl_CatagoryOwners_Pid]
    FOREIGN KEY ([Pid])
    REFERENCES [dbo].[Tbl_CatagoryOwners]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_CatagoryOwners_dbo_Tbl_CatagoryOwners_Pid'
CREATE INDEX [IX_FK_dbo_Tbl_CatagoryOwners_dbo_Tbl_CatagoryOwners_Pid]
ON [dbo].[Tbl_CatagoryOwners]
    ([Pid]);
GO

-- Creating foreign key on [Pid] in table 'Tbl_CatagoryTypes'
ALTER TABLE [dbo].[Tbl_CatagoryTypes]
ADD CONSTRAINT [FK_dbo_Tbl_CatagoryTypes_dbo_Tbl_CatagoryTypes_Pid]
    FOREIGN KEY ([Pid])
    REFERENCES [dbo].[Tbl_CatagoryTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_CatagoryTypes_dbo_Tbl_CatagoryTypes_Pid'
CREATE INDEX [IX_FK_dbo_Tbl_CatagoryTypes_dbo_Tbl_CatagoryTypes_Pid]
ON [dbo].[Tbl_CatagoryTypes]
    ([Pid]);
GO

-- Creating foreign key on [Pid] in table 'Tbl_CatagoryVendors'
ALTER TABLE [dbo].[Tbl_CatagoryVendors]
ADD CONSTRAINT [FK_dbo_Tbl_CatagoryVendors_dbo_Tbl_CatagoryVendors_Pid]
    FOREIGN KEY ([Pid])
    REFERENCES [dbo].[Tbl_CatagoryVendors]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_CatagoryVendors_dbo_Tbl_CatagoryVendors_Pid'
CREATE INDEX [IX_FK_dbo_Tbl_CatagoryVendors_dbo_Tbl_CatagoryVendors_Pid]
ON [dbo].[Tbl_CatagoryVendors]
    ([Pid]);
GO

-- Creating foreign key on [LocId] in table 'Tbl_Items'
ALTER TABLE [dbo].[Tbl_Items]
ADD CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_Locations_LocId]
    FOREIGN KEY ([LocId])
    REFERENCES [dbo].[Tbl_Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_Items_dbo_Tbl_Locations_LocId'
CREATE INDEX [IX_FK_dbo_Tbl_Items_dbo_Tbl_Locations_LocId]
ON [dbo].[Tbl_Items]
    ([LocId]);
GO

-- Creating foreign key on [SubId] in table 'Tbl_Items'
ALTER TABLE [dbo].[Tbl_Items]
ADD CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_SubLocations_SubId]
    FOREIGN KEY ([SubId])
    REFERENCES [dbo].[Tbl_SubLocations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_Items_dbo_Tbl_SubLocations_SubId'
CREATE INDEX [IX_FK_dbo_Tbl_Items_dbo_Tbl_SubLocations_SubId]
ON [dbo].[Tbl_Items]
    ([SubId]);
GO

-- Creating foreign key on [UsrId] in table 'Tbl_Items'
ALTER TABLE [dbo].[Tbl_Items]
ADD CONSTRAINT [FK_dbo_Tbl_Items_dbo_Tbl_Users_UsrId]
    FOREIGN KEY ([UsrId])
    REFERENCES [dbo].[Tbl_Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_Items_dbo_Tbl_Users_UsrId'
CREATE INDEX [IX_FK_dbo_Tbl_Items_dbo_Tbl_Users_UsrId]
ON [dbo].[Tbl_Items]
    ([UsrId]);
GO

-- Creating foreign key on [LocId] in table 'Tbl_SubLocations'
ALTER TABLE [dbo].[Tbl_SubLocations]
ADD CONSTRAINT [FK_dbo_Tbl_SubLocations_dbo_Tbl_Locations_LocId]
    FOREIGN KEY ([LocId])
    REFERENCES [dbo].[Tbl_Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_SubLocations_dbo_Tbl_Locations_LocId'
CREATE INDEX [IX_FK_dbo_Tbl_SubLocations_dbo_Tbl_Locations_LocId]
ON [dbo].[Tbl_SubLocations]
    ([LocId]);
GO

-- Creating foreign key on [RoleId] in table 'Tbl_Users'
ALTER TABLE [dbo].[Tbl_Users]
ADD CONSTRAINT [FK_dbo_Tbl_Users_dbo_Tbl_RoleId]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Tbl_Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Tbl_Users_dbo_Tbl_RoleId'
CREATE INDEX [IX_FK_dbo_Tbl_Users_dbo_Tbl_RoleId]
ON [dbo].[Tbl_Users]
    ([RoleId]);
GO

-- Creating foreign key on [Files_Id] in table 'Tbl_ItemFiles'
ALTER TABLE [dbo].[Tbl_ItemFiles]
ADD CONSTRAINT [FK_Tbl_ItemFiles_Tbl_File]
    FOREIGN KEY ([Files_Id])
    REFERENCES [dbo].[Tbl_File]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Items_Id] in table 'Tbl_ItemFiles'
ALTER TABLE [dbo].[Tbl_ItemFiles]
ADD CONSTRAINT [FK_Tbl_ItemFiles_Tbl_Items]
    FOREIGN KEY ([Items_Id])
    REFERENCES [dbo].[Tbl_Items]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tbl_ItemFiles_Tbl_Items'
CREATE INDEX [IX_FK_Tbl_ItemFiles_Tbl_Items]
ON [dbo].[Tbl_ItemFiles]
    ([Items_Id]);
GO

-- Creating foreign key on [Items_Id] in table 'Tbl_PhotoItems'
ALTER TABLE [dbo].[Tbl_PhotoItems]
ADD CONSTRAINT [FK_Tbl_PhotoItems_Tbl_Items]
    FOREIGN KEY ([Items_Id])
    REFERENCES [dbo].[Tbl_Items]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Photos_Id] in table 'Tbl_PhotoItems'
ALTER TABLE [dbo].[Tbl_PhotoItems]
ADD CONSTRAINT [FK_Tbl_PhotoItems_Tbl_Photo]
    FOREIGN KEY ([Photos_Id])
    REFERENCES [dbo].[Tbl_Photo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tbl_PhotoItems_Tbl_Photo'
CREATE INDEX [IX_FK_Tbl_PhotoItems_Tbl_Photo]
ON [dbo].[Tbl_PhotoItems]
    ([Photos_Id]);
GO

INSERT INTO [dbo].[Tbl_Roles] (RoleName)
VALUES('Admin');
GO

INSERT INTO [dbo].[Tbl_Roles] (RoleName)
VALUES('User');
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------