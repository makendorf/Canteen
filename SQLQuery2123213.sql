select ProductsList.Id, ProductsList.name, mv.*
from ProductsList
left join
(
	select dates.max_date,dates.prod, Movement.*
	from
	(
		SELECT max([date]) as max_date,[product] as prod
		FROM [CanteenTest].[dbo].[Movement]
		where type=6
		group by product
	) dates, Movement
	where Movement.product=dates.prod and Movement.date=dates.max_date and Movement.type=6
) mv
on ProductsList.Id=mv.product