CREATE PROCEDURE [dbo].[GetAllUsers] as Begin
Select 
Id,
UserName,
Email,
FirstName,
LastName,
PasswordHashed,
IsActive,
ActivationCode,
RoleId
from   
Tbl_Users End