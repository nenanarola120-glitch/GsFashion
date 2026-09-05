--select * from role_menu_permissions;

--select * from menus;

--select * from roles;

--select * from admin_users;
--select * from categories;
--select * from customers;

--select * from inventory_items;
--INSERT INTO inventory_items(    item_name,    category_id,    quantity,    unit_price)
--VALUES(    'Designer Bridal Choli',    1,    5,    1500.00);
--select * from payments;

--select * from rental_items;
-- Child tables first
--DROP TABLE IF EXISTS payments;
--drop table payments_log;
--DROP TABLE IF EXISTS rental_items;

---- Rental audit/log table
--DROP TABLE IF EXISTS rentalitemslog;
--DROP TABLE IF EXISTS rentals_log;

---- Parent table
--DROP TABLE IF EXISTS rentals;

--DROP PROCEDURE IF EXISTS usp_manage_payments;
--DROP PROCEDURE IF EXISTS usp_manage_rentals;
--DROP PROCEDURE IF EXISTS usp_manage_rental_items;
--GO

--INSERT INTO rental_items(    rental_id,    inventory_item_id,    quantity,    price)
--VALUES(    1,    1,    1,    1500.00);

--select * from rentals;
--INSERT INTO rentals(    customer_id,    rental_date,    return_date,    total_amount,    status)
--VALUES(    1,    '2026-08-30',    '2026-09-02',    1500.00,    'Active');

--EXEC usp_manage_roles
--@type = 'Insert',
--@role_name = 'Customer',
--@description = 'special person';
--update menus set menu_url='/Menu/GetAllMenuList' where menu_id=2;
--update role_menu_permissions set can_edit=0 where permission_id = 1;


-------------------

--SELECT TOP (1000) [rental_item_id]
--      ,[rental_id]
--      ,[item_id]
--      ,[agreed_rent_price]
--      ,[condition_out]
--      ,[condition_in]
--      ,[created_at]
--  FROM [GS_Fashion_05_09_2026].[dbo].[rental_items]

--  select * from roles;
--  select * from admin_users;
--  select * from menus;
--  select * from role_menu_permissions;

  --insert into menus values('Menu',1,'/Menu/GetAllMenuList/','ddd',18,1,GETDATE());
  --update menus set menu_url=NULL where menu_id=1;

  --insert into role_menu_permissions values (1,1,1,1,1,1,GETDATE());

  --insert into admin_users values('Admin','Admin','Admin','ddd@gmail.com',1,1,GETDATE());

  --insert into roles values('Customer','dddd',1,GETDATE());

  select * from customers where customer_id=6;
  select * from rentals;
  select * from inventory_items;
  select * from rental_items;

  select ii.sku_code AS SkuCode,ii.name AS Name,ii.baserentalprice AS BaseRentalPrice,ii.security_deposit AS SecurityDeposit from inventory_items ii inner join rental_items ri on ii.item_id=ri.item_id where ri.rental_id=2;

  DECLARE @rental_id int = 3;
  select * from rentals WHERE rental_id = @rental_id;
  select  * from inventory_items WHERE item_id in (1);

  select * from rental_items where rental_id=@rental_id

  select * from rentals;

  add report like get All Available report Choli  from data and to date wise and one drop down is C-15(choliname) wise also display only to check choli is availvable in fomr date to date wise with choli name also in dropdown one serch input to serch choli name and code wise serch


