Create procedure GetAllCatTypes as Begin   
Select   
Id,
TypeName,
Pid,
HasChildren
from   
Tbl_CatagoryTypes End 
