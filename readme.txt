Dynamo - Dynamic C# ORM 
=======================

A Dynamic ORM, that aims to keep ORM simple while being Code Focused first.

Below is a very early example.

// Db Table: Contact
// int Id - Primary Key
// nvarchar(255) FirstName
// nvarchar(255) Surname


public class Contact : Entity
{
	// Add custom Property, all other properties are loaded dynamically from the db.
	public string FullName
	{
		get { return Self.FirstName + " " + Self.Surname; }
	}
}

// Create New Contact
dynamic contact = new Contact();
contact.FirstName = "Andy";
contact.Surname = "Stewart";

var repository = new Respository();
repository.Save(contact);

// Retrieve contact back from Db.
var dbContact = repository.GetById<Contact>(contact.Id);
Console.WriteLine(dbContact.FullName); // Outputs "Andy Stewart"

