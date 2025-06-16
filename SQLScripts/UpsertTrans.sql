CREATE PROCEDURE UpsertTrans @jsonData NVARCHAR(MAX)
AS
BEGIN
    -- ตัวอย่าง: ใช้ OPENJSON หรือแปลงใน client ดีกว่า ถ้าใช้ SQL Server 2016+
    -- หรือเขียน MERGE แยกตามที่เราสร้างไปก่อนหน้านี้
    PRINT 'Process json data here or split insert logic in app'
END

CREATE PROCEDURE [dbo].[UpsertTrans]
  @SaleDate VARCHAR(8),
  @SPID INT,
  @STKCOD VARCHAR(2),
  @SalePrice SMALLMONEY,
  @QtyMember SMALLINT = 0,
  @QtyOther SMALLINT = 0,
  @QtyShop SMALLINT = 0,
  @QtyRetail SMALLINT = 0,
  @Qty711 SMALLINT = 0,
  @QtyYO SMALLINT = 0,
  @QtyCJ SMALLINT = 0,
  @QtyTotal SMALLINT = 0,
  @AmtSale SMALLMONEY = 0,
  @AreaID INT = NULL
AS
BEGIN
  MERGE INTO Trans AS Target
  USING (SELECT @SaleDate AS SaleDate, @SPID AS SPID, @STKCOD AS STKCOD) AS Source
  ON (Target.SaleDate = Source.SaleDate AND Target.SPID = Source.SPID AND Target.STKCOD = Source.STKCOD)
  WHEN MATCHED THEN UPDATE SET 
    SalePrice = @SalePrice, QtyMember = @QtyMember, QtyOther = @QtyOther,
    QtyShop = @QtyShop, QtyRetail = @QtyRetail, Qty711 = @Qty711,
    QtyYO = @QtyYO, QtyCJ = @QtyCJ, QtyTotal = @QtyTotal,
    AmtSale = @AmtSale, AreaID = @AreaID
  WHEN NOT MATCHED THEN INSERT 
    (SaleDate, SPID, STKCOD, SalePrice, QtyMember, QtyOther, QtyShop, QtyRetail, Qty711, QtyYO, QtyCJ, QtyTotal, AmtSale, AreaID)
    VALUES
    (@SaleDate, @SPID, @STKCOD, @SalePrice, @QtyMember, @QtyOther, @QtyShop, @QtyRetail, @Qty711, @QtyYO, @QtyCJ, @QtyTotal, @AmtSale, @AreaID);
END
