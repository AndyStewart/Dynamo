A Dynamic ORM, that aims to keep ORM simple while being Code Focused first.

Below is a very early example.


/ Db Table: Contact
// int Id - Primary Key
// nvarchar(255) FirstName
// nvarchar(255) Surname


public class Contact : Entity
{
     public string FullName
        {
                get { return Self.FirstName + " " + Self.Surname;
                   }
        }

        dynamic contact = new Contact();
        contact.FirstName = "Andy";
        contact.Surname = "Stewart";

        var repository = new Respository();
        repository.Save(contact);

        var dbContact = repository.GetById<Contact>(contact.Id);
        Console.WriteLine(dbContact.FullName); // Outputs "Andy Stewart"

