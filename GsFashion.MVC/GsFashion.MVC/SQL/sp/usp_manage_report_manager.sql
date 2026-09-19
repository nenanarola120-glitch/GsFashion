CREATE OR ALTER PROCEDURE [dbo].[usp_manage_report_manager]
    @type NVARCHAR(30),
    @item_id INT = NULL,
    @status NVARCHAR(20) = NULL,
    @searching_string NVARCHAR(150) = NULL,
    @rental_start_date DATE = NULL,
    @expected_return_date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @type = 'GetAvailableForRental'
        BEGIN
            IF @rental_start_date IS NOT NULL
               AND (@expected_return_date IS NULL OR @expected_return_date < @rental_start_date)
            BEGIN
                SELECT 'A valid rental date range is required.' AS Message, 0 AS Status;
                RETURN;
            END

            IF @rental_start_date IS NULL AND @expected_return_date IS NOT NULL
            BEGIN
                SELECT 'A valid rental date range is required.' AS Message, 0 AS Status;
                RETURN;
            END

            -- A blank status means "All statuses".  The report defaults this value to Available.
            SELECT
                i.item_id          AS ItemId,
                i.sku_code         AS SkuCode,
                i.name             AS Name,
                i.category_id      AS CategoryId,
                c.name             AS CategoryName,
                i.size             AS Size,
                i.color            AS Color,
                i.baserentalprice  AS BaseRentalPrice,
                i.security_deposit AS SecurityDeposit,
                i.purchase_cost    AS PurchaseCost,
                i.status           AS Status,
                i.image_url        AS ImageUrl,
                i.created_at       AS CreatedAt
            FROM inventory_items i
            INNER JOIN categories c ON c.category_id = i.category_id
            WHERE ISNULL(i.is_deleted, 0) = 0
              AND (@item_id IS NULL OR i.item_id = @item_id)
              AND (NULLIF(LTRIM(RTRIM(@status)), '') IS NULL OR i.status = @status)
              AND (NULLIF(LTRIM(RTRIM(@searching_string)), '') IS NULL
                   OR i.sku_code LIKE '%' + @searching_string + '%'
                   OR i.name LIKE '%' + @searching_string + '%'
                   OR c.name LIKE '%' + @searching_string + '%'
                   OR i.color LIKE '%' + @searching_string + '%')
              -- When a date range is provided, return only cholis that can be
              -- rented for the complete range. The same check works for either
              -- all cholis or one selected @item_id.
              AND
              (
                  @rental_start_date IS NULL
                  OR
                  (
                      i.status = 'Available'
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM rental_items ri
                          INNER JOIN rentals r ON r.rental_id = ri.rental_id
                          WHERE ri.item_id = i.item_id
                            AND r.status <> 'Cancelled'
                            AND r.rental_start_date <= @expected_return_date
                            AND ISNULL(r.actual_return_date, r.expected_return_date) >= @rental_start_date
                      )
                  )
              )
            ORDER BY i.item_id;
            RETURN;
        END

        ELSE IF @type = 'GetBookedCholiReport'
        BEGIN
            IF @rental_start_date IS NULL OR @expected_return_date IS NULL
               OR @expected_return_date < @rental_start_date
            BEGIN
                SELECT 'A valid from date and to date are required.' AS Message, 0 AS Status;
                RETURN;
            END

            -- Return one row for every booked Choli. The UI groups these rows
            -- by SKU/Choli and displays each booking's customer details.
            SELECT
                i.item_id AS ItemId,
                i.sku_code AS SkuCode,
                i.name AS Name,
                CONCAT('BILL-', r.rental_id) AS BillNo,
                CONCAT(c.first_name, ' ', c.last_name) AS CustomerName,
                c.phone_number AS MobileNumber
            FROM inventory_items i
            INNER JOIN rental_items ri ON ri.item_id = i.item_id
            INNER JOIN rentals r ON r.rental_id = ri.rental_id
            INNER JOIN customers c ON c.customer_id = r.customer_id
            WHERE ISNULL(i.is_deleted, 0) = 0
              AND ISNULL(ri.is_deleted, 0) = 0
              AND ISNULL(r.is_deleted, 0) = 0
              AND r.status <> 'Cancelled'
              AND r.rental_start_date >= @rental_start_date
              AND r.rental_start_date <= @expected_return_date
              AND (@item_id IS NULL OR i.item_id = @item_id)
            ORDER BY i.sku_code, r.rental_id;
            RETURN;
        END

        ELSE IF @type = 'GetDashboardMetrics'
        BEGIN
            SELECT 'Today Booking' AS Label,
                   CAST(COUNT(*) AS INT) AS Count
            FROM rentals
            WHERE ISNULL(is_deleted, 0) = 0
              AND CAST(booking_date AS DATE) = CAST(GETDATE() AS DATE)

            UNION ALL

            SELECT 'Total Booking' AS Label,
                   CAST(COUNT(*) AS INT) AS Count
            FROM rentals
            WHERE ISNULL(is_deleted, 0) = 0
              AND status <> 'Cancelled'

            UNION ALL

            SELECT 'Total Customer' AS Label,
                   CAST(COUNT(*) AS INT) AS Count
            FROM customers
            WHERE ISNULL(is_deleted, 0) = 0

            UNION ALL

            SELECT 'Inventory Item' AS Label,
                   CAST(COUNT(*) AS INT) AS Count
            FROM inventory_items
            WHERE ISNULL(is_deleted, 0) = 0

            UNION ALL

            SELECT 'Today Return Choli' AS Label,
                   CAST(COUNT(ri.rental_item_id) AS INT) AS Count
            FROM rental_items ri
            INNER JOIN rentals r ON r.rental_id = ri.rental_id
            WHERE ISNULL(ri.is_deleted, 0) = 0
              AND ISNULL(r.is_deleted, 0) = 0
              AND CAST(r.actual_return_date AS DATE) = CAST(GETDATE() AS DATE);
            RETURN;
        END

        ELSE IF @type = 'GetMonthlyPaidAmounts'
        BEGIN
            ;WITH Months AS
            (
                SELECT DATEADD(MONTH, -5, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS MonthStart
                UNION ALL
                SELECT DATEADD(MONTH, 1, MonthStart)
                FROM Months
                WHERE MonthStart < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
            )
            SELECT
                FORMAT(m.MonthStart, 'MMM yyyy') AS Label,
                ISNULL(SUM(rp.amount), 0) AS Amount
            FROM Months m
            LEFT JOIN rental_payments rp
                ON rp.payment_type = 'Rent'
               AND rp.payment_date >= m.MonthStart
               AND rp.payment_date < DATEADD(MONTH, 1, m.MonthStart)
            GROUP BY m.MonthStart
            ORDER BY m.MonthStart
            OPTION (MAXRECURSION 6);
            RETURN;
        END

        SELECT 'Invalid @type. Use GetAvailableForRental, GetBookedCholiReport, GetDashboardMetrics, or GetMonthlyPaidAmounts.' AS Message, 0 AS Status;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message, 0 AS Status;
    END CATCH
END
Go