Dynamo - Dynamic C# ORM 
=======================
Dynamo is a object relational mapper developed using the c# and .net 4.0 dynamic features and inspired by the feature set of Activerecord for Ruby on Rails.

Aim
---
To create a simple to use code focused ORM that is developed for c# first and can scale from small to large scale solutions. 

Examples
--------
Mapping a class to a table, magically adds all columns in the table as properties in the class.

	// Db Table: Contact
	// int Id - Primary Key
	// nvarchar(255) FirstName
	// nvarchar(255) Surname
	
	public class Contact : Entity
	{
	}

Creating a new Contact Record

	var session = new Session("ConnectionStringHere");
	dynamic contact = new Contact();
	contact.FirstName = "Andy";
	contact.Surname = "Stewart";
	session.Save(contact);

Associations to other classes

	public class Contact : Entity
	{
		public Contact()
		{
			BelongsTo<Company>();
			HasMany<PhoneNumber>();
		}
	}

Returning Objects By Id
	
	var session = new Session("ConnectionStringHere");
	session.GetById<Contact>(1);

Querying the database 
	
	var session = new Session("ConnectionStringHere");

	session.Find<Contact>().Where(:parameters=new { FirstName="Andy" }).OrderBy("FirstName").ToList();
	session.Find<Contact>().Where("FirstName=@FirstName", new { FirstName="Andy" }).ToList();


Querying the database using Dynamic finders
	
	var session = new Session("ConnectionStringHere");
	session.DynamicFind<Contact>().ByFirstName("Andy", "Stewart");
	session.DynamicFind<Contact>().ByFirstNameAndSurname("Andy", "Stewart");


Todo
----
- Caching support
- Pluggable Sql Providers
- Pluggable conventions
- Support for running in memory