Create procedure GetAllCatOwners as Begin   
Select   
Id,
OwnerName,
Pid,
HasChildren
from   
Tbl_CatagoryOwners End
