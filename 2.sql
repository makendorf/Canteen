﻿select ProductsList.name, catList.name from ProductsList left join CategoryList as catList on ProductsList.category = catList.Id