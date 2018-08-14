Copy C:\Development\SalesForceCLR\bin\Debug\SalesForceCLR.dll S:\Databases\CLR\ /Y
Copy C:\Development\SalesForceCLR\bin\Debug\SalesForceCLR.XmlSerializers.dll S:\Databases\CLR\ /Y

osql -S ALTBSAWSSync01 -d TMobile -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql

osql -S ALTBSAWSSync01 -d Salesforce_Gravel -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql

osql -S ALTBSAWSSync01 -d Salesforce_PreProd -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql

osql -S ALTBSAWSSync01 -d Salesforce_Production -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql