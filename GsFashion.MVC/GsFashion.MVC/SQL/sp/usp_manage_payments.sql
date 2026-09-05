CREATE OR ALTER PROCEDURE [dbo].[usp_manage_payments]
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
        SELECT
            p.payment_id AS PaymentId,
            p.rental_id AS RentalId,
            c.customer_id AS CustomerId,
            CONCAT(c.first_name, ' ', c.last_name) AS CustomerName,
            c.phone_number AS PhoneNumber,
            p.amount AS Amount,
            p.payment_date AS PaymentDate,
            p.payment_method AS PaymentMethod,
            p.transaction_ref AS TransactionRef,
            p.payment_type AS PaymentType,
            p.created_at AS CreatedAt
        FROM payments p
        INNER JOIN rentals r ON p.rental_id = r.rental_id
        INNER JOIN customers c ON r.customer_id = c.customer_id
        ORDER BY p.payment_id DESC;
        RETURN;
    END

    ELSE IF @type = 'GetById'
    BEGIN
        SELECT
            p.payment_id AS PaymentId,
            p.rental_id AS RentalId,
            c.customer_id AS CustomerId,
            CONCAT(c.first_name, ' ', c.last_name) AS CustomerName,
            c.phone_number AS PhoneNumber,
            p.amount AS Amount,
            p.payment_date AS PaymentDate,
            p.payment_method AS PaymentMethod,
            p.transaction_ref AS TransactionRef,
            p.payment_type AS PaymentType,
            p.created_at AS CreatedAt
        FROM payments p
        INNER JOIN rentals r ON p.rental_id = r.rental_id
        INNER JOIN customers c ON r.customer_id = c.customer_id
        WHERE p.payment_id = @payment_id;
        RETURN;
    END

    ELSE IF @type = 'Insert'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM rentals WHERE rental_id = @rental_id)
        BEGIN
            SELECT 'Rental booking not found' AS Message, 0 AS Status;
            RETURN;
        END

        IF @amount IS NULL OR @amount <= 0
        BEGIN
            SELECT 'Payment amount must be greater than zero' AS Message, 0 AS Status;
            RETURN;
        END

        IF @payment_method NOT IN ('Cash', 'UPI', 'Credit_Card', 'Bank_Transfer')
        BEGIN
            SELECT 'Invalid payment method' AS Message, 0 AS Status;
            RETURN;
        END

        IF @payment_type NOT IN ('Advance', 'Final_Settlement', 'Deposit_Refund', 'Late_Fee_Paid')
        BEGIN
            SELECT 'Invalid payment type' AS Message, 0 AS Status;
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            INSERT INTO payments (rental_id, amount, payment_date, payment_method, transaction_ref, payment_type)
            VALUES (@rental_id, @amount, ISNULL(@payment_date, GETDATE()), @payment_method, @transaction_ref, @payment_type);
           

            COMMIT TRANSACTION;

            SELECT 'Payment added successfully' AS Message, 1 AS Status, SCOPE_IDENTITY() AS PaymentId;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH;
        RETURN;
    END

    ELSE IF @type = 'Update'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM payments WHERE payment_id = @payment_id)
        BEGIN
            SELECT 'Payment not found' AS Message, 0 AS Status;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM rentals WHERE rental_id = @rental_id)
        BEGIN
            SELECT 'Rental booking not found' AS Message, 0 AS Status;
            RETURN;
        END

        IF @amount IS NULL OR @amount <= 0
        BEGIN
            SELECT 'Payment amount must be greater than zero' AS Message, 0 AS Status;
            RETURN;
        END

        IF @payment_method NOT IN ('Cash', 'UPI', 'Credit_Card', 'Bank_Transfer')
        BEGIN
            SELECT 'Invalid payment method' AS Message, 0 AS Status;
            RETURN;
        END

        IF @payment_type NOT IN ('Advance', 'Final_Settlement', 'Deposit_Refund', 'Late_Fee_Paid')
        BEGIN
            SELECT 'Invalid payment type' AS Message, 0 AS Status;
            RETURN;
        END

        BEGIN TRY
            BEGIN TRANSACTION;

            UPDATE payments
            SET rental_id = @rental_id,
                amount = @amount,
                payment_date = ISNULL(@payment_date, payment_date),
                payment_method = @payment_method,
                transaction_ref = @transaction_ref,
                payment_type = @payment_type
            WHERE payment_id = @payment_id;

            

            COMMIT TRANSACTION;

            SELECT 'Payment updated successfully' AS Message, 1 AS Status;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH;
        RETURN;
    END

    ELSE IF @type = 'Delete'
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM payments WHERE payment_id = @payment_id)
        BEGIN
            SELECT 'Payment not found' AS Message, 0 AS Status;
            RETURN;
        END

        DECLARE @DeleteRentalId INT;
        SELECT @DeleteRentalId = rental_id FROM payments WHERE payment_id = @payment_id;

        BEGIN TRY
            BEGIN TRANSACTION;

            --DELETE FROM payments WHERE payment_id = @payment_id;
			update payments set deleted_at=GETDATE(),is_deleted=1 where payment_id=@payment_id;

            COMMIT TRANSACTION;

            SELECT 'Payment deleted successfully' AS Message, 1 AS Status;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
        END CATCH;
        RETURN;
    END

    ELSE IF @type = 'RentalDropDown'
    BEGIN
        SELECT r.rental_id AS Id, CONCAT('Booking #', r.rental_id, ' - ', c.first_name, ' ', c.last_name) AS Name
        FROM rentals r
        INNER JOIN customers c ON r.customer_id = c.customer_id
        WHERE r.status <> 'Cancelled'
        ORDER BY r.rental_id DESC;
        RETURN;
    END

    ELSE IF @type = 'PaymentMethodDropDown'
    BEGIN
        SELECT 'Cash' AS Id, 'Cash' AS Name
        UNION ALL SELECT 'UPI', 'UPI'
        UNION ALL SELECT 'Credit_Card', 'Credit Card'
        UNION ALL SELECT 'Bank_Transfer', 'Bank Transfer';
        RETURN;
    END

    ELSE IF @type = 'PaymentTypeDropDown'
    BEGIN
        SELECT 'Advance' AS Id, 'Advance' AS Name
        UNION ALL SELECT 'Final_Settlement', 'Final Settlement'
        UNION ALL SELECT 'Deposit_Refund', 'Deposit Refund'
        UNION ALL SELECT 'Late_Fee_Paid', 'Late Fee Paid';
        RETURN;
    END

    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete, RentalDropDown, PaymentMethodDropDown or PaymentTypeDropDown.', 16, 1);
    END
END;
GO