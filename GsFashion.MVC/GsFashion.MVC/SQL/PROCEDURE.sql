-- =============================================
-- MANAGER STORED PROCEDURES
-- One proc per table, switched by @type:
--   'GetAll' | 'GetById' | 'Insert' | 'Update' | 'Delete'
-- =============================================

-- =====================================================
-- 1. CUSTOMERS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_customers
    @type NVARCHAR(20),
    @customer_id INT = NULL,
    @first_name NVARCHAR(100) = NULL,
    @last_name NVARCHAR(100) = NULL,
    @phone_number NVARCHAR(20) = NULL,
    @email NVARCHAR(150) = NULL,
    @address NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM customers ORDER BY customer_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM customers WHERE customer_id = @customer_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO customers (first_name, last_name, phone_number, email, address)
        VALUES (@first_name, @last_name, @phone_number, @email, @address);

        SELECT SCOPE_IDENTITY() AS new_customer_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE customers
        SET first_name   = @first_name,
            last_name    = @last_name,
            phone_number = @phone_number,
            email        = @email,
            address      = @address
        WHERE customer_id = @customer_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM customers WHERE customer_id = @customer_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 2. CATEGORIES
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_categories
    @type NVARCHAR(20),
    @category_id INT = NULL,
    @name NVARCHAR(100) = NULL,
    @description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM categories ORDER BY category_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM categories WHERE category_id = @category_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO categories (name, description)
        VALUES (@name, @description);

        SELECT SCOPE_IDENTITY() AS new_category_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE categories
        SET name        = @name,
            description = @description
        WHERE category_id = @category_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM categories WHERE category_id = @category_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 3. INVENTORY_ITEMS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_inventory_items
    @type NVARCHAR(20),
    @item_id INT = NULL,
    @sku_code NVARCHAR(50) = NULL,
    @name NVARCHAR(150) = NULL,
    @category_id INT = NULL,
    @size NVARCHAR(30) = NULL,
    @color NVARCHAR(50) = NULL,
    @baserentalprice DECIMAL(10,2) = NULL,
    @security_deposit DECIMAL(10,2) = NULL,
    @purchase_cost DECIMAL(10,2) = NULL,
    @status NVARCHAR(20) = NULL,
    @image_url NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM inventory_items ORDER BY item_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM inventory_items WHERE item_id = @item_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO inventory_items
            (sku_code, name, category_id, size, color, baserentalprice,
             security_deposit, purchase_cost, status, image_url)
        VALUES
            (@sku_code, @name, @category_id, @size, @color, @baserentalprice,
             ISNULL(@security_deposit, 0), ISNULL(@purchase_cost, 0),
             ISNULL(@status, 'Available'), @image_url);

        SELECT SCOPE_IDENTITY() AS new_item_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE inventory_items
        SET sku_code         = @sku_code,
            name             = @name,
            category_id      = @category_id,
            size             = @size,
            color            = @color,
            baserentalprice  = @baserentalprice,
            security_deposit = @security_deposit,
            purchase_cost    = @purchase_cost,
            status           = @status,
            image_url        = @image_url
        WHERE item_id = @item_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM inventory_items WHERE item_id = @item_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 4. RENTALS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_rentals
    @type NVARCHAR(20),
    @rental_id INT = NULL,
    @customer_id INT = NULL,
    @rental_date DATE = NULL,
    @expectedreturndate DATE = NULL,
    @actualreturndate DATE = NULL,
    @totalrentamount DECIMAL(10,2) = NULL,
    @late_fee DECIMAL(10,2) = NULL,
    @damage_fee DECIMAL(10,2) = NULL,
    @discount DECIMAL(10,2) = NULL,
    @grand_total DECIMAL(10,2) = NULL,
    @status NVARCHAR(20) = NULL,
    @notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM rentals ORDER BY rental_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM rentals WHERE rental_id = @rental_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO rentals
            (customer_id, rental_date, expectedreturndate, actualreturndate,
             totalrentamount, late_fee, damage_fee, discount, grand_total, status, notes)
        VALUES
            (@customer_id, @rental_date, @expectedreturndate, @actualreturndate,
             ISNULL(@totalrentamount, 0), ISNULL(@late_fee, 0), ISNULL(@damage_fee, 0),
             ISNULL(@discount, 0), ISNULL(@grand_total, 0), ISNULL(@status, 'Active'), @notes);

        SELECT SCOPE_IDENTITY() AS new_rental_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE rentals
        SET customer_id         = @customer_id,
            rental_date         = @rental_date,
            expectedreturndate  = @expectedreturndate,
            actualreturndate    = @actualreturndate,
            totalrentamount     = @totalrentamount,
            late_fee            = @late_fee,
            damage_fee          = @damage_fee,
            discount            = @discount,
            grand_total         = @grand_total,
            status              = @status,
            notes               = @notes
        WHERE rental_id = @rental_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM rentals WHERE rental_id = @rental_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 5. RENTAL_ITEMS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_rental_items
    @type NVARCHAR(20),
    @rental_item_id INT = NULL,
    @rental_id INT = NULL,
    @item_id INT = NULL,
    @agreed_rent_price DECIMAL(10,2) = NULL,
    @condition_out NVARCHAR(MAX) = NULL,
    @condition_in NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM rental_items ORDER BY rental_item_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM rental_items WHERE rental_item_id = @rental_item_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO rental_items
            (rental_id, item_id, agreed_rent_price, condition_out, condition_in)
        VALUES
            (@rental_id, @item_id, @agreed_rent_price, @condition_out, @condition_in);

        SELECT SCOPE_IDENTITY() AS new_rental_item_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE rental_items
        SET rental_id          = @rental_id,
            item_id            = @item_id,
            agreed_rent_price  = @agreed_rent_price,
            condition_out      = @condition_out,
            condition_in       = @condition_in
        WHERE rental_item_id = @rental_item_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM rental_items WHERE rental_item_id = @rental_item_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 6. PAYMENTS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_payments
    @type NVARCHAR(20),
    @payment_id INT = NULL,
    @rental_id INT = NULL,
    @amount DECIMAL(10,2) = NULL,
    @payment_date DATETIME = NULL,
    @payment_method NVARCHAR(20) = NULL,
    @transaction_ref NVARCHAR(100) = NULL,
    @payment_type NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM payments ORDER BY payment_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM payments WHERE payment_id = @payment_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO payments
            (rental_id, amount, payment_date, payment_method, transaction_ref, payment_type)
        VALUES
            (@rental_id, @amount, ISNULL(@payment_date, GETDATE()), @payment_method,
             @transaction_ref, @payment_type);

        SELECT SCOPE_IDENTITY() AS new_payment_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE payments
        SET rental_id       = @rental_id,
            amount          = @amount,
            payment_date    = @payment_date,
            payment_method  = @payment_method,
            transaction_ref = @transaction_ref,
            payment_type    = @payment_type
        WHERE payment_id = @payment_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM payments WHERE payment_id = @payment_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO



