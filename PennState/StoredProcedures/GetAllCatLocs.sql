Create procedure GetAllCatLocs as Begin   
Select   
Id,
LocationName,
Pid,
HasChildren
from   
Tbl_CatagoryLocations End  
