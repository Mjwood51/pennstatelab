CREATE PROCEDURE SaveItem
@Id int,
@ItemName nvarchar(50),
@AmountInStock int,
@LocationComments nvarchar(255),
@Manufacturer nvarchar(50),
@CatalogNumber nvarchar(25),
@WebAddress nvarchar(50),
@Vendor nvarchar(50),
@ContactInfo nvarchar(200),
@PurchaseDate datetime,
@Added datetime,
@Updated datetime,
@PurchasePrice decimal(18,2),
@ItemType nvarchar(20),
@ItemNotes nvarchar(255),
@UpdatedBy nvarchar(255),
@IsDeleted bit,
@UsrId int,
@LocId int,
@SubId int
as 
Begin
Update Tbl_Items Set ItemName = @ItemName, AmountInStock = @AmountInStock, LocationComments = @LocationComments, Manufacturer = @Manufacturer,
						CatalogNumber = @CatalogNumber, WebAddress = @WebAddress, Vendor = @Vendor, ContactInfo = @ContactInfo, PurchaseDate = @PurchaseDate,
						Added = @Added, Updated = @Updated, PurchasePrice = @PurchasePrice, ItemType = @ItemType, ItemNotes = @ItemNotes, UpdatedBy = @UpdatedBy,
						IsDeleted = @IsDeleted, UsrId = @UsrId, LocId = @LocId, SubId = @SubId
			     Where Id = @Id
END