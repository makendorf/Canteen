select RecipeList.dish, dish.name, RecipeList.product, prod.name from RecipeList
left join ProductsList as prod on prod.Id = RecipeList.product
left join DishList as dish on dish.Id = RecipeList.dish
where dish.name like '%соус%' --and prod.name != 'Жир' and prod.name not like '%люля%';
--(dish.name like '%печень по с%' or dish.name like '%сердце в соус%' or dish.name like '%люля%') and
--		(prod.name != 'Жир' or prod.name not like '%печень%' or prod.name not like 'сердце' or prod.name not like 'люля')
--delete from RecipeList where dish = 10063