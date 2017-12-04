truncate table  AddCategoryVendor

truncate table  OpenFood

truncate table  SelesVendorAmount
truncate table SelesVendorAmountDetail
truncate table Temp_VendorBilling

truncate table VendorBillingItem
delete from  VendorBillingMaster
DBCC CHECKIDENT ('[VendorBillingMaster]', RESEED, 0);

delete from  VendorPrice
DBCC CHECKIDENT ('[VendorPrice]', RESEED, 0);

delete from  outletvendor
DBCC CHECKIDENT ('[outletvendor]', RESEED, 0);

update  tbl_KitchenStock set Quantity = 0

truncate table tbl_OutletReturnItem

truncate table tbl_ServiceTax

update  tblBasePriceItems set Vat =5

truncate table tblConsumption
delete from  tblbilldetails
DBCC CHECKIDENT ('[tblbilldetails]', RESEED, 0);

delete from  tblbillmaster
DBCC CHECKIDENT ('[tblbillmaster]', RESEED, 0);


truncate table tblGenBarcode

truncate table tblgrnstock

truncate table  tblOpStckRate

truncate table tblpurchaseditem
truncate table tblpurchasemaster

truncate table tblpurchaseorderitem
truncate table tblpurchaseordermaster

truncate table tblpurchaseReturn
truncate table tblRETURN
truncate table tblSERVICECHARGES

TRUNCATE TABLE tblTransByStock
DELETE FROM  TBLTRANSFER
DBCC CHECKIDENT ('[TBLTRANSFER]', RESEED, 0);

TRUNCATE TABLE tblTransRetRateDet
DELETE FROM TBLTRANSFERRETURNREPORT
DBCC CHECKIDENT ('[TBLTRANSFERRETURNREPORT]', RESEED, 0);










