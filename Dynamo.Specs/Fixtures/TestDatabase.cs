using System;
using Dynamo.Provider;

namespace Dynamo.Specs.Fixtures
{
    public class TestDatabase
    {
       /// <summary>
       /// This obviously needs tidying up.
       /// </summary>
       public static void Initialise()
       {
           var provider = new SqlProvider(@"Data Source=.\sqlexpress;Initial Catalog=Master;Integrated Security=True");

           try
           {
               provider.ExecuteNonQuery(@"DROP DATABASE [Dynamo_Test]");
           }
           catch (Exception e)
           {
               // Fails if it doesn't exist
           }
           

           // Create Db
           provider.ExecuteNonQuery(@"CREATE DATABASE [Dynamo_Test] ON  PRIMARY 
                        ( NAME = N'Dynamo_Test', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\Dynamo_Test.mdf' , SIZE = 3072KB , FILEGROWTH = 1024KB )
                         LOG ON 
                        ( NAME = N'Dynamo_Test_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\DATA\Dynamo_Test_log.ldf' , SIZE = 1024KB , FILEGROWTH = 10%)");

           var provider2 = new SqlProvider(@"Data Source=.\sqlexpress;Initial Catalog=Dynamo_Test;Integrated Security=True");
           provider2.ExecuteNonQuery(@"CREATE TABLE dbo.Contact
	                    (
	                    Id int NOT NULL IDENTITY (1, 1),
	                    FirstName nvarchar(50) NULL,
	                    Surname nvarchar(50) NULL,
	                    Company_Id int NULL
	                    )  ON [PRIMARY]
                    ALTER TABLE dbo.Contact ADD CONSTRAINT
	                    PK_Table_1 PRIMARY KEY CLUSTERED 
	                    (
	                    Id
	                    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

           provider2.ExecuteNonQuery(@"CREATE TABLE dbo.Company
	                    (
	                    Id int NOT NULL IDENTITY (1, 1),
	                    Name nvarchar(50) NULL
	                    )  ON [PRIMARY]
                    ALTER TABLE dbo.Company ADD CONSTRAINT
	                    PK_Table_2 PRIMARY KEY CLUSTERED 
	                    (
	                    Id
	                    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
       }
    }
}