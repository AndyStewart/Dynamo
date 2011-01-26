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

        public void Where(string condition, object paramaters)
        {
            Condition = condition;

            ConditionParamaters = new Dictionary<string, object>();
            if (paramaters == null) return;

            foreach (var paramater in paramaters.GetType().GetProperties())
                ConditionParamaters.Add("@" + paramater.Name, paramater.GetValue(paramaters, null));
        }

        public IList<dynamic> ToList()
        {
            return ExecuteQuery().Cast<dynamic>().ToList();
        }

        private IEnumerable<T> ExecuteQuery()
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
}