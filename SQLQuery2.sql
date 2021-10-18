select top 1
(select sum(quantity) from ProductionSale 
where date = '13.10.2021' and 
dish = 10031 and 
type = 4) - 
(select sum(quantity) from ProductionSale 
where date = '13.10.2021' and 
dish = 10031 and 
type = 2)
from ProductionSale 
