CREATE OR ALTER PROCEDURE [dbo].[usp_manage_inventory_items]
    @type NVARCHAR(30),
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
    @image_url NVARCHAR(500) = NULL,
    @searching_string NVARCHAR(150) = NULL,
    @rental_start_date DATE = NULL,
    @expected_return_date DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @type = 'GetAll'
        BEGIN
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
            INNER JOIN categories c
                ON c.category_id = i.category_id
            WHERE ISNULL(i.is_deleted, 0) = 0 AND (NULLIF(LTRIM(RTRIM(@searching_string)), '') IS NULL
               OR i.sku_code LIKE '%' + @searching_string + '%'
               OR i.name LIKE '%' + @searching_string + '%'
			   OR c.name LIKE '%' + @searching_string + '%'
               OR i.color LIKE '%' + @searching_string + '%'
               OR CONVERT(NVARCHAR(50), i.baserentalprice) LIKE '%' + @searching_string + '%'
               OR i.status LIKE '%' + @searching_string + '%')
            ORDER BY i.item_id;
            RETURN;
        END
        ELSE IF @type = 'GetAvailableForRental'
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
        ELSE IF @type = 'GetById'
        BEGIN
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
            INNER JOIN categories c
                ON c.category_id = i.category_id
            WHERE i.item_id = @item_id;

            RETURN;
        END

        ELSE IF @type = 'Insert'
        BEGIN

            IF EXISTS(SELECT 1 FROM inventory_items WHERE sku_code = @sku_code)
            BEGIN
                SELECT 'SKU code already exists' AS Message, 0 AS Status;
                RETURN;
            END

            INSERT INTO inventory_items(sku_code,name,category_id,size,color,baserentalprice,security_deposit,
                purchase_cost,status,image_url)
            VALUES(@sku_code,@name,@category_id,@size,@color,@baserentalprice,ISNULL(@security_deposit, 0),
                ISNULL(@purchase_cost, 0),ISNULL(@status, 'Available'),@image_url);

            SELECT 'Inventory item added successfully' AS Message,1 AS Status;
            RETURN;
        END

        ELSE IF @type = 'Update'
        BEGIN

            IF NOT EXISTS ( SELECT 1 FROM inventory_items WHERE item_id = @item_id)
            BEGIN
                SELECT 'Inventory item not found' AS Message,0 AS Status;
                RETURN;
            END

            IF EXISTS(SELECT 1 FROM inventory_items WHERE sku_code = @sku_code AND item_id <> @item_id)
            BEGIN
                SELECT 'SKU code already exists' AS Message,0 AS Status;
                RETURN;
            END

            UPDATE inventory_items SET sku_code= @sku_code,name= @name,category_id= @category_id,
                size= @size,color= @color,baserentalprice  = @baserentalprice,security_deposit = @security_deposit,purchase_cost= @purchase_cost,status= @status,image_url= @image_url
            WHERE item_id = @item_id;

            SELECT 'Inventory item updated successfully' AS Message,1 AS Status;
            RETURN;
        END

        ELSE IF @type = 'Delete'
        BEGIN

            IF NOT EXISTS(SELECT 1 FROM inventory_items WHERE item_id = @item_id)
            BEGIN
                SELECT 'Inventory item not found' AS Message,0 AS Status;
                RETURN;
            END
			ELSE IF EXISTS(select 1 from rental_items where item_id=@item_id)
			BEGIN
				SELECT 'Inventory item are available in rental item.' AS Message,0 AS Status;
                RETURN;
			END
			ELSE
			BEGIN
				update inventory_items set deleted_at=GETDATE(),is_deleted=1 where item_id=@item_id;
				
				SELECT'Inventory item deleted successfully' AS Message, 1 AS Status;
				RETURN;
			END
        END

        ELSE IF @type = 'InventoryItemDropDown'
        BEGIN

            SELECT i.item_id AS Id,i.sku_code AS ItemCode,i.name AS ItemName,c.name AS Category,i.size AS Size,
                i.color AS Color,i.baserentalprice AS RentPrice,i.status AS Status FROM inventory_items i
            INNER JOIN categories c ON i.category_id = c.category_id WHERE i.status = 'Available'
            ORDER BY i.item_id DESC;
            RETURN;
        END
        ELSE IF @type = 'DropDown'
        BEGIN

            SELECT item_id AS Id,sku_code + ' - ' + name AS Name FROM inventory_items WHERE status = 'Available'ORDER BY name;
            RETURN;
        END
        ELSE
        BEGIN
            SELECT 'Invalid @type. Use GetAll, GetById, Insert, Update, Delete, InventoryItemDropDown or DropDown.'AS Message,0 AS Status;
            RETURN;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE() AS Message,0 AS Status;
    END CATCH
END
GO
