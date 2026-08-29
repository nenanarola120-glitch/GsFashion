-- =============================================================
-- ADMIN MODULE: ROLES, MENUS, ROLE-WISE MENU PERMISSIONS, ADMIN USERS
-- Follows the same conventions as your existing schema:
--   IDENTITY PKs, *_log audit tables, AFTER INSERT/UPDATE triggers
--   logging FOR JSON PATH rows, and usp_manage_* procs switched by @type
-- =============================================================

-- =============================================
-- 1. CORE TABLES
-- =============================================

-- Roles Table
CREATE TABLE roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE,
    description NVARCHAR(MAX) NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Menus Table (supports parent/child for sub-menus)
CREATE TABLE menus (
    menu_id INT IDENTITY(1,1) PRIMARY KEY,
    menu_name NVARCHAR(100) NOT NULL,
    parent_menu_id INT NULL,
    menu_url NVARCHAR(255) NULL,
    icon_class NVARCHAR(100) NULL,
    display_order INT NOT NULL DEFAULT 0,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_menus_parent FOREIGN KEY (parent_menu_id)
        REFERENCES menus(menu_id)
);
GO

-- Admin Users Table (login users tied to a role)
CREATE TABLE admin_users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(100) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(150) NULL,
    email NVARCHAR(150) NULL,
    role_id INT NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_adminusers_role FOREIGN KEY (role_id)
        REFERENCES roles(role_id)
);
GO

-- Role <-> Menu Permissions Table
CREATE TABLE role_menu_permissions (
    permission_id INT IDENTITY(1,1) PRIMARY KEY,
    role_id INT NOT NULL,
    menu_id INT NOT NULL,
    can_view BIT NOT NULL DEFAULT 1,
    can_add BIT NOT NULL DEFAULT 0,
    can_edit BIT NOT NULL DEFAULT 0,
    can_delete BIT NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_rmp_role FOREIGN KEY (role_id)
        REFERENCES roles(role_id),
    CONSTRAINT FK_rmp_menu FOREIGN KEY (menu_id)
        REFERENCES menus(menu_id),
    CONSTRAINT UQ_role_menu UNIQUE (role_id, menu_id)
);
GO

-- =============================================
-- 2. LOG (AUDIT) TABLES
-- =============================================

CREATE TABLE roles_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    role_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE menus_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    menu_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE admin_users_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    user_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE role_menu_permissions_log (
    log_id INT IDENTITY(1,1) PRIMARY KEY,
    operation_type NVARCHAR(10) NOT NULL CHECK (operation_type IN ('INSERT','UPDATE','DELETE')),
    permission_id INT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    changed_by NVARCHAR(128) NOT NULL DEFAULT SYSTEM_USER,
    changed_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 3. AUDIT TRIGGERS (INSERT + UPDATE)
-- =============================================

CREATE OR ALTER TRIGGER trg_roles_ins_upd
ON roles
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO roles_log (operation_type, role_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.role_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.role_id = i.role_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.role_id IS NULL;

    INSERT INTO roles_log (operation_type, role_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.role_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.role_id = i.role_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

CREATE OR ALTER TRIGGER trg_menus_ins_upd
ON menus
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO menus_log (operation_type, menu_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.menu_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.menu_id = i.menu_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.menu_id IS NULL;

    INSERT INTO menus_log (operation_type, menu_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.menu_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.menu_id = i.menu_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

CREATE OR ALTER TRIGGER trg_admin_users_ins_upd
ON admin_users
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO admin_users_log (operation_type, user_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.user_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.user_id = i.user_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.user_id IS NULL;

    INSERT INTO admin_users_log (operation_type, user_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.user_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.user_id = i.user_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

CREATE OR ALTER TRIGGER trg_role_menu_permissions_ins_upd
ON role_menu_permissions
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO role_menu_permissions_log (operation_type, permission_id, old_data, new_data, changed_by)
    SELECT 'INSERT', i.permission_id, NULL, j.new_data, SYSTEM_USER
    FROM inserted i
    LEFT JOIN deleted d ON d.permission_id = i.permission_id
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) j(new_data)
    WHERE d.permission_id IS NULL;

    INSERT INTO role_menu_permissions_log (operation_type, permission_id, old_data, new_data, changed_by)
    SELECT 'UPDATE', i.permission_id, jd.old_data, ji.new_data, SYSTEM_USER
    FROM inserted i
    JOIN deleted d ON d.permission_id = i.permission_id
    CROSS APPLY (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) jd(old_data)
    CROSS APPLY (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) ji(new_data);
END
GO

-- =============================================================
-- 4. MANAGER STORED PROCEDURES
-- One proc per table, switched by @type:
--   'GetAll' | 'GetById' | 'Insert' | 'Update' | 'Delete'
-- =============================================================

-- =====================================================
-- 4.1 ROLES
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_roles
    @type NVARCHAR(20),
    @role_id INT = NULL,
    @role_name NVARCHAR(50) = NULL,
    @description NVARCHAR(MAX) = NULL,
    @is_active BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM roles ORDER BY role_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM roles WHERE role_id = @role_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO roles (role_name, description, is_active)
        VALUES (@role_name, @description, ISNULL(@is_active, 1));

        SELECT SCOPE_IDENTITY() AS new_role_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE roles
        SET role_name   = @role_name,
            description = @description,
            is_active   = @is_active
        WHERE role_id = @role_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM roles WHERE role_id = @role_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 4.2 MENUS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_menus
    @type NVARCHAR(20),
    @menu_id INT = NULL,
    @menu_name NVARCHAR(100) = NULL,
    @parent_menu_id INT = NULL,
    @menu_url NVARCHAR(255) = NULL,
    @icon_class NVARCHAR(100) = NULL,
    @display_order INT = NULL,
    @is_active BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM menus ORDER BY parent_menu_id, display_order, menu_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM menus WHERE menu_id = @menu_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO menus (menu_name, parent_menu_id, menu_url, icon_class, display_order, is_active)
        VALUES (@menu_name, @parent_menu_id, @menu_url, @icon_class,
                ISNULL(@display_order, 0), ISNULL(@is_active, 1));

        SELECT SCOPE_IDENTITY() AS new_menu_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE menus
        SET menu_name      = @menu_name,
            parent_menu_id = @parent_menu_id,
            menu_url       = @menu_url,
            icon_class     = @icon_class,
            display_order  = @display_order,
            is_active      = @is_active
        WHERE menu_id = @menu_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM menus WHERE menu_id = @menu_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 4.3 ADMIN_USERS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_admin_users
    @type NVARCHAR(20),
    @user_id INT = NULL,
    @username NVARCHAR(100) = NULL,
    @password_hash NVARCHAR(255) = NULL,
    @full_name NVARCHAR(150) = NULL,
    @email NVARCHAR(150) = NULL,
    @role_id INT = NULL,
    @is_active BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT u.*, r.role_name
        FROM admin_users u
        JOIN roles r ON r.role_id = u.role_id
        ORDER BY u.user_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT u.*, r.role_name
        FROM admin_users u
        JOIN roles r ON r.role_id = u.role_id
        WHERE u.user_id = @user_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO admin_users (username, password_hash, full_name, email, role_id, is_active)
        VALUES (@username, @password_hash, @full_name, @email, @role_id, ISNULL(@is_active, 1));

        SELECT SCOPE_IDENTITY() AS new_user_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE admin_users
        SET username      = @username,
            full_name     = @full_name,
            email         = @email,
            role_id       = @role_id,
            is_active     = @is_active
        WHERE user_id = @user_id;
        -- password_hash intentionally not updated here; use a dedicated
        -- usp_change_admin_password proc if you want password changes audited separately.
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM admin_users WHERE user_id = @user_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 4.4 ROLE_MENU_PERMISSIONS
-- =====================================================
CREATE OR ALTER PROCEDURE usp_manage_role_menu_permissions
    @type NVARCHAR(20),
    @permission_id INT = NULL,
    @role_id INT = NULL,
    @menu_id INT = NULL,
    @can_view BIT = NULL,
    @can_add BIT = NULL,
    @can_edit BIT = NULL,
    @can_delete BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @type = 'GetAll'
    BEGIN
        SELECT * FROM role_menu_permissions ORDER BY role_id, menu_id;
    END
    ELSE IF @type = 'GetById'
    BEGIN
        SELECT * FROM role_menu_permissions WHERE permission_id = @permission_id;
    END
    ELSE IF @type = 'Insert'
    BEGIN
        INSERT INTO role_menu_permissions
            (role_id, menu_id, can_view, can_add, can_edit, can_delete)
        VALUES
            (@role_id, @menu_id, ISNULL(@can_view, 1), ISNULL(@can_add, 0),
             ISNULL(@can_edit, 0), ISNULL(@can_delete, 0));

        SELECT SCOPE_IDENTITY() AS new_permission_id;
    END
    ELSE IF @type = 'Update'
    BEGIN
        UPDATE role_menu_permissions
        SET role_id    = @role_id,
            menu_id    = @menu_id,
            can_view   = @can_view,
            can_add    = @can_add,
            can_edit   = @can_edit,
            can_delete = @can_delete
        WHERE permission_id = @permission_id;
    END
    ELSE IF @type = 'Delete'
    BEGIN
        DELETE FROM role_menu_permissions WHERE permission_id = @permission_id;
    END
    ELSE
    BEGIN
        RAISERROR('Invalid @type. Use GetAll, GetById, Insert, Update, Delete.', 16, 1);
    END
END
GO


-- =====================================================
-- 4.5 GET MENUS FOR A ROLE (drives the left-nav display)
-- Returns only menus the role is allowed to view, in
-- parent -> child order, with the permission flags so the
-- UI can also show/hide Add/Edit/Delete buttons per menu.
-- =====================================================
CREATE OR ALTER PROCEDURE usp_get_menus_by_role
    @role_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.menu_id,
        m.menu_name,
        m.parent_menu_id,
        m.menu_url,
        m.icon_class,
        m.display_order,
        rmp.can_view,
        rmp.can_add,
        rmp.can_edit,
        rmp.can_delete
    FROM menus m
    JOIN role_menu_permissions rmp
        ON rmp.menu_id = m.menu_id
       AND rmp.role_id = @role_id
    WHERE m.is_active = 1
      AND rmp.can_view = 1
    ORDER BY ISNULL(m.parent_menu_id, m.menu_id), m.parent_menu_id, m.display_order, m.menu_id;
END
GO


