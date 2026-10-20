-- Задание 9. Атомарность явной транзакции
USE PVR_MyBase_1;
GO

DELETE FROM ВЫПОЛНЕНИЕ WHERE ID_выполнения IN (91, 92);
DELETE FROM РАБОТНИКИ WHERE ID_работника = 91;
GO

BEGIN TRY
    BEGIN TRAN;
        INSERT РАБОТНИКИ VALUES (91, N'Тестов', N'Иван', N'Петрович', N'ул. Тестовая 1', N'111-11', 2);
        INSERT ВЫПОЛНЕНИЕ VALUES (91, N'Сварка', 10, '2024-04-01', 91);
        INSERT ВЫПОЛНЕНИЕ VALUES (92, N'Сварка', -5, '2024-04-02', 91);
    COMMIT TRAN;
END TRY
BEGIN CATCH
    PRINT N'Ошибка: ' + ERROR_MESSAGE();
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
END CATCH;
GO

SELECT * FROM РАБОТНИКИ WHERE ID_работника = 91;
SELECT * FROM ВЫПОЛНЕНИЕ WHERE ID_выполнения IN (91, 92);
GO


-- Задание 9. Точки сохранения (SAVE TRANSACTION)
DELETE FROM ВЫПОЛНЕНИЕ WHERE ID_выполнения IN (93, 94);
DELETE FROM РАБОТНИКИ WHERE ID_работника = 93;
GO

BEGIN TRY
    BEGIN TRAN;
        INSERT РАБОТНИКИ VALUES (93, N'Сохранов', N'Петр', N'Иванович', N'ул. Новая 3', N'333-33', 4);
        SAVE TRAN SAVE_WORKER;
        BEGIN TRY
            INSERT ВЫПОЛНЕНИЕ VALUES (93, N'Фрезеровка', 15, '2024-04-03', 93);
            INSERT ВЫПОЛНЕНИЕ VALUES (94, N'Фрезеровка', -1, '2024-04-04', 93);
        END TRY
        BEGIN CATCH
            PRINT N'Ошибка: ' + ERROR_MESSAGE();
            ROLLBACK TRAN SAVE_WORKER;
        END CATCH;
    COMMIT TRAN;
END TRY
BEGIN CATCH
    PRINT N'Ошибка: ' + ERROR_MESSAGE();
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
END CATCH;
GO

SELECT * FROM РАБОТНИКИ WHERE ID_работника = 93;
SELECT * FROM ВЫПОЛНЕНИЕ WHERE ID_выполнения IN (93, 94);
GO


-- Задание 9. Вложенные транзакции
DELETE FROM ВЫПОЛНЕНИЕ WHERE ID_работника IN (95, 96);
DELETE FROM РАБОТНИКИ WHERE ID_работника IN (95, 96);
GO

BEGIN TRAN OUTER_TRAN;
PRINT N'@@TRANCOUNT = ' + CAST(@@TRANCOUNT AS NVARCHAR(10));
INSERT РАБОТНИКИ VALUES (95, N'Внешний', N'Иван', N'Иванович', N'ул. Внешняя 1', N'555-55', 5);

BEGIN TRAN INNER_TRAN;
PRINT N'@@TRANCOUNT = ' + CAST(@@TRANCOUNT AS NVARCHAR(10));
INSERT РАБОТНИКИ VALUES (96, N'Внутренний', N'Петр', N'Петрович', N'ул. Внутренняя 2', N'666-66', 6);

COMMIT TRAN INNER_TRAN;
PRINT N'@@TRANCOUNT = ' + CAST(@@TRANCOUNT AS NVARCHAR(10));

ROLLBACK TRAN OUTER_TRAN;
PRINT N'@@TRANCOUNT = ' + CAST(@@TRANCOUNT AS NVARCHAR(10));

SELECT * FROM РАБОТНИКИ WHERE ID_работника IN (95, 96);
GO
