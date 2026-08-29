
-- Customers Table
CREATE TABLE customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    phone_number NVARCHAR(20) NOT NULL UNIQUE,
    email NVARCHAR(150) NULL,
    address NVARCHAR(MAX) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Categories Table
CREATE TABLE categories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Inventory Items Table
CREATE TABLE inventory_items (
    item_id INT IDENTITY(1,1) PRIMARY KEY,
    sku_code NVARCHAR(50) NOT NULL UNIQUE,
    name NVARCHAR(150) NOT NULL,
    category_id INT NOT NULL,
    size NVARCHAR(30) NULL,
    color NVARCHAR(50) NULL,
    baserentalprice DECIMAL(10,2) NOT NULL,
    security_deposit DECIMAL(10,2) NOT NULL DEFAULT 0,
    purchase_cost DECIMAL(10,2) NOT NULL DEFAULT 0,
    status NVARCHAR(20) NOT NULL DEFAULT 'Available'
        CHECK (status IN ('Available','Rented','InWash','UnderRepair','Retired')),
    image_url NVARCHAR(500) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKinventorycategory FOREIGN KEY (category_id)
        REFERENCES categories(category_id)
);
GO

-- Rentals Table
CREATE TABLE rentals (
    rental_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    rental_date DATE NOT NULL,
    expectedreturndate DATE NOT NULL,
    actualreturndate DATE NULL,
    totalrentamount DECIMAL(10,2) NOT NULL DEFAULT 0,
    late_fee DECIMAL(10,2) NOT NULL DEFAULT 0,
    damage_fee DECIMAL(10,2) NOT NULL DEFAULT 0,
    discount DECIMAL(10,2) NOT NULL DEFAULT 0,
    grand_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    status NVARCHAR(20) NOT NULL DEFAULT 'Active'
        CHECK (status IN ('Active','Returned','Overdue','Cancelled')),
    notes NVARCHAR(MAX) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FKrentalscustomer FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id)
);
GO

-- Rental Items Table (maps outfits to a rental bill)
CREATE TABLE rental_items (
    rental_item_id INT IDENTITY(1,1) PRIMARY KEY,
    rental_id INT NOT NULL,
    item_id INT NOT NULL,
    agreed_rent_price DECIMAL(10,2) NOT NULL,
    condition_out NVARCHAR(MAX) NULL,
    condition_in NVARCHAR(MAX) NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_rentalitems_rental FOREIGN KEY (rental_id)
        REFERENCES rentals(rental_id),

    CONSTRAINT FK_rentalitems_item FOREIGN KEY (item_id)
        REFERENCES inventory_items(item_id)
);
GO

-- Payments Table
CREATE TABLE payments (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    rental_id INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_date DATETIME NOT NULL DEFAULT GETDATE(),
    payment_method NVARCHAR(20) NOT NULL
        CHECK (payment_method IN ('Cash','UPI','Credit_Card','Bank_Transfer')),
    transaction_ref NVARCHAR(100) NULL,
    payment_type NVARCHAR(30) NOT NULL
        CHECK (payment_type IN ('Advance','Final_Settlement','Deposit_Refund','Late_Fee_Paid')),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_payments_rental FOREIGN KEY (rental_id)
        REFERENCES rentals(rental_id)
);
GO

-- =============================================
-- 2. LOG (AUDIT) TABLES
-- =============================================

CREATE TABLE customers_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operationtype NVARCHAR(10) NOT NULL CHECK (operationtype IN ('INSERT','UPDATE','DELETE')),
    customer_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changedby NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE categories_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    category_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE inventoryitemslog (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    item_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE rentals_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    rental_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE rentalitemslog (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    rentalite_mid INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changedby NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE payments_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    payment_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO
