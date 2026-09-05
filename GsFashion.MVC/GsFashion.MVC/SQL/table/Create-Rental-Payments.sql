CREATE TABLE dbo.rental_payments (
    rental_payment_id INT IDENTITY(1,1) PRIMARY KEY,
    rental_id INT NOT NULL,
    payment_type NVARCHAR(20) NOT NULL CHECK (payment_type IN ('Deposit', 'Rent', 'DepositRefund')),
    amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
    payment_date DATETIME NOT NULL DEFAULT GETDATE(),
    notes NVARCHAR(MAX) NULL,
    CONSTRAINT FK_rental_payments_rentals FOREIGN KEY (rental_id) REFERENCES dbo.rentals(rental_id)
);
GO
