select * from Movement 
select prod.name as Продукт, Dish.name, sum(Movement.quantity) from Movement 
left join ProductsList as prod on prod.Id = Movement.product
left join DishList as Dish on Dish.Id = Movement.dish
where type = 1 and date = '05.10.2021'
group by prod.name, Dish.name