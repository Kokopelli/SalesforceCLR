"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\x64\sgen.exe" /force "C:\Development\SalesForceCLR\bin\Debug\SalesForceCLR.dll"
Copy C:\Development\SalesForceCLR\bin\Debug\SalesForceCLR.dll c:\Databases\CLR\ /Y
Copy C:\Development\SalesForceCLR\bin\Debug\SalesForceCLR.XmlSerializers.dll c:\Databases\CLR\ /Y
osql -S . -d Production -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql
osql -S . -d Test -E -i C:\Development\SalesForceCLR\Resources\UpdateCLR.sql
