using System;
using System.Data;

namespace Dynamo.Commands
{
    public class FindByIdCommand<T> : ISqlCommand where T : Entity
    {
        private readonly int id;

        public FindByIdCommand(int id)
        {
            this.id = id;
        }

        public void Execute(IDbCommand dbCommand)
        {
            dbCommand.CommandText = "SELECT * FROM " + typeof(T).Name + " WHERE Id=" + id;

            using (var reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    Result = Activator.CreateInstance<T>();
                    Result.Populate(reader);
                }
            }
        }

        public T Result { get; set; }
    }
}