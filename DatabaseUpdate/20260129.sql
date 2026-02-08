USE ManagementSystem
GO

ALTER TABLE MstItem ADD
  MinimumStock INT NOT NULL CONSTRAINT [DF_MstItem_MinimumStock] DEFAULT(0)
GO

ALTER TABLE MstItem ADD
  SupplierCode VARCHAR(50) NOT NULL CONSTRAINT [DF_MstItem_SupplierCode] DEFAULT('')
GO
