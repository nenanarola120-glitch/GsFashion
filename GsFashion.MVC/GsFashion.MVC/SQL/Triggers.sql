-- =============================================
-- AUDIT TRIGGERS: INSERT + UPDATE
-- Logs old/new row data as JSON into each *_log table
-- Works correctly for both single-row and multi-row (batch) DML
-- =============================================

-- ---------------------------------------------
-- 1. CUSTOMERS
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_customers_ins_upd
ON customers
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- New rows (present in inserted, absent in deleted) => INSERT
    INSERT INTO customers_log (operationtype, customer_id, old_data, new_data, changedby)
    SELECT 'INSERT', i.customer_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.customer_id = i.customer_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.customer_id IS NULL;

    -- Rows present in both => UPDATE
    INSERT INTO customers_log (operationtype, customer_id, old_data, new_data, changedby)
    SELECT 'UPDATE', i.customer_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.customer_id = i.customer_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- ---------------------------------------------
-- 2. CATEGORIES
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_categories_ins_upd
ON categories
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO categories_log (operation_type, category_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.category_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.category_id = i.category_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.category_id IS NULL;

    INSERT INTO categories_log (operation_type, category_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.category_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.category_id = i.category_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- ---------------------------------------------
-- 3. INVENTORY_ITEMS
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_inventory_items_ins_upd
ON inventory_items
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO inventoryitemslog (operation_type, item_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.item_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.item_id = i.item_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.item_id IS NULL;

    INSERT INTO inventoryitemslog (operation_type, item_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.item_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.item_id = i.item_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- ---------------------------------------------
-- 4. RENTALS
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_rentals_ins_upd
ON rentals
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO rentals_log (operation_type, rental_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.rental_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.rental_id = i.rental_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.rental_id IS NULL;

    INSERT INTO rentals_log (operation_type, rental_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.rental_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.rental_id = i.rental_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- ---------------------------------------------
-- 5. RENTAL_ITEMS
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_rental_items_ins_upd
ON rental_items
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO rentalitemslog (operation_type, rentalite_mid, old_data, new_data, changedby)
    SELECT 'INSERT', i.rental_item_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.rental_item_id = i.rental_item_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.rental_item_id IS NULL;

    INSERT INTO rentalitemslog (operation_type, rentalite_mid, old_data, new_data, changedby)
    SELECT 'UPDATE', i.rental_item_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.rental_item_id = i.rental_item_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- ---------------------------------------------
-- 6. PAYMENTS
-- ---------------------------------------------
CREATE OR ALTER TRIGGER trg_payments_ins_upd
ON payments
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO payments_log (operation_type, payment_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.payment_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.payment_id = i.payment_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.payment_id IS NULL;

    INSERT INTO payments_log (operation_type, payment_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.payment_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.payment_id = i.payment_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO