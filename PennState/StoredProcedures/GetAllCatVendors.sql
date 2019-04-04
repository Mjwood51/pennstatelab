Create procedure GetAllCatVendors as Begin   
Select   
Id,
VendorName,
Pid,
HasChildren
from   
Tbl_CatagoryVendors End
