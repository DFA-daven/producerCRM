# Using Only Stored Procedures
This document is adapted from a walk-through: [MSDN: Using Only Stored Procedures]("http://msdn.microsoft.com/en-us/library/bb399407(v=vs.110).aspx").

## SQLMetal Code Generator
[CodeProject: Using SQLMetal code generator tool for LINQ to SQL](http://www.codeproject.com/Articles/35655/Using-SQLMetal-code-generator-tool-for-LINQ-to-SQL)


## Proof-of-concept
    sqlmetal /code:"c:\linqtest7\northwind.cs" /language:csharp "c:\linqtest7\northwnd.mdf" /sprocs /functions /pluralize

Note: the database name ("northwnd") will be the name of the class.

This failed, giving the following:

  Error : A network-related or instance-specific error occurred while establishing a connection
  to SQL Server. The server was not found or was not accessible. Verify that the instance name 
  is correct and that SQL Server is configured to allow remote connections. (provider: SQL 
  Network Interfaces, error: 26 - Error Locating Server/Instance Specified)

It's not entirely clear why the command above fails, but either of the following work:

  sqlMetal /server:localhost /database:NorthWind /dbml:NorthWind.dbml /namespace:NorthWind.DAL /Context:NorthWindDataContext /sprocs /functions /pluralize

  sqlMetal /server:localhost /database:NorthWind /code:"c:\linqtest7\northwind.cs" /language:csharp /namespace:NorthWind.DAL /Context:NorthWindDataContext /sprocs /functions /pluralize

  sqlMetal /server:localhost /database:NorthWind /code:"c:\linqtest7\northwind.cs" /language:csharp /sprocs /functions /pluralize

Both returns a warning:

  Warning DBML1008: Mapping between DbType 'Decimal(38,2)' and Type 'System.Decimal' in Column
  'TotalPurchase' of Type 'SalesByCategoryResult' may cause data loss when loading from the database.

*Note: the database name ("NorthWind") will be the name of the class.

## Enterprise database
### Generate code
    sqlMetal /server:dl-sqlmilk-01 /database:Enterprise /code:"c:\linqtest7\enterprise.cs" /language:csharp /sprocs /functions /pluralize

### Add LINQ to SQL assembly reference
- In Solution Explorer, right-click References, and then click Add reference.
- In the Add reference dialog box, click .NET, click the System.Data.Linq assembly, and then click ok.

### Add Enterprice code file to Project
- On the Project menu, Add Existing Item.
- In the Add Existing Item dialog box, move to the file location, and then click Add.

### Create a database connection

### Set up the User Interface

### Test