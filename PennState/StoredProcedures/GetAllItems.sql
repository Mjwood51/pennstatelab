CREATE PROCEDURE [dbo].[GetAllItems] as Begin
Select 
Id,
ItemName,
AmountInStock,
LocationComments,
Manufacturer,
CatalogNumber,
WebAddress,
Vendor,
ContactInfo,
PurchaseDate,
Added,
Updated,
PurchasePrice,
ItemType,
ItemNotes,
UpdatedBy,
IsDeleted,
UsrId,
LocId,
SubId
from   
Tbl_Items End