CREATE Procedure [dbo].[GetFileDetails]  
(  
@Id int=null  
)  
as  
begin  
select Id, ItemFileName, DataStream from Tbl_File  
where Id=isnull(@Id,Id)  
End  
