CREATE OR ALTER PROCEDURE [dbo].[usp_manage_rentals]
    @type NVARCHAR(20),
    @rental_id INT = NULL,
    @customer_id INT = NULL,
    @rental_start_date DATE = NULL,
    @expected_return_date DATE = NULL,
    @total_rent_amount DECIMAL(10,2) = NULL,
    @security_deposit DECIMAL(10,2) = NULL,
    @discount DECIMAL(10,2) = NULL,
    @grand_total DECIMAL(10,2) = NULL,
    @amount_paid DECIMAL(10,2) = NULL,
    @balance_amount DECIMAL(10,2) = NULL,
    @status NVARCHAR(20) = NULL,
    @notes NVARCHAR(MAX) = NULL,
    @item_ids NVARCHAR(MAX) = NULL,       -- comma-separated inventory_items.item_id list
    @condition_out NVARCHAR(MAX) = NULL,   -- applied to newly added rental_item rows
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
        SELECT
            r.rental_id AS RentalId,
            r.customer_id AS CustomerId,
            c.first_name AS FirstName,
            c.last_name AS LastName,
            c.phone_number AS PhoneNumber,
            r.booking_date AS BookingDate,
            r.rental_start_date AS RentalStartDate,
            r.expected_return_date AS ExpectedReturnDate,
            r.actual_return_date AS ActualReturnDate,
            r.total_rent_amount AS TotalRentAmount,
            r.security_deposit AS SecurityDeposit,
            r.discount AS Discount,
            r.grand_total AS GrandTotal,
            r.amount_paid AS AmountPaid,
            r.balance_amount AS BalanceAmount,
            r.status AS Status,
            r.notes AS Notes,
            r.created_at AS CreatedAt
        FROM rentals r
        INNER JOIN customers c ON r.customer_id = c.customer_id
        --WHERE r.status <> 'Cancelled'
        ORDER BY r.rental_id DESC;
        RETURN;
    END

    ELSE IF @type = 'GetById'
    BEGIN
        SELECT r.rental_id AS RentalId, r.customer_id AS CustomerId, c.first_name AS CustomerFirstName,
            c.last_name AS CustomerLastName,c.email AS CustomerEmail,c.address AS CustomerAddress,
            c.phone_number AS CustomerPhoneNumber,r.booking_date AS BookingDate,r.rental_start_date AS RentalStartDate,r.expected_return_date AS ExpectedReturnDate,r.actual_return_date AS ActualReturnDate,r.total_rent_amount AS TotalRentAmount,r.security_deposit AS SecurityDeposit,
            r.discount AS Discount,r.grand_total AS GrandTotal,r.amount_paid AS AmountPaid,
            r.balance_amount AS BalanceAmount,r.status AS Status,r.notes AS Notes,r.created_at AS CreatedAt
        FROM rentals r
        INNER JOIN customers c ON r.customer_id = c.customer_id
        WHERE r.rental_id = @rental_id;
		
		select ii.sku_code AS SkuCode,ii.name AS Name,ii.baserentalprice AS BaseRentalPrice,ii.security_deposit AS SecurityDeposit from inventory_items ii inner join rental_items ri on ii.item_id=ri.item_id where ri.rental_id= @rental_id;

        RETURN;
    END

    ELSE IF @type = 'Insert'
    BEGIN
        
        IF @rental_start_date IS NULL
        BEGIN
            SELECT 'Rental start date is required' AS Message, 0 AS Status;
            RETURN;
        END

        IF @rental_start_date < CAST(GETDATE() AS DATE)
        BEGIN
            SELECT 'Rental start date must be today or later' AS Message, 0 AS Status;
            RETURN;
        END

        IF @expected_return_date IS NULL
        BEGIN
            SELECT 'Expected return date is required' AS Message, 0 AS Status;
            RETURN;
        END

        IF @expected_return_date < @rental_start_date
        BEGIN
            SELECT 'Expected return date cannot be before rental start date' AS Message, 0 AS Status;
            RETURN;
        END

        IF @item_ids IS NULL OR LTRIM(RTRIM(@item_ids)) = ''
        BEGIN
            SELECT 'Please select at least one item to book' AS Message, 0 AS Status;
            RETURN;
        END

        DECLARE @NewItemIds TABLE (ItemId INT);
        INSERT INTO @NewItemIds (ItemId)
        SELECT TRY_CAST(LTRIM(RTRIM(value)) AS INT)
        FROM STRING_SPLIT(@item_ids, ',')
        WHERE TRY_CAST(LTRIM(RTRIM(value)) AS INT) IS NOT NULL;

        IF NOT EXISTS (SELECT 1 FROM @NewItemIds)
        BEGIN
            SELECT 'No valid items were selected' AS Message, 0 AS Status;
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            -- A customer can make more than one booking. Reuse the customer
            -- matched by the unique phone number instead of inserting a duplicate.
            SELECT @customer_id = customer_id
            FROM customers WITH (UPDLOCK, HOLDLOCK)
            WHERE phone_number = @phone_number;

            IF @customer_id IS NULL
            BEGIN
                INSERT INTO customers (first_name, last_name, phone_number, email, address)
                VALUES (@first_name, @last_name, @phone_number, @email, @address);

                SET @customer_id = SCOPE_IDENTITY();
            END

            INSERT INTO rentals
                (customer_id, booking_date, rental_start_date, expected_return_date, actual_return_date,
                 total_rent_amount, security_deposit, discount, grand_total,
                 amount_paid, balance_amount, status, notes)
            VALUES
                (@customer_id, GETDATE(), @rental_start_date, @expected_return_date, NULL,
                 ISNULL(@total_rent_amount, 0), ISNULL(@security_deposit, 0), ISNULL(@discount, 0),
                 ISNULL(@grand_total, 0), ISNULL(@amount_paid, 0), ISNULL(@balance_amount, 0),
                 ISNULL(@status, 'Booked'), @notes);

            DECLARE @NewRentalId INT = SCOPE_IDENTITY();

            -- The booking payment is the refundable security deposit only.
            INSERT INTO rental_payments (rental_id, payment_type, amount, notes)
            VALUES (@NewRentalId, 'Deposit', ISNULL(@security_deposit, 0), 'Security deposit collected at booking');

            DECLARE @CurrentItemId INT, @AgreedPrice DECIMAL(10,2);
            DECLARE @InsertedCount INT = 0;
            DECLARE @SkippedItems NVARCHAR(MAX) = '';

            DECLARE insert_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT ItemId FROM @NewItemIds;

            OPEN insert_cursor;
            FETCH NEXT FROM insert_cursor INTO @CurrentItemId;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                IF EXISTS (SELECT 1 FROM inventory_items WHERE item_id = @CurrentItemId AND status = 'Available')
                BEGIN
                   SELECT @AgreedPrice = baserentalprice FROM inventory_items WHERE item_id = @CurrentItemId;

                    INSERT INTO rental_items (rental_id, item_id, agreed_rent_price, condition_out)
                    VALUES (@NewRentalId, @CurrentItemId, @AgreedPrice, @condition_out);

            --        UPDATE inventory_items SET status = 'Rented' WHERE item_id = @CurrentItemId;

                    SET @InsertedCount = @InsertedCount + 1;
                END
                ELSE
                BEGIN
                    SET @SkippedItems = @SkippedItems + CAST(@CurrentItemId AS NVARCHAR(10)) + ',';
                END

                FETCH NEXT FROM insert_cursor INTO @CurrentItemId;
            END

            CLOSE insert_cursor;
            DEALLOCATE insert_cursor;

            --IF @InsertedCount = 0
            --BEGIN
            --    ROLLBACK TRANSACTION;
            --    SELECT 'None of the selected items are available anymore. Booking not created.' AS Message, 0 AS Status;
            --    RETURN;
            --END

            COMMIT TRANSACTION;

            --IF LEN(@SkippedItems) > 0
            --    SELECT CONCAT('Rental booked with ', @InsertedCount, ' item(s). Skipped unavailable item id(s): ',
            --                   LEFT(@SkippedItems, LEN(@SkippedItems) - 1)) AS Message, 1 AS Status, @NewRentalId AS RentalId;
            --ELSE
                SELECT CONCAT(@first_name,@last_name,'Your choli is booked successfully') AS Message, 1 AS Status, @NewRentalId AS Id;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH
        RETURN;
    END

    ELSE IF @type = 'Update'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM rentals WHERE rental_id = @rental_id)
        BEGIN
            SELECT 'Rental booking not found' AS Message, 0 AS Status;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM customers WHERE customer_id = @customer_id)
        BEGIN
            SELECT 'Customer not found' AS Message, 0 AS Status;
            RETURN;
        END

        IF @expected_return_date < @rental_start_date
        BEGIN
            SELECT 'Expected return date cannot be before rental start date' AS Message, 0 AS Status;
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            UPDATE rentals
            SET customer_id = @customer_id,
                rental_start_date = @rental_start_date,
                expected_return_date = @expected_return_date,
                total_rent_amount = ISNULL(@total_rent_amount, 0),
                security_deposit = ISNULL(@security_deposit, 0),
                discount = ISNULL(@discount, 0),
                grand_total = ISNULL(@grand_total, 0),
                amount_paid = ISNULL(@amount_paid, 0),
                balance_amount = ISNULL(@balance_amount, 0),
                status = ISNULL(@status, status),
                notes = @notes
            WHERE rental_id = @rental_id;

            DECLARE @SkippedUpdateItems NVARCHAR(MAX) = '';
            DECLARE @AddedCount INT = 0, @RemovedCount INT = 0;

            -- Only touch items at all if the caller actually passed a new item list.
            -- Passing NULL/empty @item_ids on Update means "leave items alone".
            IF @item_ids IS NOT NULL AND LTRIM(RTRIM(@item_ids)) <> ''
            BEGIN
                DECLARE @DesiredItemIds TABLE (ItemId INT);
                INSERT INTO @DesiredItemIds (ItemId)
                SELECT TRY_CAST(LTRIM(RTRIM(value)) AS INT)
                FROM STRING_SPLIT(@item_ids, ',')
                WHERE TRY_CAST(LTRIM(RTRIM(value)) AS INT) IS NOT NULL;

                -- ===== cursor 1: release items that were on the rental but aren't in the new list =====
                DECLARE @RemoveItemId INT;
                DECLARE remove_cursor CURSOR LOCAL FAST_FORWARD FOR
                    SELECT ri.item_id
                    FROM rental_items ri
                    WHERE ri.rental_id = @rental_id
                      AND ri.item_id NOT IN (SELECT ItemId FROM @DesiredItemIds);

                OPEN remove_cursor;
                FETCH NEXT FROM remove_cursor INTO @RemoveItemId;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DELETE FROM rental_items WHERE rental_id = @rental_id AND item_id = @RemoveItemId;
                    UPDATE inventory_items SET status = 'Available' WHERE item_id = @RemoveItemId;
                    SET @RemovedCount = @RemovedCount + 1;

                    FETCH NEXT FROM remove_cursor INTO @RemoveItemId;
                END

                CLOSE remove_cursor;
                DEALLOCATE remove_cursor;

                -- ===== cursor 2: book items newly added that aren't already on the rental =====
                DECLARE @AddItemId INT, @AddPrice DECIMAL(10,2);
                DECLARE add_cursor CURSOR LOCAL FAST_FORWARD FOR
                    SELECT d.ItemId
                    FROM @DesiredItemIds d
                    WHERE NOT EXISTS (
                        SELECT 1 FROM rental_items ri
                        WHERE ri.rental_id = @rental_id AND ri.item_id = d.ItemId
                    );

                OPEN add_cursor;
                FETCH NEXT FROM add_cursor INTO @AddItemId;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    IF EXISTS (SELECT 1 FROM inventory_items WHERE item_id = @AddItemId AND status = 'Available')
                    BEGIN
                        SELECT @AddPrice = baserentalprice FROM inventory_items WHERE item_id = @AddItemId;

                        INSERT INTO rental_items (rental_id, item_id, agreed_rent_price, condition_out)
                        VALUES (@rental_id, @AddItemId, @AddPrice, @condition_out);

                        UPDATE inventory_items SET status = 'Rented' WHERE item_id = @AddItemId;

                        SET @AddedCount = @AddedCount + 1;
                    END
                    ELSE
                    BEGIN
                        SET @SkippedUpdateItems = @SkippedUpdateItems + CAST(@AddItemId AS NVARCHAR(10)) + ',';
                    END

                    FETCH NEXT FROM add_cursor INTO @AddItemId;
                END

                CLOSE add_cursor;
                DEALLOCATE add_cursor;
            END

            COMMIT TRANSACTION;

            IF LEN(@SkippedUpdateItems) > 0
                SELECT CONCAT('Rental updated. ', @AddedCount, ' item(s) added, ', @RemovedCount,
                               ' item(s) removed. Skipped unavailable item id(s): ',
                               LEFT(@SkippedUpdateItems, LEN(@SkippedUpdateItems) - 1)) AS Message, 1 AS Status;
            ELSE
                SELECT CONCAT('Rental booking updated successfully. ', @AddedCount, ' item(s) added, ',
                               @RemovedCount, ' item(s) removed.') AS Message, 1 AS Status;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH
        RETURN;
    END

    ELSE IF @type = 'Delete'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM rentals WHERE rental_id = @rental_id AND status <> 'Cancelled')
        BEGIN
            SELECT 'Rental booking not found' AS Message, 0 AS Status;
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            --DECLARE @ReleaseItemId INT;
            --DECLARE @ReleasedCount INT = 0;

            --DECLARE release_cursor CURSOR LOCAL FAST_FORWARD FOR
            --    SELECT item_id FROM rental_items WHERE rental_id = @rental_id;

            --OPEN release_cursor;
            --FETCH NEXT FROM release_cursor INTO @ReleaseItemId;

            --WHILE @@FETCH_STATUS = 0
            --BEGIN
            --    UPDATE inventory_items SET status = 'Available' WHERE item_id = @ReleaseItemId;
            --    SET @ReleasedCount = @ReleasedCount + 1;

            --    FETCH NEXT FROM release_cursor INTO @ReleaseItemId;
            --END

            --CLOSE release_cursor;
            --DEALLOCATE release_cursor;

            UPDATE rentals SET status = 'Cancelled' WHERE rental_id = @rental_id;

            COMMIT TRANSACTION;

            SELECT 'Rental booking cancelled successfully.' AS Message, 1 AS Status;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH
        RETURN;
    END

    ELSE IF @type = 'CustomerDropDown'
    BEGIN
        SELECT customer_id AS Id, CONCAT(first_name, ' ', last_name) AS Name
        FROM customers
        ORDER BY first_name, last_name;
        RETURN;
    END

    ELSE IF @type = 'StatusDropDown'
    BEGIN
        SELECT 'Booked' AS Id, 'Booked' AS Name
        UNION ALL SELECT 'Active', 'Active'
        UNION ALL SELECT 'Returned', 'Returned'
        UNION ALL SELECT 'Overdue', 'Overdue'
        UNION ALL SELECT 'Cancelled', 'Cancelled';
        RETURN;
    END

    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete, CustomerDropDown or StatusDropDown.', 16, 1);
    END
END;
GO
