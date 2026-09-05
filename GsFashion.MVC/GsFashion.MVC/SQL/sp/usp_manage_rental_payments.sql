CREATE OR ALTER PROCEDURE dbo.usp_manage_rental_payments
    @type NVARCHAR(20), @rental_id INT, @payment_type NVARCHAR(20) = NULL,
    @actual_return_date DATE = NULL, @notes NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @type = 'GetByRental'
    BEGIN
        SELECT rental_payment_id AS RentalPaymentId, rental_id AS RentalId, payment_type AS PaymentType,
               amount AS Amount, payment_date AS PaymentDate, notes AS Notes
        FROM dbo.rental_payments WHERE rental_id = @rental_id ORDER BY payment_date;
        RETURN;
    END
    IF @type <> 'Record'
    BEGIN RAISERROR('Invalid payment type.', 16, 1); RETURN; END

    DECLARE @rent DECIMAL(10,2), @deposit DECIMAL(10,2), @discount DECIMAL(10,2), @start DATE, @amount DECIMAL(10,2);
    SELECT @rent = total_rent_amount, @deposit = security_deposit, @discount = discount, @start = rental_start_date
    FROM dbo.rentals WHERE rental_id = @rental_id;
    IF @rent IS NULL BEGIN SELECT 'Rental not found.' AS Message, 0 AS Status; RETURN; END

    IF @payment_type = 'Rent'
    BEGIN
        IF CAST(GETDATE() AS DATE) < @start BEGIN SELECT 'Rent can be collected on the rental start date.' AS Message, 0 AS Status; RETURN; END
        IF EXISTS (SELECT 1 FROM dbo.rental_payments WHERE rental_id = @rental_id AND payment_type = 'Rent') BEGIN SELECT 'Rent has already been collected.' AS Message, 0 AS Status; RETURN; END
        SET @amount = @rent - ISNULL(@discount, 0);
        INSERT INTO dbo.rental_payments (rental_id, payment_type, amount, notes) VALUES (@rental_id, 'Rent', @amount, @notes);
        UPDATE dbo.rentals SET amount_paid = ISNULL(amount_paid,0) + @amount, balance_amount = 0, status = 'Active' WHERE rental_id = @rental_id;
        SELECT 'Rental amount collected.' AS Message, 1 AS Status; RETURN;
    END
    IF @payment_type = 'DepositRefund'
    BEGIN
        IF @actual_return_date IS NULL BEGIN SELECT 'Actual return date is required before refunding the deposit.' AS Message, 0 AS Status; RETURN; END
        IF NOT EXISTS (SELECT 1 FROM dbo.rental_payments WHERE rental_id = @rental_id AND payment_type = 'Rent') BEGIN SELECT 'Collect the rental amount before refunding the deposit.' AS Message, 0 AS Status; RETURN; END
        IF EXISTS (SELECT 1 FROM dbo.rental_payments WHERE rental_id = @rental_id AND payment_type = 'DepositRefund') BEGIN SELECT 'Deposit has already been refunded.' AS Message, 0 AS Status; RETURN; END
        INSERT INTO dbo.rental_payments (rental_id, payment_type, amount, notes) VALUES (@rental_id, 'DepositRefund', @deposit, @notes);
        UPDATE dbo.rentals SET actual_return_date = @actual_return_date, status = 'Returned' WHERE rental_id = @rental_id;
        SELECT 'Security deposit refunded and rental marked as returned.' AS Message, 1 AS Status; RETURN;
    END
    SELECT 'Unsupported payment action.' AS Message, 0 AS Status;
END;
GO
