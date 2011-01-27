using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dynamo.Commands;
using Dynamo.Provider;

namespace Dynamo
{
    public class Query<T> : DynamicObject where T : Entity
    {
        private readonly IDbProvider dbProvider;

        public Query(IDbProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        public string OrderClause { get; set; }
        public Dictionary<string, object> ConditionParamaters { get; set; }
        public string Condition { get; set; }

        public Query<T> Where(string condition = null, object paramaters = null)
        {
            Condition = condition;

            ConditionParamaters = new Dictionary<string, object>();
            if (paramaters == null) return this;


            var generateCondition = String.IsNullOrEmpty(condition);
            foreach (var paramater in paramaters.GetType().GetProperties())
            {
                if (generateCondition )
                    Condition = paramater.Name + "=@" + paramater.Name;
                
                ConditionParamaters.Add("@" + paramater.Name, paramater.GetValue(paramaters, null));
            }

            return this;
        }

        public IList<dynamic> ToList()
        {
            Mode = QueryMode.Queries;
            return ExecuteQuery().Cast<dynamic>().ToList();
        }

        public int Count(string property)
        {
            Mode = QueryMode.Count;
            return ExecuteQuery().Cast<int>().First();
        }

        public QueryMode Mode { get; set; }

        public string CountProperty { get; set; }

        private IEnumerable ExecuteQuery()
        {
            var command = new FindCommand<T>(this);
            dbProvider.ExecuteCommand(command);
            return command.Result;
        }

        public IEnumerator GetEnumerator()
        {
            return ExecuteQuery().GetEnumerator();
        }

        public Query<T> OrderBy(string orderClause)
        {
            OrderClause = orderClause;
            return this;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var wordSoFar = "";
            var words = new List<string>();
            foreach (var charactor in binder.Name)
            {
                if (Char.IsUpper(charactor))
                {
                    words.Add(wordSoFar);
                    wordSoFar = "";
                }
                wordSoFar += charactor;
            }
            words.RemoveAt(0);
            words.Add(wordSoFar);

            var conditionWord = "";
            var conditionWords = new List<string>();
            foreach (var word in words)
            {
                if (word != "By" && word != "And")
                    conditionWord += word;

                if (word == "And")
                {
                    conditionWords .Add(conditionWord);
                    conditionWord = "";
                }
            }
            conditionWords.Add(conditionWord);

            Condition = "";
            
            ConditionParamaters = new Dictionary<string, object>();
            for (var i = 0; i < conditionWords.Count ; i++)
            {
                Condition += conditionWords[i] + "= @" + conditionWords[i] + " AND ";
                ConditionParamaters.Add(conditionWords[i], args[i]);
            }

            Condition = Condition.Substring(0, Condition.Length - 4);

            result = this;
            return true;
        }

    }

    public enum QueryMode
    {
        Count,
        Queries
    }
}